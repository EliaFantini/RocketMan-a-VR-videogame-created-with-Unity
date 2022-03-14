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

namespace Oculus.Interaction.Input
{
    public class DummyDataModifier : Hand
    {
        public Vector3 offset;
        public float animationTime;

        #region IHandInputDataModifier Implementation
        protected override void Apply(HandDataAsset handDataAsset)
        {
            if (!handDataAsset.IsTracked)
            {
                return;
            }

            var interpolant = Mathf.Sin(Mathf.PI * 2.0f * (Time.time % animationTime) / animationTime) * 0.5f + 0.5f;
            handDataAsset.Root.position = handDataAsset.Root.position + interpolant * offset + offset * -0.5f;

            ref var joint = ref handDataAsset.Joints[(int)HandJointId.HandIndex1];
            var rot = Quaternion.AngleAxis(interpolant * 90 - 45, Vector3.forward);
            joint = joint * rot;
        }
        #endregion
    }
}
