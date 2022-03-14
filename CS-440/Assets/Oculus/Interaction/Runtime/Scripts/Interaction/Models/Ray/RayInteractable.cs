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
    public class RayInteractable : Interactable<RayInteractor, RayInteractable>, IPointable
    {
        [SerializeField]
        private Collider _collider;
        public Collider Collider { get => _collider; }

        [SerializeField, Optional]
        private Transform _pointablePlane = null;

        public event Action<PointerArgs> OnPointerEvent = delegate { };
        private PointableDelegate<RayInteractor> _pointableDelegate;

        protected bool _started = false;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_collider);
            _pointableDelegate = new PointableDelegate<RayInteractor>(this, ComputePointer);
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

        private void InvokePointerEvent(PointerArgs args)
        {
            OnPointerEvent(args);
        }

        private  void ComputePointer(RayInteractor rayInteractor, out Vector3 position, out Quaternion rotation)
        {
            if (_pointablePlane != null)
            {
                var plane = new Plane(-1f * _pointablePlane.forward, _pointablePlane.position);
                var ray = new Ray(rayInteractor.Origin, rayInteractor.Rotation * Vector3.forward);

                float enter;
                if (plane.Raycast(ray, out enter))
                {
                    position = ray.GetPoint(enter);
                    rotation = Quaternion.LookRotation(-1f * _pointablePlane.forward);
                    return;
                }
            }

            rotation = rayInteractor.Rotation;

            if (rayInteractor.CollisionInfo != null)
            {
                position = rayInteractor.CollisionInfo.Value.point;
                return;
            }

            position = Vector3.zero;
        }

        protected virtual void OnDestroy()
        {
            _pointableDelegate = null;
        }

        #region Inject

        public void InjectAllRayInteractable(Collider collider)
        {
            InjectCollider(collider);
        }

        public void InjectCollider(Collider collider)
        {
            _collider = collider;
        }

        public void InjectOptionalPointablePlane(Transform pointablePlane)
        {
            _pointablePlane = pointablePlane;
        }

        #endregion
    }
}
