/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class PokeInteractable : Interactable<PokeInteractor, PokeInteractable>, IPointable
    {
        [SerializeField, Interface(typeof(IProximityField))]
        private MonoBehaviour _proximityField;
        public IProximityField ProximityField;

        [SerializeField]
        // The plane "forward" direction should be towards negative Z (to maintain parity with Canvas)
        private Transform _triggerPlaneTransform;
        public Transform TriggerPlaneTransform => _triggerPlaneTransform;

        [SerializeField]
        private float _maxDistance = 0.1f;
        public float MaxDistance => _maxDistance;

        [SerializeField]
        private float _releaseDistance = 0.25f;
        public float ReleaseDistance => _releaseDistance;

        [SerializeField]
        private float _horizontalDragThreshold = 0.0f;
        public float HorizontalDragThreshold => _horizontalDragThreshold;

        [SerializeField]
        private float _verticalDragThreshold = 0.0f;
        public float VerticalDragThreshold => _verticalDragThreshold;

        [SerializeField, Optional]
        private Collider _volumeMask = null;
        public Collider VolumeMask { get => _volumeMask; }

        public event Action<PointerArgs> OnPointerEvent = delegate { };
        private PointableDelegate<PokeInteractor> _pointableDelegate;

        protected bool _started = false;

        protected virtual void Awake()
        {
            ProximityField = _proximityField as IProximityField;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_triggerPlaneTransform);
            Assert.IsNotNull(ProximityField);

            _pointableDelegate = new PointableDelegate<PokeInteractor>(this, ComputePointer);
            this.EndStart(ref _started);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_started)
            {
                _pointableDelegate.OnPointerEvent += InvokePointerEvent;
            }
        }

        protected override void OnDisable()
        {
            if (_started)
            {
                _pointableDelegate.OnPointerEvent -= InvokePointerEvent;
            }

            base.OnDisable();
        }

        public Vector3 ComputeClosestPoint(Vector3 point)
        {
            return ProximityField.ComputeClosestPoint(point);
        }

        private void ComputePointer(PokeInteractor pokeInteractor, out Vector3 position, out Quaternion rotation)
        {
            position = pokeInteractor.TouchPoint;
            rotation = _triggerPlaneTransform.rotation;
        }

        private void InvokePointerEvent(PointerArgs args)
        {
            OnPointerEvent(args);
        }

        protected virtual void OnDestroy()
        {
            _pointableDelegate = null;
        }

        #region Inject

        public void InjectAllPokeInteractable(Transform triggerPlaneTransform,
                                              IProximityField proximityField)
        {
            InjectTriggerPlaneTransform(triggerPlaneTransform);
            InjectProximityField(proximityField);
        }

        public void InjectTriggerPlaneTransform(Transform triggerPlaneTransform)
        {
            _triggerPlaneTransform = triggerPlaneTransform;
        }

        public void InjectProximityField(IProximityField proximityField)
        {
            _proximityField = proximityField as MonoBehaviour;
            ProximityField = proximityField;
        }

        public void InjectOptionalMaxDistance(float maxDistance)
        {
            _maxDistance = maxDistance;
        }

        public void InjectOptionalReleaseDistance(float releaseDistance)
        {
            _releaseDistance = releaseDistance;
        }

        public void InjectHorizontalDragThreshold(float horizontalDragThreshold)
        {
            _horizontalDragThreshold = horizontalDragThreshold;
        }

        public void InjectVerticalDragThreshold(float verticalDragThreshold)
        {
            _verticalDragThreshold = verticalDragThreshold;
        }

        public void InjectOptionalVolumeMask(Collider volumeMask)
        {
            _volumeMask = volumeMask;
        }

        #endregion
    }
}
