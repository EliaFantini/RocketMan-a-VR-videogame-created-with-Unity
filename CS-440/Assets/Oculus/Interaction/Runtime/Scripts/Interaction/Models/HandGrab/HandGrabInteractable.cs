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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction.HandPosing
{
    /// <summary>
    /// Serializable data-only version of the HandGrabInteractable so it can be stored when they
    /// are generated at Play-Mode (where Hand-tracking works).
    /// </summary>
    [Serializable]
    public struct HandGrabInteractableData
    {
        public List<HandGrabPointData> points;
        public SnapType snapType;
        public GrabTypeFlags grabType;
        public float travelSpeed;
        public PoseMeasureParameters scoringModifier;
    }

    /// <summary>
    /// A HandGrabInteractable indicates the properties about how a hand can snap to an object.
    /// The most important is the position/rotation and finger rotations for the hand,
    /// but it can also contain extra information like a valid holding surface (instead of just
    /// a single point) or a visual representation (using a hand-ghost)
    /// </summary>
    [Serializable]
    public class HandGrabInteractable : Interactable<HandGrabInteractor, HandGrabInteractable>,
        IPointable, ISnappable, IRigidbodyRef, IHandGrabInteractable
    {
        [Header("Grab")]
        /// <summary>
        /// The transform of the object this HandGrabInteractable refers to.
        /// Typically the parent.
        /// </summary>
        [SerializeField]
        private Transform _relativeTo;

        [SerializeField]
        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField, Optional]
        private float _releaseDistance = 0f;

        [SerializeField, Optional]
        private PhysicsTransformable _physicsObject = null;

        [SerializeField]
        private PoseMeasureParameters _scoringModifier = new PoseMeasureParameters(0.1f, 0f);

        [Space]
        [SerializeField]
        private GrabTypeFlags _supportedGrabTypes = GrabTypeFlags.All;
        [SerializeField]
        private GrabbingRule _pinchGrabRules = GrabbingRule.DefaultPinchRule;
        [SerializeField]
        private GrabbingRule _palmGrabRules = GrabbingRule.DefaultPalmRule;

        [Header("Snap")]
        /// <summary>
        /// How the snap will occur.
        /// For example the hand can artificially move to perfectly wrap the object, or the object can move to align with the hand.
        /// </summary>
        [Tooltip("How the snap will occur, for example the hand can artificially move to perfectly wrap the object, or the object can move to align with the hand")]
        [SerializeField]
        private SnapType _snapType;

        /// <summary>
        /// When attracting the object, indicates how many seconds it will take for the object to realign with the hand after a grab
        /// </summary>
        [Tooltip("When attracting the object, indicates the speed (in m/s) for the object to realign with the hand after a grab.")]
        [SerializeField]
        private float _travelSpeed = 1f;

        [SerializeField, Optional]
        private List<HandGrabPoint> _handGrabPoints = new List<HandGrabPoint>();

        /// <summary>
        /// General getter for the transform of the object this interactable refers to.
        /// </summary>
        public Transform RelativeTo => _relativeTo != null ? _relativeTo : this.transform.parent;

        /// <summary>
        /// General getter indicating how the hand and object will align for the grab.
        /// </summary>
        public SnapType SnapType => _snapType;

        public GrabTypeFlags SupportedGrabTypes => _supportedGrabTypes;
        public GrabbingRule PinchGrabRules => _pinchGrabRules;
        public GrabbingRule PalmGrabRules => _palmGrabRules;

        public List<HandGrabPoint> GrabPoints => _handGrabPoints;
        public Collider[] Colliders { get; private set; }
        public float ReleaseDistance => _releaseDistance;

        public event Action<PointerArgs> OnPointerEvent = delegate { };
        private GrabPointsPoseFinder _grabPointsPoseFinder;
        private PointableDelegate<HandGrabInteractor> _pointableDelegate;

        private static CollisionInteractionRegistry<HandGrabInteractor, HandGrabInteractable> _registry = null;

        protected bool _started = false;

        #region editor events
        protected virtual void Reset()
        {
            _relativeTo = this.transform.parent;
        }
        #endregion

        protected virtual void Awake()
        {
            if (_registry == null)
            {
                _registry = new CollisionInteractionRegistry<HandGrabInteractor, HandGrabInteractable>();
                SetRegistry(_registry);
            }
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Rigidbody);
            Colliders = Rigidbody.GetComponentsInChildren<Collider>();
            Assert.IsTrue(Colliders.Length > 0,
            "The associated Rigidbody must have at least one Collider.");

            _grabPointsPoseFinder = new GrabPointsPoseFinder(_handGrabPoints, this.transform);
            _pointableDelegate = new PointableDelegate<HandGrabInteractor>(this, ComputePointer);
            this.EndStart(ref _started);
        }

        private void ComputePointer(HandGrabInteractor interactor, out Vector3 position, out Quaternion rotation)
        {
            position = interactor.GrabPose.position;
            rotation = interactor.GrabPose.rotation;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_started)
            {
                _pointableDelegate.OnPointerEvent += InvokeOnPointerEvent;
            }
        }

        protected override void OnDisable()
        {
            if (_started)
            {
                _pointableDelegate.OnPointerEvent -= InvokeOnPointerEvent;
            }
            base.OnDisable();
        }

        private void InvokeOnPointerEvent(PointerArgs args)
        {
            OnPointerEvent.Invoke(args);
        }

        protected virtual void OnDestroy()
        {
            _pointableDelegate = null;
        }

        #region pose snapping

        public bool CalculateBestPose(Pose userPose, float handScale, Handedness handedness,
            ref HandPose result, ref Pose snapPoint, out bool usesHandPose, out float score)
        {
            return _grabPointsPoseFinder.FindBestPose(userPose, handScale, handedness,
                ref result, ref snapPoint, _scoringModifier, out usesHandPose, out score);
        }

        public bool UsesHandPose()
        {
            return _grabPointsPoseFinder.UsesHandPose();
        }

        #endregion

        #region generation
        /// <summary>
        /// Creates a new HandGrabInteractable under the given object
        /// </summary>
        /// <param name="parent">The relative object for the interactable</param>
        /// <returns>An non-populated HandGrabInteractable</returns>
        public static HandGrabInteractable Create(Transform parent)
        {
            GameObject go = new GameObject("HandGrabInteractable");
            go.transform.SetParent(parent, false);
            HandGrabInteractable record = go.AddComponent<HandGrabInteractable>();
            record._relativeTo = parent;
            return record;
        }

        public HandGrabPoint CreatePoint()
        {
            GameObject go = this.gameObject;
            if (this.TryGetComponent(out HandGrabPoint point))
            {
                go = new GameObject("HandGrab Point");
                go.transform.SetParent(this.transform, false);
            }
            HandGrabPoint record = go.AddComponent<HandGrabPoint>();
            return record;
        }
        #endregion

        #region dataSave
        /// <summary>
        /// Serializes the data of the HandGrabInteractable so it can be stored
        /// </summary>
        /// <returns>The struct data to recreate the interactable</returns>
        public HandGrabInteractableData SaveData()
        {
            return new HandGrabInteractableData()
            {
                snapType = _snapType,
                travelSpeed = _travelSpeed,
                points = _handGrabPoints.Select(p => p.SaveData()).ToList(),
                scoringModifier = _scoringModifier
            };
        }

        /// <summary>
        /// Populates the HandGrabInteractable with the serialized data version
        /// </summary>
        /// <param name="data">The serialized data for the HandGrabInteractable.</param>
        public void LoadData(HandGrabInteractableData data)
        {
            _snapType = data.snapType;
            _travelSpeed = data.travelSpeed;
            _scoringModifier = data.scoringModifier;
            foreach (HandGrabPointData pointData in data.points)
            {
                LoadPoint(pointData);
            }
        }

        public HandGrabPoint LoadPoint(HandGrabPointData pointData)
        {
            HandGrabPoint point = CreatePoint();
            point.LoadData(pointData, this.RelativeTo);
            _handGrabPoints.Add(point);
            return point;
        }
        #endregion

        public void ApplyVelocities(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            if (_physicsObject == null)
            {
                return;
            }
            _physicsObject.ApplyVelocities(linearVelocity, angularVelocity);
        }

        public PoseTravelData CreateTravelData(in Pose from, in Pose to)
        {
            return new PoseTravelData(from, to, _travelSpeed);
        }

        #region Inject

        public void InjectAllHandGrabInteractable(Transform relativeTo, Rigidbody rigidbody,
            GrabTypeFlags supportedGrabTypes, GrabbingRule pinchGrabRules, GrabbingRule palmGrabRules,
            float travelSpeed, SnapType snapType)
        {
            InjectRelativeTo(relativeTo);
            InjectRigidbody(rigidbody);
            InjectTravelSpeed(travelSpeed);
            InjectSnapType(snapType);
            InjectSupportedGrabTypes(supportedGrabTypes);
            InjectPinchGrabRules(pinchGrabRules);
            InjectPalmGrabRules(palmGrabRules);
        }

        public void InjectRelativeTo(Transform relativeTo)
        {
            _relativeTo = relativeTo;
        }

        public void InjectRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void InjectOptionalReleaseDistance(float releaseDistance)
        {
            _releaseDistance = releaseDistance;
        }

        public void InjectSupportedGrabTypes(GrabTypeFlags supportedGrabTypes)
        {
            _supportedGrabTypes = supportedGrabTypes;
        }

        public void InjectPinchGrabRules(GrabbingRule pinchGrabRules)
        {
            _pinchGrabRules = pinchGrabRules;
        }

        public void InjectPalmGrabRules(GrabbingRule palmGrabRules)
        {
            _palmGrabRules = palmGrabRules;
        }

        public void InjectOptionalPhysicsObject(PhysicsTransformable physicsObject)
        {
            _physicsObject = physicsObject;
        }

        public void InjectSnapType(SnapType snapType)
        {
            _snapType = snapType;
        }

        public void InjectTravelSpeed(float travelSpeed)
        {
            _travelSpeed = travelSpeed;
        }

        public void InjectOptionalHandGrabPoints(List<HandGrabPoint> handGrabPoints)
        {
            _handGrabPoints = handGrabPoints;
        }
        #endregion

        #region editor

        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawIcon(this.transform.position, "sv_icon_dot10_pix16_gizmo");
        }

        #endregion
    }
}
