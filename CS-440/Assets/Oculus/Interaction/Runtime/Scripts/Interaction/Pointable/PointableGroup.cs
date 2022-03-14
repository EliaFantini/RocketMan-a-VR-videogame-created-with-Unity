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
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class PointableGroup : MonoBehaviour, IPointable
    {
        [SerializeField, Interface(typeof(IPointable))]
        private List<MonoBehaviour> _pointables;
        private List<IPointable> Pointables = null;
        public event Action<PointerArgs> OnPointerEvent = delegate { };

        protected virtual void Awake()
        {
            if (_pointables != null)
            {
                Pointables = _pointables.ConvertAll(mono => mono as IPointable);
            }
        }

        protected bool _started = false;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            foreach (IPointable pointable in _pointables)
            {
                Assert.IsNotNull(pointable);
            }
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                foreach (IPointable pointable in Pointables)
                {
                    pointable.OnPointerEvent += ForwardPointerEvent;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                foreach (IPointable pointable in Pointables)
                {
                    pointable.OnPointerEvent -= ForwardPointerEvent;
                }
            }
        }

        private void ForwardPointerEvent(PointerArgs args)
        {
            OnPointerEvent(args);
        }

        #region Inject

        public void InjectAllPointableGroup(List<IPointable> pointables)
        {
            InjectPointables(pointables);
        }

        public void InjectPointables(List<IPointable> pointables)
        {
            Pointables = pointables;
            _pointables = pointables.ConvertAll(pointable => pointable as MonoBehaviour);
        }
        #endregion
    }
}
