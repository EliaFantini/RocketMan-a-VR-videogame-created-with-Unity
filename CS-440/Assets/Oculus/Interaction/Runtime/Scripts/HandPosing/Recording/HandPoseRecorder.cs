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
using Oculus.Interaction.HandPosing.Visuals;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction.HandPosing.Recording
{
    /// <summary>
    /// Extract the current pose of the user hand and generates a valid HandGrabInteractable in the
    /// nearest HandPoseRecordable object.
    /// Typically used in play-mode.
    /// </summary>
    public class HandPoseRecorder : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        private IHand Hand;

        [SerializeField]
        private Transform _gripPoint;

        [SerializeField]
        [Min(0f)]
        private float _maxRecordingDistance = 0.05f;

#if UNITY_INPUTSYSTEM
        [SerializeField]
        private string _recordKey = "space";
#else
        [SerializeField]
        private KeyCode _recordKeyCode = KeyCode.Space;
#endif

        [Space(20f)]
        /// <summary>
        /// References the hand prototypes used to represent the HandGrabInteractables. These are the
        /// static hands placed around the interactable to visualize the different holding hand-poses.
        /// Not mandatory.
        /// </summary>
        [SerializeField, Optional]
        [Tooltip("Prototypes of the static hands (ghosts) that visualize holding poses")]
        private HandGhostProvider _ghostProvider;

        /// <summary>
        /// Create an Inspector button for manually triggering the pose recorder.
        /// </summary>
        [InspectorButton("RecordPose")]
        [SerializeField]
        private string _record;

        private HandPoseRecordable[] _allRecordables;

        protected virtual void Start()
        {
            Hand = _hand as IHand;
            Assert.IsNotNull(Hand);

            _allRecordables = GameObject.FindObjectsOfType<HandPoseRecordable>();

#if UNITY_INPUTSYSTEM
            UnityEngine.InputSystem.InputAction recordAction = new UnityEngine.InputSystem.InputAction("onRecordPose", binding: $"<Keyboard>/{_recordKey}");
            recordAction.Enable();
            recordAction.performed += ctx => RecordPose();
#endif
        }

#if !UNITY_INPUTSYSTEM
        protected virtual void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_recordKeyCode))
            {
                RecordPose();
            }
        }
#endif

        /// <summary>
        /// Finds the nearest object that can be snapped to and adds a new HandGrabInteractable to
        /// it with the user hand representation.
        /// </summary>
        public void RecordPose()
        {
            HandPoseRecordable recordable = NearestRecordable();
            if (recordable == null)
            {
                return;
            }
            HandPose trackedHandPose = TrackedPose();
            if(trackedHandPose == null)
            {
                return;
            }
            Pose gripPoint = recordable.transform.RelativeOffset(_gripPoint);
            HandGrabPoint point = recordable.AddHandGrabPoint(trackedHandPose, gripPoint);
            AttachGhost(point);

        }

        private void AttachGhost(HandGrabPoint point)
        {
            if (_ghostProvider != null)
            {
                HandGhost ghostPrefab = _ghostProvider.GetHand(Hand.Handedness);
                HandGhost ghost = GameObject.Instantiate(ghostPrefab, point.transform);
                ghost.SetPose(point);
            }
        }

        private HandPose TrackedPose()
        {
            if (!Hand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints))
            {
                return null;
            }
            HandPose result = new HandPose();
            result.Handedness = Hand.Handedness;
            for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; ++i)
            {
                HandJointId jointID = FingersMetadata.HAND_JOINT_IDS[i];
                result.JointRotations[i] = localJoints[jointID].rotation;
            }
            return result;
        }

        /// <summary>
        /// Finds the nearest recordable (measured from its transform.position) of all the available in the scene.
        /// </summary>
        /// <returns></returns>
        private HandPoseRecordable NearestRecordable()
        {
            float minDistance = float.PositiveInfinity;
            HandPoseRecordable nearRecordable = null;
            foreach (HandPoseRecordable recordable in _allRecordables)
            {
                float distanceToObject = recordable.DistanceToObject(this.transform.position);
                if (distanceToObject == 0f)
                {
                    return recordable;
                }

                if (distanceToObject <= _maxRecordingDistance && distanceToObject < minDistance)
                {
                    minDistance = distanceToObject;
                    nearRecordable = recordable;
                }
            }
            return nearRecordable;
        }
    }
}
