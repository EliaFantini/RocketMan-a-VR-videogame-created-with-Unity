/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using Oculus.Interaction.Throw;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction.HandPosing
{
    using SnapAddress = SnapAddress<HandGrabInteractable>;

    /// <summary>
    /// The HandGrabInteractor allows grabbing objects while having the hands snap to them
    /// adopting a previously authored HandPose.
    /// There are different snapping techniques available, and when None is selected it will
    /// behave as a normal GrabInteractor.
    /// </summary>
    public class HandGrabInteractor : Interactor<HandGrabInteractor, HandGrabInteractable>
        , ISnapper, IRigidbodyRef, IHandGrabInteractor
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;

        private IHand Hand { get; set; }

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private HandGrabAPI _handGrabApi;

        [SerializeField]
        private GrabTypeFlags _supportedGrabTypes = GrabTypeFlags.All;

        [SerializeField]
        private HandWristOffset _gripPoint;
        [SerializeField, Optional]
        private Transform _pinchPoint;

        [FormerlySerializedAs("_optionalVelocityProviderMono")]
        [SerializeField, Interface(typeof(IVelocityCalculator)), Optional]
        private MonoBehaviour _velocityCalculator;
        public IVelocityCalculator VelocityCalculator { get; set; }

        private SnapAddress _currentSnap = new SnapAddress();
        private HandPose _cachedBestHandPose = new HandPose();
        private Pose _cachedBestSnapPoint = new Pose();

        protected bool _isTravelling;
        private PoseTravelData _travelInformation;
        private Pose _grabPose;

        private Pose _wristToGripOffset;
        private Pose _gripToPinchOffset;
        private Pose _trackedGripPose;
        private Grab.HandGrabInteractableData _lastInteractableData = new Grab.HandGrabInteractableData();

        public Pose GrabPose => _grabPose;

        #region IHandGrabInteractor
        public HandGrabAPI HandGrabApi => _handGrabApi;
        public GrabTypeFlags SupportedGrabTypes => _supportedGrabTypes;
        public IHandGrabInteractable TargetInteractable => Interactable;
        #endregion

        #region ISnapper

        public virtual bool IsSnapping => HasSelectedInteractable && !_isTravelling;
        public float SnapStrength { get; private set; }

        public HandFingerFlags SnappingFingers()
        {
            return HandGrab.GrabbingFingers(this, SelectedInteractable);
        }

        public Pose WristToGripOffset => _wristToGripOffset;

        public ISnapData SnapData { get; private set; }
        public System.Action<ISnapper> WhenSnapStarted { get; set; } = delegate { };
        public System.Action<ISnapper> WhenSnapEnded { get; set; } = delegate { };
        #endregion

        #region IRigidbodyRef
        public Rigidbody Rigidbody => _rigidbody;
        #endregion

        protected bool _started = false;

        #region editor events
        protected virtual void Reset()
        {
            _hand = this.GetComponentInParent<IHand>() as MonoBehaviour;
            _handGrabApi = this.GetComponentInParent<HandGrabAPI>();
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            Hand = _hand as IHand;
            VelocityCalculator = _velocityCalculator as IVelocityCalculator;
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, base.Start);

            Assert.IsNotNull(Rigidbody);
            Collider[] colliders = Rigidbody.GetComponentsInChildren<Collider>();
            Assert.IsTrue(colliders.Length > 0,
                "The associated Rigidbody must have at least one Collider.");
            foreach (Collider collider in colliders)
            {
                Assert.IsTrue(collider.isTrigger,
                    "Associated Colliders must be marked as Triggers.");
            }
            Assert.IsNotNull(_handGrabApi);
            Assert.IsNotNull(Hand);
            if (_velocityCalculator != null)
            {
                Assert.IsNotNull(VelocityCalculator);
            }
            this.EndStart(ref _started);
        }

        #region life cycle

        /// <summary>
        /// During the update event, move the current interactor (containing also the
        /// trigger for detecting nearby interactableS) to the tracked position of the grip.
        ///
        /// That is the tracked wrist plus a pregenerated position and rotation offset.
        /// </summary>
        protected override void DoEveryUpdate()
        {
            base.DoEveryUpdate();

            _gripPoint.GetWorldPose(ref _trackedGripPose);
            _gripPoint.GetOffset(ref _wristToGripOffset);

            this.transform.SetPose(_trackedGripPose);
        }

        /// <summary>
        /// Each call while the hand is selecting/grabbing an interactable, it moves the item to the
        /// new position while also attracting it towards the hand if the snapping mode requires it.
        ///
        /// In some cases the parameter can be null, for example if the selection was interrupted
        /// by another hand grabbing the object. In those cases it will come out of the release
        /// state once the grabbing gesture properly finishes.
        /// </summary>
        /// <param name="interactable">The selected item</param>
        protected override void DoSelectUpdate(HandGrabInteractable interactable)
        {
            base.DoSelectUpdate(interactable);
            _isTravelling = false;

            if (interactable == null)
            {
                _currentSnap.Clear();
            }

            Pose grabbingPoint = GrabbingPoint(_currentSnap);
            if (CanTravel(_currentSnap))
            {
                _travelInformation.DestinationPose = grabbingPoint;
                bool travelFinished = _travelInformation.CurrentTravelPose(ref _grabPose);
                _isTravelling = !travelFinished;
            }
            else
            {
                _grabPose = grabbingPoint;
            }

            if (!_isTravelling
                && HandGrabInteractionUtilities.CheckReleaseDistance(interactable,
                    grabbingPoint.position, Hand.Scale))
            {
                ShouldUnselect = true;
            }
            else
            {
                if (interactable != null)
                {
                    HandGrab.StoreGrabData(this, interactable, ref _lastInteractableData);
                    ShouldUnselect = HandGrab.ComputeShouldUnselect(this, interactable);
                }
                else
                {
                    ShouldUnselect = true;
                }
            }
        }

        protected virtual bool CanTravel(SnapAddress snap)
        {
            return !SnapAddress.IsNullOrInvalid(snap)
                && (snap.SnapType == SnapType.HandToObjectAndReturn
                    || snap.SnapType == SnapType.ObjectToHand);
        }

        /// <summary>
        /// Each call while the interactor is hovering, it checks whether there is an interaction
        /// being hovered and sets the target snapping address to it. In the HandToObject snapping
        /// behaviors this is relevant as the hand can approach the object progressively even before
        /// a true grab starts.
        /// </summary>
        protected override void DoHoverUpdate()
        {
            base.DoHoverUpdate();
            if (_currentSnap.IsValidAddress)
            {
                SnapStrength = HandGrab.ComputeHoverStrength(this, Interactable,
                    out GrabTypeFlags hoverGrabTypes);
                SnapData = _currentSnap;
            }
            else
            {
                SnapStrength = 0f;
                SnapData = null;
            }

            if (Interactable != null)
            {
                ShouldSelect = HandGrab.ComputeShouldSelect(this, Interactable,
                    out GrabTypeFlags selectingGrabTypes);
            }
        }

        public override void Select()
        {
            PrepareStartGrab(_currentSnap);
            base.Select();
        }

        /// <summary>
        /// When a new interactable is selected, start the grab at the ideal point. When snapping is
        /// involved that can be a point in the interactable offset from the hand
        /// which will be stored to progressively reduced it in the next updates,
        /// effectively attracting the object towards the hand.
        /// When no snapping is involved the point will be the grip point of the hand directly.
        /// Note: ideally this code would be in InteractableSelected but it needs
        /// to be called before the object is marked as active.
        /// </summary>
        /// <param name="snap">The selected Snap Data </param>
        private void PrepareStartGrab(SnapAddress snap)
        {
            if (SnapAddress.IsNullOrInvalid(snap))
            {
                return;
            }

            Pose grabbingPoint = _trackedGripPose;
            Pose pinchPose = _pinchPoint.GetPose();
            _gripToPinchOffset = PoseUtils.RelativeOffset(pinchPose, _trackedGripPose);
            if (snap.SnappedToPinch)
            {
                grabbingPoint = pinchPose;
            }

            Pose originalGrabPose = snap.Interactable.RelativeTo.GlobalPose(snap.SnapPoint);
            _travelInformation = snap.Interactable.CreateTravelData(originalGrabPose, grabbingPoint);

            if (CanTravel(snap))
            {
                _grabPose.CopyFrom(originalGrabPose);
            }
            else
            {
                _grabPose = grabbingPoint;
            }
        }

        /// <summary>
        /// When releasing an active interactable, calculate the releasing point in similar
        /// fashion to  InteractableSelected
        /// </summary>
        /// <param name="interactable">The released interactable</param>
        protected override void InteractableUnselected(HandGrabInteractable interactable)
        {
            base.InteractableUnselected(interactable);
            if (interactable == null)
            {
                return;
            }

            ReleaseVelocityInformation throwVelocity = VelocityCalculator != null ?
                VelocityCalculator.CalculateThrowVelocity(interactable.transform) :
                new ReleaseVelocityInformation(Vector3.zero, Vector3.zero, Vector3.zero);
            interactable.ApplyVelocities(throwVelocity.LinearVelocity, throwVelocity.AngularVelocity);
        }

        protected override void InteractableSet(HandGrabInteractable interactable)
        {
            base.InteractableSet(interactable);
            WhenSnapStarted.Invoke(this);
        }

        protected override void InteractableUnset(HandGrabInteractable interactable)
        {
            base.InteractableUnset(interactable);
            WhenSnapEnded.Invoke(this);
        }

        #endregion

        #region grab detection

        private bool CanSnapToPinchPoint(HandGrabInteractable interactable, GrabTypeFlags grabTypes)
        {
            return _pinchPoint != null
                && !interactable.UsesHandPose()
                && (grabTypes & GrabTypeFlags.Pinch) != 0;
        }

        private Pose GrabbingPoint(SnapAddress snapAddress)
        {
            if (snapAddress.SnappedToPinch)
            {
                return PoseUtils.Multiply(_trackedGripPose, _gripToPinchOffset);
            }
            else
            {
                return _trackedGripPose;
            }
        }
        #endregion

        /// <summary>
        /// Compute the best interactable to snap to. In order to do it the method measures
        /// the score from the current grip pose to the closes pose in the surfaces
        /// of each one of the interactables in the registry.
        /// Even though it returns the best interactable, it also saves the entire SnapAddress to
        /// it in which the exact pose within the surface is already recorded to avoid recalculations
        /// within the same frame.
        /// </summary>
        /// <returns>The best interactable to snap the hand to.</returns>
        protected override HandGrabInteractable ComputeCandidate()
        {
            ComputeBestSnapAddress(ref _currentSnap);
            return _currentSnap.Interactable;
        }

        protected virtual void ComputeBestSnapAddress(ref SnapAddress snapAddress)
        {
            IEnumerable<HandGrabInteractable> interactables = HandGrabInteractable.Registry.List(this);
            float bestFingerScore = -1f;
            float bestPoseScore = -1f;

            foreach (HandGrabInteractable interactable in interactables)
            {
                float fingerScore = 1.0f;
                if (!HandGrab.ComputeShouldSelect(this, interactable, out GrabTypeFlags selectingGrabTypes))
                {
                    fingerScore = HandGrab.ComputeHoverStrength(this, interactable, out selectingGrabTypes);
                }
                if (fingerScore < bestFingerScore)
                {
                    continue;
                }

                bool usePinchPoint = CanSnapToPinchPoint(interactable, selectingGrabTypes);
                Pose grabPoint = usePinchPoint ? _pinchPoint.GetPose() : _trackedGripPose;
                bool poseFound = interactable.CalculateBestPose(grabPoint, Hand.Scale, Hand.Handedness,
                    ref _cachedBestHandPose, ref _cachedBestSnapPoint,
                    out bool usesHandPose, out float poseScore);

                if (!poseFound)
                {
                    continue;
                }

                if (fingerScore > bestFingerScore
                    || poseScore > bestPoseScore)
                {
                    bestFingerScore = fingerScore;
                    bestPoseScore = poseScore;
                    HandPose handPose = usesHandPose ? _cachedBestHandPose : null;
                    snapAddress.Set(interactable, handPose, _cachedBestSnapPoint, usePinchPoint);
                }

            }

            if (bestFingerScore < 0)
            {
                snapAddress.Clear();
            }
        }

        #region Inject

        public void InjectAllHandGrabInteractor(HandGrabAPI handGrabApi,
            IHand hand, Rigidbody rigidbody, GrabTypeFlags supportedGrabTypes, HandWristOffset gripPoint)
        {
            InjectHandGrabApi(handGrabApi);
            InjectHand(hand);
            InjectRigidbody(rigidbody);
            InjectSupportedGrabTypes(supportedGrabTypes);
            InjectGripPoint(gripPoint);
        }

        public void InjectHandGrabApi(HandGrabAPI handGrabAPI)
        {
            _handGrabApi = handGrabAPI;
        }

        public void InjectHand(IHand hand)
        {
            _hand = hand as MonoBehaviour;
            Hand = hand;
        }

        public void InjectRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void InjectSupportedGrabTypes(GrabTypeFlags supportedGrabTypes)
        {
            _supportedGrabTypes = supportedGrabTypes;
        }

        public void InjectGripPoint(HandWristOffset gripPoint)
        {
            _gripPoint = gripPoint;
        }

        public void InjectOptionalPinchPoint(Transform pinchPoint)
        {
            _pinchPoint = pinchPoint;
        }

        public void InjectOptionalVelocityCalculator(IVelocityCalculator velocityCalculator)
        {
            _velocityCalculator = velocityCalculator as MonoBehaviour;
            VelocityCalculator = velocityCalculator;
        }

        #endregion
    }
}
