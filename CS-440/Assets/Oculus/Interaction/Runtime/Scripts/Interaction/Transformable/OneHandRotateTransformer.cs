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
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// A Transformer that rotates the target about an axis.
    /// Updates apply relative rotational changes of a GrabPoint about an axis.
    /// The axis is defined by a pivot transform: a world position and up vector.
    /// </summary>
    public class OneHandRotateTransformer : MonoBehaviour, ITransformer
    {
        public enum Axis
        {
            Right = 0,
            Up = 1,
            Forward = 2
        }

        [SerializeField, Optional]
        private Transform _pivotTransform = null;
        [SerializeField]
        private Axis _rotationAxis = Axis.Up;

        [Serializable]
        public class OneHandRotateConstraints
        {
            public FloatConstraint MinAngle;
            public FloatConstraint MaxAngle;
        }

        [SerializeField]
        private OneHandRotateConstraints _constraints;

        public OneHandRotateConstraints Constraints
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

        private float _relativeAngle = 0.0f;
        private float _constrainedRelativeAngle = 0.0f;

        private ITransformable _transformable;

        public void Initialize(ITransformable transformable)
        {
            _transformable = transformable;
        }

        public void BeginTransform() { }

        public void UpdateTransform()
        {
            var grabPoint = _transformable.GrabPoints[0];
            var targetTransform = _transformable.Transform;

            Transform pivot = _pivotTransform != null ? _pivotTransform : targetTransform;
            Vector3 worldAxis = Vector3.zero;
            worldAxis[(int)_rotationAxis] = 1f;
            Vector3 rotationAxis = pivot.InverseTransformDirection(worldAxis);

            // Project our positional offsets onto a plane with normal equal to the rotation axis
            Vector3 initialOffset = grabPoint.PreviousGrabPosition - pivot.position;
            Vector3 initialVector = Vector3.ProjectOnPlane(initialOffset, rotationAxis);

            Vector3 targetOffset = grabPoint.GrabPosition - pivot.position;
            Vector3 targetVector = Vector3.ProjectOnPlane(targetOffset, rotationAxis);

            // Shortest angle between two planar vectors is the angle about the axis
            // Because we know the vectors are planar, we derive the sign ourselves
            float angleDelta = Vector3.Angle(initialVector, targetVector);
            angleDelta *= Vector3.Dot(Vector3.Cross(initialVector, targetVector), rotationAxis) > 0.0f ? 1.0f : -1.0f;

            float previousAngle = _constrainedRelativeAngle;

            _relativeAngle += angleDelta;
            _constrainedRelativeAngle = _relativeAngle;
            if (_constraints.MinAngle.Constrain)
            {
                _constrainedRelativeAngle = Mathf.Max(_constrainedRelativeAngle, _constraints.MinAngle.Value);
            }

            if (_constraints.MaxAngle.Constrain)
            {
                _constrainedRelativeAngle = Mathf.Min(_constrainedRelativeAngle, _constraints.MaxAngle.Value);
            }

            angleDelta = _constrainedRelativeAngle - previousAngle;

            // Apply this angle rotation about the axis to our transform
            targetTransform.RotateAround(pivot.position, rotationAxis, angleDelta);
        }

        public void EndTransform() { }

        #region Inject

        public void InjectOptionalPivotTransform(Transform pivotTransform)
        {
            _pivotTransform = pivotTransform;
        }

        public void InjectOptionalRotationAxis(Axis rotationAxis)
        {
            _rotationAxis = rotationAxis;
        }

        #endregion
    }
}
