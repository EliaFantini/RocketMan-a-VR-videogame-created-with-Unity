/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using Oculus.Interaction.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction.HandPosing.Visuals
{
    /// <summary>
    /// This component is used to drive the HandGrabModifier.
    /// It sets the desired fingers and wrist positions of the hand structure
    /// in the modifier, informing it of any changes coming from the HandGrabInteractor.
    /// </summary>
    public class HandGrabInteractorVisual : MonoBehaviour
    {
        /// <summary>
        /// The HandGrabInteractor, to override the tracked hand
        /// when snapping to something.
        /// </summary>
        [SerializeField]
        [Interface(typeof(ISnapper))]
        private List<MonoBehaviour> _snappers;
        private List<ISnapper> Snappers;

        private ITrackingToWorldTransformer Transformer;

        /// <summary>
        /// The modifier is part of the InputDataStack and this
        /// class will set its values each frame.
        /// </summary>
        [SerializeField]
        private SyntheticHandModifier _modifier;

        private bool _areFingersFree = true;
        private bool _isWristFree = true;

        private ISnapper _currentSnapper;

        protected bool _started = false;

        #region manual initialization
        public static HandGrabInteractorVisual Create(
           GameObject gameObject,
           List<ISnapper> snappers,
           ITrackingToWorldTransformer transformer,
           SyntheticHandModifier modifier)
        {
            HandGrabInteractorVisual component = gameObject.AddComponent<HandGrabInteractorVisual>();
            component.Snappers = snappers;
            component.Transformer = transformer;
            component._modifier = modifier;
            return component;
        }
        #endregion

        protected virtual void Awake()
        {
            Snappers = _snappers.ConvertAll(mono => mono as ISnapper);
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            foreach (ISnapper snapper in Snappers)
            {
                Assert.IsNotNull(snapper);
            }

            Assert.IsNotNull(_modifier);
            Transformer = _modifier.Config.TrackingToWorldTransformer;
            Assert.IsNotNull(Transformer);

            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                RegisterCallbacks(true);
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                RegisterCallbacks(false);
            }
        }

        private void LateUpdate()
        {
            UpdateHand(_currentSnapper);
            _modifier.MarkInputDataRequiresUpdate();
        }

        private void RegisterCallbacks(bool register)
        {
            if (register)
            {
                foreach (ISnapper snapper in _snappers)
                {
                    snapper.WhenSnapStarted += HandleSnapStarted;
                    snapper.WhenSnapEnded += HandleSnapEnded;
                }

            }
            else
            {
                foreach (ISnapper snapper in _snappers)
                {
                    snapper.WhenSnapStarted -= HandleSnapStarted;
                    snapper.WhenSnapEnded -= HandleSnapEnded;
                }

            }
        }
        private void HandleSnapStarted(ISnapper snapper)
        {
            _currentSnapper = snapper;
        }

        private void HandleSnapEnded(ISnapper snapper)
        {
            if (_currentSnapper == snapper)
            {
                _currentSnapper = null;
            }
        }


        private void UpdateHand(ISnapper constrainingSnapper)
        {
            if (constrainingSnapper != null)
            {
                ConstrainingForce(constrainingSnapper, out float fingersConstraint, out float wristConstraint);
                UpdateHandPose(constrainingSnapper, fingersConstraint, wristConstraint);
            }
            else
            {
                FreeFingers();
                FreeWrist();
            }
        }

        private void ConstrainingForce(ISnapper snapper, out float fingersConstraint, out float wristConstraint)
        {
            ISnapData snap = snapper.SnapData;
            fingersConstraint = wristConstraint = 0;
            if (snap == null || snap.HandPose == null)
            {
                return;
            }

            bool isSnapping = snapper.IsSnapping;

            if (snap.SnapType == SnapType.HandToObject
                || snap.SnapType == SnapType.HandToObjectAndReturn)
            {
                fingersConstraint = snapper.SnapStrength;
                wristConstraint = snapper.SnapStrength;
            }
            else if (isSnapping
                && snap.SnapType == SnapType.ObjectToHand)
            {
                fingersConstraint = 1f;
                wristConstraint = 1f;
            }

            if (fingersConstraint >= 1f && !isSnapping)
            {
                fingersConstraint = 0;
            }

            if (wristConstraint >= 1f && !isSnapping)
            {
                wristConstraint = 0f;
            }
        }

        private void UpdateHandPose(ISnapper snapper, float fingersConstraint, float wristConstraint)
        {
            ISnapData snap = snapper.SnapData;

            if (fingersConstraint > 0f)
            {
                UpdateFingers(snap.HandPose, snapper.SnappingFingers(), fingersConstraint);
                _areFingersFree = false;
            }
            else
            {
                FreeFingers();
            }

            if (wristConstraint > 0f)
            {
                Pose wristPose = GetWristPose(snap.WorldSnapPose, snapper.WristToGripOffset);
                wristPose = Transformer.ToTrackingPose(wristPose);
                _modifier.LockWristPose(wristPose, wristConstraint);
                _isWristFree = false;
            }
            else
            {
                FreeWrist();
            }
        }

        /// <summary>
        /// Writes the desired rotation values for each joint based on the provided SnapAddress.
        /// Apart from the rotations it also writes in the modifier if it should allow rotations
        /// past that.
        /// When no snap is provided, it frees all fingers allowing unconstrained tracked motion.
        /// </summary>
        private void UpdateFingers(HandPose handPose, HandFingerFlags grabbingFingers, float strength)
        {
            Quaternion[] desiredRotations = handPose.JointRotations;
            _modifier.OverrideAllJoints(desiredRotations, strength);

            for (int fingerIndex = 0; fingerIndex < Constants.NUM_FINGERS; fingerIndex++)
            {
                int fingerFlag = 1 << fingerIndex;
                JointFreedom fingerFreedom = handPose.FingersFreedom[fingerIndex];
                if (fingerFreedom == JointFreedom.Constrained
                    && ((int)grabbingFingers & fingerFlag) != 0)
                {
                    fingerFreedom = JointFreedom.Locked;
                }
                _modifier.SetFingerFreedom((HandFinger)fingerIndex, fingerFreedom);
            }
        }

        private Pose GetWristPose(Pose gripPoint, Pose wristToGripOffset)
        {
            Pose gripToWrist = wristToGripOffset;
            gripToWrist.Invert();
            gripPoint.Premultiply(gripToWrist);
            return gripPoint;
        }

        private bool FreeFingers()
        {
            if (!_areFingersFree)
            {
                _modifier.FreeAllJoints();
                _areFingersFree = true;
                return true;
            }
            return false;
        }

        private bool FreeWrist()
        {
            if (!_isWristFree)
            {
                _modifier.FreeWrist();
                _isWristFree = true;
                return true;
            }
            return false;
        }
        #region Inject

        public void InjectSnappers(List<ISnapper> snappers)
        {
            _snappers = snappers.ConvertAll(mono => mono as MonoBehaviour);
            Snappers = snappers;
        }
        public void InjectModifier(SyntheticHandModifier modifier)
        {
            _modifier = modifier;
        }

        public void InjectAllHandGrabInteractorVisual(List<ISnapper> snappers, SyntheticHandModifier modifier)
        {
            InjectSnappers(snappers);
            InjectModifier(modifier);
        }

        #endregion
    }
}
