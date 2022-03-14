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
    public class GrabInteractable : Interactable<GrabInteractor, GrabInteractable>,
                                      IPointable, IRigidbodyRef
    {
        private Collider[] _colliders;
        public Collider[] Colliders => _colliders;

        [SerializeField]
        Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField]
        private float _releaseDistance = 0f;
        public float ReleaseDistance => _releaseDistance;

        [SerializeField, Optional]
        private PhysicsTransformable _physicsObject = null;

        private static CollisionInteractionRegistry<GrabInteractor, GrabInteractable> _grabRegistry = null;

        public event Action<PointerArgs> OnPointerEvent = delegate { };
        private PointableDelegate<GrabInteractor> _pointableDelegate;

        protected bool _started = false;

        protected virtual void Awake()
        {
            if (_grabRegistry == null)
            {
                _grabRegistry = new CollisionInteractionRegistry<GrabInteractor, GrabInteractable>();
                SetRegistry(_grabRegistry);
            }
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);

            Assert.IsNotNull(Rigidbody);
            _colliders = Rigidbody.GetComponentsInChildren<Collider>();
            Assert.IsTrue(Colliders.Length > 0,
            "The associated Rigidbody must have at least one Collider.");

            _pointableDelegate =
                new PointableDelegate<GrabInteractor>(this, ComputePointer);

            this.EndStart(ref _started);
        }

        public void ApplyVelocities(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            if (_physicsObject == null)
            {
                return;
            }
            _physicsObject.ApplyVelocities(linearVelocity, angularVelocity);
        }

        private void ComputePointer(GrabInteractor interactor, out Vector3 position, out Quaternion rotation)
        {
            position = interactor.GrabPosition;
            rotation = interactor.GrabRotation;
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
            OnPointerEvent(args);
        }

        protected virtual void OnDestroy()
        {
            _pointableDelegate = null;
        }

        #region Inject

        public void InjectAllGrabInteractable(Rigidbody rigidbody)
        {
             InjectRigidbody(rigidbody);
        }

        public void InjectRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void InjectOptionalReleaseDistance(float releaseDistance)
        {
            _releaseDistance = releaseDistance;
        }

        public void InjectOptionalPhysicsObject(PhysicsTransformable physicsObject)
        {
            _physicsObject = physicsObject;
        }

        #endregion
    }
}
