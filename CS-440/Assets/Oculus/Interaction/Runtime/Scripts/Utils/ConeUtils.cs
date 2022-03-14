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

namespace Oculus.Interaction
{
    public class ConeUtils
    {
        public static bool RayWithinCone(Ray ray, Vector3 position, float apertureDegrees)
        {
            float minDotProductThreshold = Mathf.Cos(apertureDegrees * Mathf.Deg2Rad);

            var vectorToInteractable = position - ray.origin;
            var distanceToInteractable = vectorToInteractable.magnitude;

            if (Mathf.Abs(distanceToInteractable) < 0.001f) return true;

            vectorToInteractable /= distanceToInteractable;
            var dotProduct = Vector3.Dot(vectorToInteractable, ray.direction);

            return dotProduct >= minDotProductThreshold;
        }
    }

    [System.Serializable]
    public class ConicalFrustrum
    {
        [HideInInspector]
        public Pose pose;
        [HideInInspector]
        public float minFactor = 1f;

        [Min(0f)]
        public float minLength;
        [Min(0f)]
        public float maxLength;
        [Min(0f)]
        public float radiusStart;
        [Range(0f,60f)]
        public float apertureDegrees;

        public bool IsPointInConeFrustrum(Vector3 point)
        {
            Vector3 coneOriginToPoint = point - pose.position;
            Vector3 pointProjection = Vector3.Project(coneOriginToPoint, pose.forward);
            float pointLength = pointProjection.magnitude;

            if (pointLength < minLength * minFactor
                || pointLength > maxLength)
            {
                return false;
            }

            float pointRadius = Vector3.Distance(pose.position + pointProjection, point);
            return pointRadius <= ConeFrustrumRadiusAtLength(pointLength);
        }

        public float ConeFrustrumRadiusAtLength(float length)
        {
            float angleRadius = Mathf.Asin(apertureDegrees * Mathf.Deg2Rad);
            float radiusEnd = angleRadius * maxLength;

            float lengthRatio = length / maxLength;
            float radiusAtLength = Mathf.Lerp(radiusStart, radiusEnd, lengthRatio);
            return radiusAtLength;
        }
    }
}
