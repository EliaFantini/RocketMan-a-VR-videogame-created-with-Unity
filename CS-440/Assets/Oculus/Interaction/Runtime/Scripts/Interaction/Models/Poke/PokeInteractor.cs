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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction
{
    /// <summary>
    /// Defines a near-poke interaction that is driven by a near-distance
    /// proximity computation and a raycast between the position
    /// recorded across two frames against a target plane.
    /// </summary>
    public class PokeInteractor : Interactor<PokeInteractor, PokeInteractable>
    {
        [SerializeField]
        private Transform _pointTransform;

        [SerializeField]
        private float _touchReleaseThreshold = 0.002f;

        [SerializeField]
        private ProgressCurve _dragStartCurve;

        private Vector3 _prevPosition = new Vector3();
        private PokeInteractable _hitInteractable = null;
        public Vector3 ClosestPoint { get; private set; }

        private Vector3 _currentPosition;
        public Vector3 Origin => _currentPosition;

        private Vector3 _touchPoint;
        public Vector3 TouchPoint => _touchPoint;

        private Vector3 _previousTouchPoint;
        private Vector3 _capturedTouchPoint;

        private bool _dragging;
        private Vector3 _lateralDelta;
        private Vector3 _startDragOffset;

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(_pointTransform);
        }

        protected override void DoEveryUpdate()
        {
            _prevPosition = _currentPosition;
            _currentPosition = _pointTransform.position;
        }

        protected override void DoHoverUpdate()
        {
            if (_interactable != null)
            {
                _touchPoint = _interactable.ComputeClosestPoint(Origin);
            }

            if (_hitInteractable != null)
            {
                _hitInteractable = null;
                ShouldSelect = true;
                _dragging = false;
            }
        }

        protected override PokeInteractable ComputeCandidate()
        {
            // First, see if we trigger a press on any interactable
            PokeInteractable closestInteractable = ComputeSelectCandidate();
            if (closestInteractable != null)
            {
                // We have found an active hit target, so we return it
                _hitInteractable = closestInteractable;
                return _hitInteractable;
            }

            // Otherwise we have no active interactable, so we do a proximity-only check for
            // closest hovered interactable (above the trigger plane)
            closestInteractable = ComputeBestHoverInteractable();

            return closestInteractable;
        }

        private PokeInteractable ComputeSelectCandidate()
        {
            PokeInteractable closestInteractable = null;
            float closestSqrDist = float.MaxValue;

            IEnumerable<PokeInteractable> interactables = PokeInteractable.Registry.List(this);

            Vector3 direction = _currentPosition - _prevPosition;
            float magnitude = direction.magnitude;
            if (magnitude == 0f)
            {
                return null;
            }

            direction /= magnitude;
            Ray ray = new Ray(_prevPosition, direction);

            // Check the trigger plane first as a movement through this will
            // automatically put us in a "active" state. We expect the raycast
            // to happen only in one direction
            foreach (PokeInteractable interactable in interactables)
            {
                Transform triggerPlaneTransform = interactable.TriggerPlaneTransform;

                // First check that we are moving towards the trigger plane by checking
                // the direction of our position delta with the forward direction of the trigger transform.
                // This is to not allow presses from "behind" the trigger plane.

                // We consider the plane "forward" normal to be z = -1 (to maintain parity with Canvas)
                if (Vector3.Dot(direction, triggerPlaneTransform.transform.forward) > 0f)
                {
                    // Then do a raycast against the trigger plane defined by the trigger transform
                    Plane triggerPlane = new Plane(-1f * triggerPlaneTransform.forward,
                                                        triggerPlaneTransform.position);

                    float hitDistance;
                    bool hit = triggerPlane.Raycast(ray, out hitDistance);
                    if (hit && hitDistance <= magnitude)
                    {
                        // We collided against the trigger plane and now we must rank this
                        // interactable versus others that also pass this test this frame
                        // but may be at a closer proximity. For this we use the closest
                        // point compute against the plane intersection point
                        Vector3 planePoint = ray.GetPoint(hitDistance);

                        // Check if our collision lies outside of the optional volume mask
                        if (interactable.VolumeMask != null &&
                            !Collisions.IsPointWithinCollider(planePoint, interactable.VolumeMask))
                        {
                            continue;
                        }

                        Vector3 closestPointToHitPoint = interactable.ComputeClosestPoint(planePoint);

                        float sqrDistanceFromPoint = (closestPointToHitPoint - planePoint).sqrMagnitude;

                        if (sqrDistanceFromPoint > interactable.MaxDistance * interactable.MaxDistance) continue;

                        if (sqrDistanceFromPoint < closestSqrDist)
                        {
                            closestSqrDist = sqrDistanceFromPoint;
                            closestInteractable = interactable;
                            ClosestPoint = closestPointToHitPoint;
                            _touchPoint = ClosestPoint;
                        }

                    }
                }
            }
            return closestInteractable;
        }

        private PokeInteractable ComputeBestHoverInteractable()
        {
            PokeInteractable closestInteractable = null;
            float closestSqrDist = float.MaxValue;

            IEnumerable<PokeInteractable> interactables = PokeInteractable.Registry.List(this);

            // We check that we're above the trigger plane first as we don't
            // care about hovers that originate below the trigger plane
            foreach (PokeInteractable interactable in interactables)
            {
                Transform triggerPlaneTransform = interactable.TriggerPlaneTransform;

                Vector3 planeToPoint = _currentPosition - triggerPlaneTransform.position;
                float magnitude = planeToPoint.magnitude;
                if (magnitude != 0f)
                {
                    // We consider the plane "forward" normal to be z = -1 (to maintain parity with Canvas)
                    if (Vector3.Dot(planeToPoint, triggerPlaneTransform.transform.forward) < 0f)
                    {
                        // Check if our position lies outside of the optional volume mask
                        if (interactable.VolumeMask != null &&
                            !Collisions.IsPointWithinCollider(_currentPosition, interactable.VolumeMask))
                        {
                            continue;
                        }

                        // We're above the plane so now we must rank this
                        // interactable versus others that also pass this test this frame
                        // but may be at a closer proximity.
                        Vector3 closestPoint = interactable.ComputeClosestPoint(_currentPosition);

                        float sqrDistanceFromPoint = (closestPoint - _currentPosition).sqrMagnitude;

                        if (sqrDistanceFromPoint > interactable.MaxDistance * interactable.MaxDistance) continue;

                        if (sqrDistanceFromPoint < closestSqrDist)
                        {
                            closestSqrDist = sqrDistanceFromPoint;
                            closestInteractable = interactable;
                            ClosestPoint = closestPoint;
                            _touchPoint = ClosestPoint;
                        }

                    }
                }
            }
            return closestInteractable;
        }

        protected override void InteractableSelected(PokeInteractable interactable)
        {
            if (interactable != null)
            {
                Vector3 worldPosition = ComputePlanePosition(interactable);
                _previousTouchPoint = worldPosition;
                _capturedTouchPoint = worldPosition;
                _lateralDelta = Vector3.zero;
            }

            base.InteractableSelected(interactable);
        }

        private Vector3 ComputePlanePosition(PokeInteractable interactable)
        {
            Vector3 originRelativeToPlane = Origin - interactable.TriggerPlaneTransform.position;
            Vector3 planePosition = Vector3.ProjectOnPlane(originRelativeToPlane, -1f * interactable.TriggerPlaneTransform.forward);
            return planePosition + interactable.TriggerPlaneTransform.position;
        }

        private float ComputeDepth(PokeInteractable interactable, Vector3 loc)
        {
            Vector3 originRelativeToPlane = loc - interactable.TriggerPlaneTransform.position;
            return Vector3.Project(originRelativeToPlane,
                    interactable.TriggerPlaneTransform.forward).magnitude *
                (Vector3.Dot(originRelativeToPlane, interactable.TriggerPlaneTransform.forward) > 0
                    ? 1.0f
                    : 0.0f);
        }

        protected override void DoSelectUpdate(PokeInteractable interactable)
        {
            if (interactable == null)
            {
                ShouldUnselect = true;
                return;
            }

            Transform triggerPlaneTransform = interactable.TriggerPlaneTransform;
            Vector3 triggerToInteractor = _currentPosition - triggerPlaneTransform.position;

            // Unselect our interactor if it is above the plane of the trigger collider by at least releaseDistancePadding
            if (Vector3.Dot(triggerToInteractor, triggerPlaneTransform.forward) < -1f * _touchReleaseThreshold)
            {
                ShouldUnselect = true;
            }

            Vector3 worldPosition = ComputePlanePosition(interactable);

            float planarDelta = (worldPosition - _previousTouchPoint).magnitude;
            float depthDelta = Mathf.Abs(ComputeDepth(interactable, Origin) -
                                         ComputeDepth(interactable, _prevPosition));

            Vector3 frameDelta = worldPosition - _previousTouchPoint;
            bool outsideDelta = false;

            if (!_dragging && planarDelta > depthDelta)
            {
                _lateralDelta += frameDelta;

                while (!outsideDelta)
                {
                    float horizontalDelta =
                        Vector3.Project(_lateralDelta, interactable.TriggerPlaneTransform.right).magnitude;

                    if (horizontalDelta > _selectedInteractable.HorizontalDragThreshold)
                    {
                        outsideDelta = true;
                        break;
                    }

                    float verticalDelta =
                        Vector3.Project(_lateralDelta, interactable.TriggerPlaneTransform.up).magnitude;

                    if (verticalDelta > _selectedInteractable.VerticalDragThreshold)
                    {
                        outsideDelta = true;
                        break;
                    }

                    break;
                }

                if (outsideDelta)
                {
                    _dragStartCurve.Start();
                    _startDragOffset = _capturedTouchPoint - worldPosition;
                    _dragging = true;
                }
            }

            if(!_dragging)
            {
                _touchPoint = _capturedTouchPoint;
            }
            else
            {
                float deltaEase = _dragStartCurve.Progress();
                Vector3 offset = Vector3.Lerp(_startDragOffset, Vector3.zero, deltaEase);

                _touchPoint = worldPosition + offset;
            }

            _previousTouchPoint = worldPosition;

            Vector3 closestPoint = interactable.ComputeClosestPoint(_currentPosition);
            float distanceFromPoint = (closestPoint - _currentPosition).magnitude;
            if (interactable.ReleaseDistance > 0.0f)
            {
                if (distanceFromPoint > interactable.ReleaseDistance)
                {
                    ShouldUnselect = true;
                }
            }
        }

        #region Inject

        public void InjectAllPokeInteractor(Transform pointTransform)
        {
            InjectPointTransform(pointTransform);
        }

        public void InjectPointTransform(Transform pointTransform)
        {
            _pointTransform = pointTransform;
        }

        public void InjectOptionalTouchReleaseThreshold(float touchReleaseThreshold)
        {
            _touchReleaseThreshold = touchReleaseThreshold;
        }

        public void InjectOptionDragStartCurve(ProgressCurve dragStartCurve)
        {
            _dragStartCurve = dragStartCurve;
        }

        #endregion
    }
}
