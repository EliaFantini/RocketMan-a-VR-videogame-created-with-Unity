/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction
{
    public class RayInteractor : Interactor<RayInteractor, RayInteractable>
    {
        [SerializeField]
        private Transform _rayOrigin;

        [SerializeField]
        private float _maxRayLength = 5f;

        public Vector3 Origin { get; protected set; }
        public Quaternion Rotation { get; protected set; }
        public Vector3 Forward { get; protected set; }
        public Vector3 End { get; set; }
        public float MaxRayLength => _maxRayLength;
        public RaycastHit? CollisionInfo { get; set; }

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(_rayOrigin);
        }

        protected override void DoEveryUpdate()
        {
            Origin = _rayOrigin.transform.position;
            Rotation = _rayOrigin.transform.rotation;
            Forward = Rotation * Vector3.forward;
        }

        protected override RayInteractable ComputeCandidate()
        {
            RayInteractable closestInteractable = null;

            float closestDist = float.MaxValue;

            End = Origin + MaxRayLength * Forward;
            Ray ray = new Ray(Origin, Forward);

            CollisionInfo = null;
            IEnumerable<RayInteractable> interactables = RayInteractable.Registry.List(this);
            foreach (RayInteractable interactable in interactables)
            {
                Collider collider = interactable.Collider;
                RaycastHit hitInfo;
                if (collider.Raycast(ray, out hitInfo, MaxRayLength))
                {
                    if (hitInfo.distance < closestDist)
                    {
                        closestDist = hitInfo.distance;
                        closestInteractable = interactable;
                        CollisionInfo = hitInfo;
                    }
                }
            }

            float rayDist = (closestInteractable != null ? closestDist : MaxRayLength);
            End = Origin + rayDist * Forward;

            return closestInteractable;
        }

        protected override void DoSelectUpdate(RayInteractable interactable)
        {
            CollisionInfo = null;

            // set default end position and update if interactable is involved.
            End = Origin + MaxRayLength * Forward;

            if (interactable != null)
            {
                Ray ray = new Ray(Origin, Forward);

                Collider collider = interactable.Collider;
                RaycastHit hitInfo;
                if (collider.Raycast(ray, out hitInfo, MaxRayLength))
                {
                    End = hitInfo.point;
                    CollisionInfo = hitInfo;
                }
                else
                {
                    End = Origin + MaxRayLength * Forward;
                }
            }
        }

        #region Inject
        public void InjectAllRayInteractor(Transform rayOrigin)
        {
            InjectRayOrigin(rayOrigin);
        }

        public void InjectRayOrigin(Transform rayOrigin)
        {
            _rayOrigin = rayOrigin;
        }

        public void InjectOptionalMaxRayLength(float maxRayLength)
        {
            _maxRayLength = maxRayLength;
        }
        #endregion
    }
}
