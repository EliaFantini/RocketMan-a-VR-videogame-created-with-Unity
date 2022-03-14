/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using Oculus.Interaction.Throw;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class GrabInteractor : Interactor<GrabInteractor, GrabInteractable>, IRigidbodyRef
    {
        [SerializeField]
        private Transform _targetTransform;

        [SerializeField]
        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField, Optional]
        private Transform _grabAnchorTransform;

        [SerializeField, Interface(typeof(IVelocityCalculator)), Optional]
        private MonoBehaviour _velocityCalculator;
        public IVelocityCalculator VelocityCalculator { get; set; }

        private Collider[] _colliders;

        public float BestInteractableWeight { get; private set; } = float.MaxValue;

        public Vector3 GrabPosition => _grabAnchorTransform.position;
        public Quaternion GrabRotation => _grabAnchorTransform.rotation;

        protected override void Awake()
        {
            base.Awake();
            VelocityCalculator = _velocityCalculator as IVelocityCalculator;
        }

        protected override void Start()
        {
            Assert.IsNotNull(_targetTransform);
            Assert.IsNotNull(Rigidbody);

            if (_grabAnchorTransform == null)
            {
                _grabAnchorTransform = _targetTransform;
            }

            Assert.IsNotNull(Rigidbody);

            _colliders = Rigidbody.GetComponentsInChildren<Collider>();
            Assert.IsTrue(_colliders.Length > 0,
            "The associated Rigidbody must have at least one Collider.");
            foreach (Collider collider in _colliders)
            {
                Assert.IsTrue(collider.isTrigger,
                    "Associated Colliders must be marked as Triggers.");
            }

            if (_velocityCalculator != null)
            {
                Assert.IsNotNull(VelocityCalculator);
            }
        }

        protected override void DoEveryUpdate()
        {
            base.DoEveryUpdate();

            transform.position = _targetTransform.position;
            transform.rotation = _targetTransform.rotation;
            // Any collider on the Interactor may need updating for
            // future collision checks this frame
            Physics.SyncTransforms();
        }

        protected override GrabInteractable ComputeCandidate()
        {
            GrabInteractable closestInteractable = null;
            float bestWeight = float.MinValue;
            float weight = bestWeight;

            IEnumerable<GrabInteractable> interactables = GrabInteractable.Registry.List(this);
            foreach (GrabInteractable interactable in interactables)
            {
                Collider[] colliders = interactable.Colliders;
                foreach (Collider collider in colliders)
                {
                    if (Collisions.IsPointWithinCollider(Rigidbody.transform.position, collider))
                    {
                        // Points within a collider are always weighted better than those outside
                        float sqrDistanceFromCenter =
                            (Rigidbody.transform.position - collider.bounds.center).magnitude;
                        weight = float.MaxValue - sqrDistanceFromCenter;
                    }
                    else
                    {
                        var position = Rigidbody.transform.position;
                        Vector3 closestPointOnInteractable = collider.ClosestPoint(position);
                        weight = -1f * (position - closestPointOnInteractable).magnitude;
                    }

                    if (weight > bestWeight)
                    {
                        bestWeight = weight;
                        closestInteractable = interactable;
                    }
                }
            }

            BestInteractableWeight = bestWeight;
            return closestInteractable;
        }

        protected override void InteractableUnselected(GrabInteractable interactable)
        {
            base.InteractableUnselected(interactable);

            ReleaseVelocityInformation throwVelocity = VelocityCalculator != null ?
                VelocityCalculator.CalculateThrowVelocity(interactable.transform) :
                new ReleaseVelocityInformation(Vector3.zero, Vector3.zero, Vector3.zero);
            interactable.ApplyVelocities(throwVelocity.LinearVelocity, throwVelocity.AngularVelocity);
        }

        protected override void DoSelectUpdate(GrabInteractable interactable)
        {
            base.DoSelectUpdate();

            if (interactable == null)
            {
                return;
            }

            if (interactable.ReleaseDistance > 0.0f)
            {
                float closestSqrDist = float.MaxValue;
                Collider[] colliders = interactable.Colliders;
                foreach (Collider collider in colliders)
                {
                    float sqrDistanceFromCenter =
                        (collider.bounds.center - Rigidbody.transform.position).sqrMagnitude;
                    closestSqrDist = Mathf.Min(closestSqrDist, sqrDistanceFromCenter);
                }

                float sqrReleaseDistance = interactable.ReleaseDistance * interactable.ReleaseDistance;

                if (closestSqrDist > sqrReleaseDistance)
                {
                    ShouldUnselect = true;
                }
            }
        }

        #region Inject

        public void InjectAllGrabInteractor(Transform targetTransform, Rigidbody rigidbody)
        {
            InjectTargetTransform(targetTransform);
            InjectRigidbodyRef(rigidbody);
        }

        public void InjectTargetTransform(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public void InjectRigidbodyRef(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void InjectOptionalGrabAnchorTransform(Transform grabAnchorTransform)
        {
            _grabAnchorTransform = grabAnchorTransform;
        }

        public void InjectOptionalVelocityCalculator(IVelocityCalculator velocityCalculator)
        {
            _velocityCalculator = velocityCalculator as MonoBehaviour;
            VelocityCalculator = velocityCalculator;
        }

        #endregion
    }
}
