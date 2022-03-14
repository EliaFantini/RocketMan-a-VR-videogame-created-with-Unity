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

namespace Oculus.Interaction.HandPosing
{
    /// <summary>
    /// Utilities class for HandGrab and DistanceHandGrab interaction common methods.
    /// </summary>
    public static class HandGrabInteractionUtilities
    {
        /// <summary>
        /// Forces the release of an interactable when
        /// the distance to the grabber is big enough.
        /// </summary>
        /// <param name="snappable">The object to check for release</param>
        public static bool CheckReleaseDistance(ISnappable snappable, Vector3 centerPoint, float scaleModifier)
        {
            if (snappable == null
                || snappable.ReleaseDistance <= 0f)
            {
                return false;
            }

            float closestSqrDist = float.MaxValue;
            Collider[] colliders = snappable.Colliders;
            foreach (Collider collider in colliders)
            {
                float sqrDistanceFromCenter = (collider.bounds.center - centerPoint).sqrMagnitude;
                closestSqrDist = Mathf.Min(closestSqrDist, sqrDistanceFromCenter);
            }

            float scaledDistance = snappable.ReleaseDistance * scaleModifier;
            float sqrReleaseDistance = scaledDistance * scaledDistance;

            return (closestSqrDist > sqrReleaseDistance);
        }

        /// <summary>
        /// Calculates the normalized snapback time for a given grab event.
        /// It indicates how close in time we are from the given event.
        /// </summary>
        /// <param name="grabEventTime">Time at which the grab event initiated.</param>
        /// <param name="maxTime">Total time desired for the event. Used for normalization.</param>
        /// <returns>1 at the grab moment, 0 after _snapbackTime seconds or more</returns>
        public static float GetNormalizedEventTime(float grabEventTime, float maxTime)
        {
            return Mathf.Clamp01((Time.realtimeSinceStartup - grabEventTime) / maxTime);
        }
    }
}
