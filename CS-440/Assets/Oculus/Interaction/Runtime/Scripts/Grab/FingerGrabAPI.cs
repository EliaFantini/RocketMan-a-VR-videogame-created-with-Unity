/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
    public class FingerGrabAPI : IFingerAPI
    {
        private Vector3 _poseVolumeCenterOffset = Vector3.zero;
        private Vector3 _poseVolumeCenter = Vector3.zero;

        private static readonly float START_THRESHOLD = 0.75f;
        private static readonly float RELEASE_THRESHOLD = 0.25f;
        private static readonly float FINGER_TIP_RADIUS = 0.01f;
        private static readonly float POSE_VOLUME_RADIUS = 0.05f;
        private static readonly Vector3 POSE_VOLUME_OFFSET_RIGHT = new Vector3(0.07f, -0.03f, 0.0f);
        private static readonly Vector3 POSE_VOLUME_OFFSET_LEFT = new Vector3(-0.07f, 0.03f, 0.0f);

        private class FingerGrabData
        {
            private readonly HandJointId _tipId;
            private Vector3 _tipPosition;

            public float GrabStrength;
            public bool IsGrabbing;
            public bool IsGrabbingChanged;

            public FingerGrabData(HandFinger fingerId)
            {
                _tipId = HandJointUtils.GetHandFingerTip(fingerId);
            }

            public void UpdateTipValues(IHand hand)
            {
                if (hand.GetJointPoseFromWrist(_tipId, out Pose pose))
                {
                    _tipPosition = pose.position;
                }
            }

            public void UpdateGrabStrength(Vector3 poseVolumeCenter)
            {
                float outsidePoseVolumeRadius = POSE_VOLUME_RADIUS + FINGER_TIP_RADIUS;
                float insidePoseVolumeRadius = POSE_VOLUME_RADIUS - FINGER_TIP_RADIUS;
                float sqrOutsidePoseVolume = outsidePoseVolumeRadius * outsidePoseVolumeRadius;
                float sqrInsidePoseVolume = insidePoseVolumeRadius * insidePoseVolumeRadius;

                float sqrDist = (poseVolumeCenter - _tipPosition).sqrMagnitude;
                if (sqrDist >= sqrOutsidePoseVolume)
                {
                    GrabStrength = 0.0f;
                }
                else if (sqrDist <= sqrInsidePoseVolume)
                {
                    GrabStrength = 1.0f;
                }
                else
                {
                    float distance = Mathf.Sqrt(sqrDist);
                    GrabStrength = 1.0f - Mathf.Clamp01(
                        (distance - insidePoseVolumeRadius) / (2.0f * FINGER_TIP_RADIUS));
                }
            }

            public void UpdateIsGrabbing()
            {
                IsGrabbingChanged = false;
                if (GrabStrength > START_THRESHOLD)
                {
                    if (!IsGrabbing)
                    {
                        IsGrabbing = true;
                        IsGrabbingChanged = true;
                    }
                    return;
                }

                if (GrabStrength < RELEASE_THRESHOLD)
                {
                    if (IsGrabbing)
                    {
                        IsGrabbing = false;
                        IsGrabbingChanged = true;
                    }
                }
            }
        }

        private readonly FingerGrabData[] _fingersGrabData = {
            new FingerGrabData(HandFinger.Thumb),
            new FingerGrabData(HandFinger.Index),
            new FingerGrabData(HandFinger.Middle),
            new FingerGrabData(HandFinger.Ring),
            new FingerGrabData(HandFinger.Pinky)
        };

        public bool GetFingerIsGrabbing(HandFinger finger)
        {
            return _fingersGrabData[(int)finger].IsGrabbing;
        }

        public bool GetFingerIsGrabbingChanged(HandFinger finger, bool targetGrabState)
        {
            return _fingersGrabData[(int)finger].IsGrabbingChanged &&
                   _fingersGrabData[(int)finger].IsGrabbing == targetGrabState;
        }


        public float GetFingerGrabStrength(HandFinger finger)
        {
            return _fingersGrabData[(int)finger].GrabStrength;
        }


        public Vector3 GetCenterOffset()
        {
            return _poseVolumeCenterOffset;
        }

        public void Update(IHand hand)
        {
            if (hand == null || !hand.IsTrackedDataValid)
            {
                return;
            }

            UpdateVolumeCenter(hand);
            for (int i = 0; i < Constants.NUM_FINGERS; ++i)
            {
                _fingersGrabData[i].UpdateTipValues(hand);
                _fingersGrabData[i].UpdateGrabStrength(_poseVolumeCenter);
                _fingersGrabData[i].UpdateIsGrabbing();
            }
        }

        private void UpdateVolumeCenter(IHand hand)
        {
            if (!hand.GetJointPoseFromWrist(HandJointId.HandWristRoot, out var wristPose))
            {
                return;
            }

            Matrix4x4 wristPoseMat = Matrix4x4.TRS(wristPose.position, wristPose.rotation, Vector3.one);
            _poseVolumeCenterOffset = hand.Handedness == Handedness.Left
                ? POSE_VOLUME_OFFSET_LEFT
                : POSE_VOLUME_OFFSET_RIGHT;

            _poseVolumeCenter = wristPose.position +
                                wristPoseMat.MultiplyVector(_poseVolumeCenterOffset);
        }
    }
}
