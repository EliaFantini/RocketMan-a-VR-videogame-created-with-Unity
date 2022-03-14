/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class SnapPoint : MonoBehaviour
    {
        [SerializeField, Optional]
        private Collider _collider;
        public Collider Collider => _collider;

        [SerializeField]
        private float _distanceThreshold;
        public float DistanceThreshold => _distanceThreshold;

        #region Inject

        public void InjectOptionalCollider(Collider collider)
        {
            _collider = collider;
        }

        public void InjectOptionalDistanceThreshold(float distanceThreshold)
        {
            _distanceThreshold = distanceThreshold;
        }

        #endregion
    }
}
