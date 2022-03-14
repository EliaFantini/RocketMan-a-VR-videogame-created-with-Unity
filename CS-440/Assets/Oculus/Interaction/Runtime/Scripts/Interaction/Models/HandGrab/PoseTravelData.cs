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
    /// Utility struct containing the information and calculations
    /// to move a pose in space using an  AnimationCurve at a given speed.
    /// Unifies code in the HandGrab and DistanceHandGrab interactors.
    /// </summary>
    public struct PoseTravelData
    {
        private const float DEGREES_TO_PERCEIVED_METERS = 0.1f / 360f;

        private float _startTime;
        private float _totalTime;
        private Pose _sourcePose;
        private AnimationCurve _easingCurve;

        public Pose DestinationPose { private get; set; }

        public PoseTravelData(in Pose from, in Pose to, float speed, AnimationCurve curve = null)
        {
            _startTime = Time.realtimeSinceStartup;
            _sourcePose = from;
            _easingCurve = curve;

            DestinationPose = to;

            Pose grabOffset = PoseUtils.RelativeOffset(from, to);
            float travelDistance = Mathf.Max(grabOffset.position.magnitude,
                (grabOffset.rotation.eulerAngles * DEGREES_TO_PERCEIVED_METERS).magnitude);
            _totalTime = travelDistance / speed;
        }

        public bool CurrentTravelPose(ref Pose currentTravelPose)
        {
            float animationProgress = HandGrabInteractionUtilities.GetNormalizedEventTime(_startTime, _totalTime);
            bool isCompleted = animationProgress >= 1f;
            if (_easingCurve != null)
            {
                animationProgress = _easingCurve.Evaluate(animationProgress);
            }
            PoseUtils.Lerp(_sourcePose, DestinationPose, animationProgress, ref currentTravelPose);
            return isCompleted;
        }
    }
}
