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

namespace Oculus.Interaction
{
    /// <summary>
    /// This Virtual Pointable can broadcasts grab events that can be toggled
    /// from within the Unity inspector using the Grab Flag
    /// </summary>
    public class VirtualPointable : MonoBehaviour, IPointable
    {
        [SerializeField]
        private bool _grabFlag;

        public event Action<PointerArgs> OnPointerEvent;

        private UniqueIdentifier _id;
        private bool _currentlyGrabbing;

        protected virtual void Awake()
        {
            _id = UniqueIdentifier.Generate();
        }

        protected virtual void Update()
        {
            if (_currentlyGrabbing != _grabFlag)
            {
                _currentlyGrabbing = _grabFlag;
                if (_currentlyGrabbing)
                {
                    OnPointerEvent(new PointerArgs(_id.ID, PointerEvent.Hover, transform.position,
                        transform.rotation));
                    OnPointerEvent(new PointerArgs(_id.ID, PointerEvent.Select, transform.position,
                        transform.rotation));
                }
                else
                {
                    OnPointerEvent(new PointerArgs(_id.ID, PointerEvent.Unselect, transform.position,
                        transform.rotation));
                    OnPointerEvent(new PointerArgs(_id.ID, PointerEvent.Unhover, transform.position, transform.rotation));
                }
                return;
            }

            if (_currentlyGrabbing)
            {
                OnPointerEvent(new PointerArgs(_id.ID, PointerEvent.Move, transform.position,
                    transform.rotation));
            }
        }

        public void SetGrabFlag(bool grabFlag)
        {
            _grabFlag = grabFlag;
        }

        protected virtual void OnDestroy()
        {
            UniqueIdentifier.Release(_id);
        }

    }
}
