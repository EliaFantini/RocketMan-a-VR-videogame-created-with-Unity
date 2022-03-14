/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction
{
    public class PhysicsTransformable : MonoBehaviour
    {
        [SerializeField]
        private Transformable _transformable;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        [Tooltip("If enabled, the object's mass will scale appropriately as the scale of the object changes.")]
        private bool _scaleMassWithSize = true;

        private bool _savedIsKinematicState = false;
        private bool _isBeingTransformed = false;
        private Vector3 _initialScale;
        private bool _hasPendingForce;
        private Vector3 _linearVelocity;
        private Vector3 _angularVelocity;

        protected bool _started = false;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_transformable);
            Assert.IsNotNull(_rigidbody);
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _transformable.WhenTransformableUpdated += HandleTransformableUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                _transformable.WhenTransformableUpdated -= HandleTransformableUpdated;
            }
        }

        private void HandleTransformableUpdated(TransformableArgs args)
        {
            switch (args.TransformableEvent)
            {
                case TransformableEvent.Add:
                    if (_transformable.GrabPointsCount == 1 && !_isBeingTransformed)
                    {
                        DisablePhysics();
                    }

                    break;
                case TransformableEvent.Remove:
                    if (_transformable.GrabPointsCount == 0)
                    {
                        ReenablePhysics();
                    }

                    break;
            }
        }

        private void DisablePhysics()
        {
            _isBeingTransformed = true;
            CachePhysicsState();
            _rigidbody.isKinematic = true;
        }

        private void ReenablePhysics()
        {
            _isBeingTransformed = false;
            // update the mass based on the scale change
            if (_scaleMassWithSize)
            {
                float initialScaledVolume = _initialScale.x * _initialScale.y * _initialScale.z;

                Vector3 currentScale = _rigidbody.transform.localScale;
                float currentScaledVolume = currentScale.x * currentScale.y * currentScale.z;

                float changeInMassFactor = currentScaledVolume / initialScaledVolume;
                _rigidbody.mass *= changeInMassFactor;
            }

            // revert the original kinematic state
            _rigidbody.isKinematic = _savedIsKinematicState;
        }

        public void ApplyVelocities(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            _hasPendingForce = true;
            _linearVelocity = linearVelocity;
            _angularVelocity = angularVelocity;
        }

        private void FixedUpdate()
        {
            if (_hasPendingForce)
            {
                _hasPendingForce = false;
                _rigidbody.AddForce(_linearVelocity, ForceMode.VelocityChange);
                _rigidbody.AddTorque(_angularVelocity, ForceMode.VelocityChange);
            }
        }

        private void CachePhysicsState()
        {
            _savedIsKinematicState = _rigidbody.isKinematic;
            _initialScale = _rigidbody.transform.localScale;
        }

        #region Inject

        public void InjectAllPhysicsTransformable(Transformable transformable, Rigidbody rigidbody)
        {
            InjectTransformable(transformable);
            InjectRigidbody(rigidbody);
        }

        public void InjectTransformable(Transformable transformable)
        {
            _transformable = transformable;
        }

        public void InjectRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void InjectOptionalScaleMassWithSize(bool scaleMassWithSize)
        {
            _scaleMassWithSize = scaleMassWithSize;
        }

        #endregion
    }
}
