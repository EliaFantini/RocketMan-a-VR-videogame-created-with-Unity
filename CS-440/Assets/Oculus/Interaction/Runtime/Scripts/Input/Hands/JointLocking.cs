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

namespace Oculus.Interaction.Input
{
    public class JointLock
    {
        public HandJointId JointId;
        public bool Locked;
        public Quaternion SourceRotation;

        public Quaternion AnimationStartRotation;
        public Quaternion AnimationTargetRotation;
        public float AnimationTime = 0f;
        public Quaternion OutputRotation;
    }

    public class JointLocking
    {
        public static void Initialize(HandDataAsset data,
            out Dictionary<HandJointId, JointLock> jointMap)
        {
            jointMap = new Dictionary<HandJointId, JointLock>();
            foreach (HandJointId jointId in HandJointUtils.JointIds)
            {
                JointLock jointLock = new JointLock()
                {
                    JointId = jointId,
                    SourceRotation = data.Joints[(int)jointId],
                    AnimationTargetRotation = data.Joints[(int)jointId],
                    OutputRotation = data.Joints[(int)jointId],
                    AnimationTime = 0f,
                    Locked = false
                };

                jointMap.Add(jointId, jointLock);
            }
        }

        public static void UpdateLockedJoints(
            HandDataAsset data,
            Dictionary<HandJointId, JointLock> jointMap,
            float totalEaseTime,
            AnimationCurve animationCurve)
        {
            foreach (JointLock angleLock in jointMap.Values)
            {
                angleLock.SourceRotation = data.Joints[(int)angleLock.JointId];
                if (!angleLock.Locked)
                {
                    angleLock.AnimationTargetRotation = data.Joints[(int)angleLock.JointId];
                }

                angleLock.AnimationTime = Math.Min(angleLock.AnimationTime + Time.deltaTime /
                    totalEaseTime, 1.0f);
                float easeTime = animationCurve.Evaluate(angleLock.AnimationTime);

                angleLock.OutputRotation = Quaternion.Slerp(angleLock.AnimationStartRotation,
                    angleLock.AnimationTargetRotation, easeTime);
                data.Joints[(int)angleLock.JointId] = angleLock.OutputRotation;
            }
        }

        public static void LockJoint(
            HandJointId jointId,
            Quaternion targetRotation,
            Dictionary<HandJointId, JointLock> jointMap)
        {
            if (jointMap[jointId].Locked)
            {
                return;
            }

            jointMap[jointId].Locked = true;
            jointMap[jointId].AnimationTargetRotation = targetRotation;
            jointMap[jointId].AnimationStartRotation = jointMap[jointId].OutputRotation;
            jointMap[jointId].AnimationTime = 0f;
        }

        public static void UnlockJoint(HandJointId jointId,
            Dictionary<HandJointId, JointLock> jointMap)
        {
            if (!jointMap[jointId].Locked) return;

            jointMap[jointId].Locked = false;
            jointMap[jointId].AnimationStartRotation = jointMap[jointId].OutputRotation;
            jointMap[jointId].AnimationTime = 0f;
        }

        public static void UnlockAllJoints(Dictionary<HandJointId, JointLock> jointMap)
        {
            foreach (var jointId in jointMap.Keys)
            {
                UnlockJoint(jointId, jointMap);
            }
        }

        public static void LockByFingerId(HandFinger finger,
            Dictionary<HandJointId, JointLock> jointMap)
        {
            HandJointId[] joints = HandJointUtils.FingerToJointList[(int)finger];
            foreach (var jointId in joints)
            {
                LockJoint(jointId, jointMap[jointId].SourceRotation, jointMap);
            }
        }

        public static void UnlockByFingerId(HandFinger finger,
            Dictionary<HandJointId, JointLock> jointMap)
        {
            HandJointId[] joints = HandJointUtils.FingerToJointList[(int)finger];
            foreach (var jointId in joints)
            {
                UnlockJoint(jointId, jointMap);
            }
        }
    }
}
