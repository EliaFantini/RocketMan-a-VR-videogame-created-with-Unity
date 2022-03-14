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
using System;

namespace Oculus.Interaction
{
    /// <summary>
    /// A Transformer that transforms the target in a free form way for an intuitive
    /// two hand translation, rotation and scale.
    /// </summary>
    public class TwoHandFreeTransformer : MonoBehaviour, ITransformer
    {
        // The active rotation for this transformation is tracked because it
        // cannot be derived each frame from the grab point information alone.
        private Quaternion _activeRotation;
        private Vector3 _initialTransformableLocalScale;
        private float _initialDistance;
        private float _initialScale = 1.0f;
        private float _activeScale = 1.0f;

        [Serializable]
        public class TwoHandFreeConstraints
        {
            [Tooltip("If true then the constraints are relative to the initial scale of the object " +
                     "if false, constraints are absolute with respect to the object's x-axis scale.")]
            public bool ConstraintsAreRelative;
            public FloatConstraint MinScale;
            public FloatConstraint MaxScale;
        }

        [SerializeField]
        private TwoHandFreeConstraints _constraints;

        public TwoHandFreeConstraints Constraints
        {
            get
            {
                return _constraints;
            }

            set
            {
                _constraints = value;
            }
        }

        private ITransformable _transformable;


        public void Initialize(ITransformable transformable)
        {
            _transformable = transformable;

            if (!_constraints.ConstraintsAreRelative)
            {
                _activeScale = transformable.Transform.localScale.x;
            }
        }

        public void BeginTransform()
        {
            var grabA = _transformable.GrabPoints[0];
            var grabB = _transformable.GrabPoints[1];

            // Initialize our transformer rotation
            Vector3 diff = grabB.GrabPosition - grabA.GrabPosition;
            _activeRotation = Quaternion.LookRotation(diff, Vector3.up).normalized;
            _initialDistance = diff.magnitude;
            _initialScale = _activeScale;
            _initialTransformableLocalScale = _transformable.Transform.localScale / _initialScale;
        }

        public void UpdateTransform()
        {
            var grabA = _transformable.GrabPoints[0];
            var grabB = _transformable.GrabPoints[1];
            var targetTransform = _transformable.Transform;

            // Use the centroid of our grabs as the transformation center
            Vector3 initialCenter = Vector3.Lerp(grabA.PreviousGrabPosition, grabB.PreviousGrabPosition, 0.5f);
            Vector3 targetCenter = Vector3.Lerp(grabA.GrabPosition, grabB.GrabPosition, 0.5f);

            // Our transformer rotation is based off our previously saved rotation
            Quaternion initialRotation = _activeRotation;

            // The base rotation is based on the delta in vector rotation between grab points
            Vector3 initialVector = grabB.PreviousGrabPosition - grabA.PreviousGrabPosition;
            Vector3 targetVector = grabB.GrabPosition - grabA.GrabPosition;
            Quaternion baseRotation = Quaternion.FromToRotation(initialVector, targetVector);

            // Any local grab point rotation contributes 50% of its rotation to the final transformation
            // If both grab points rotate the same amount locally, the final result is a 1-1 rotation
            Quaternion deltaA = grabA.GrabRotation * Quaternion.Inverse(grabA.PreviousGrabRotation);
            Quaternion halfDeltaA = Quaternion.Slerp(Quaternion.identity, deltaA, 0.5f);

            Quaternion deltaB = grabB.GrabRotation * Quaternion.Inverse(grabB.PreviousGrabRotation);
            Quaternion halfDeltaB = Quaternion.Slerp(Quaternion.identity, deltaB, 0.5f);

            // Apply all the rotation deltas
            Quaternion baseTargetRotation = baseRotation * halfDeltaA * halfDeltaB * initialRotation;

            // Normalize the rotation
            Vector3 upDirection = baseTargetRotation * Vector3.up;
            Quaternion targetRotation = Quaternion.LookRotation(targetVector, upDirection).normalized;

            // Save this target rotation as our active rotation state for future updates
            _activeRotation = targetRotation;

            // Scale logic
            float activeDistance = targetVector.magnitude;
            if(Mathf.Abs(activeDistance) < 0.0001f) activeDistance = 0.0001f;

            float scalePercentage = activeDistance / _initialDistance;

            float previousScale = _activeScale;
            _activeScale = _initialScale * scalePercentage;

            if(_constraints.MinScale.Constrain)
            {
                _activeScale = Mathf.Max(_constraints.MinScale.Value, _activeScale);
            }
            if(_constraints.MaxScale.Constrain)
            {
                _activeScale = Mathf.Min(_constraints.MaxScale.Value, _activeScale);
            }

            // Apply the positional delta initialCenter -> targetCenter and the
            // rotational delta initialRotation -> targetRotation to the target transform
            Vector3 worldOffsetFromCenter = targetTransform.position - initialCenter;

            Vector3 offsetInTargetSpace = Quaternion.Inverse(initialRotation) * worldOffsetFromCenter;
            offsetInTargetSpace /= previousScale;

            Quaternion rotationInTargetSpace = Quaternion.Inverse(initialRotation) * targetTransform.rotation;

            targetTransform.position = (targetRotation * (_activeScale * offsetInTargetSpace)) + targetCenter;
            targetTransform.rotation = targetRotation * rotationInTargetSpace;
            targetTransform.localScale = _activeScale * _initialTransformableLocalScale;
        }

        public void MarkAsBaseScale()
        {
            _activeScale = 1.0f;
        }

        public void EndTransform() { }

        #region Inject

        public void InjectOptionalTwoHandFreeConstraints(TwoHandFreeConstraints constraints)
        {
            _constraints = constraints;
        }

        #endregion
    }
}
