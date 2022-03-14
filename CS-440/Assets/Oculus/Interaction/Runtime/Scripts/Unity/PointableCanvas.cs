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
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// PointerCanvas allows any IPointable to forward its
    /// events onto an associated Canvas via the IPointableCanvas interface
    /// Requires a PointableCanvasModule present in the scene.
    /// </summary>
    public class PointableCanvas : MonoBehaviour, IPointableCanvas
    {
        [SerializeField, Interface(typeof(IPointable))]
        private MonoBehaviour _pointable;
        private IPointable Pointable;

        [SerializeField]
        private Canvas _canvas;
        public Canvas Canvas => _canvas;

        public event Action<PointerArgs> OnPointerEvent = delegate { };

        private bool _registered = false;

        protected bool _started = false;

        protected virtual void Awake()
        {
            Pointable = _pointable as IPointable;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Pointable);
            this.EndStart(ref _started);
        }

        private void Register()
        {
            PointableCanvasModule.RegisterPointableCanvas(this);
            Pointable.OnPointerEvent += HandlePointerEvent;
            _registered = true;
        }

        private void Unregister()
        {
            if (!_registered) return;
            Pointable.OnPointerEvent -= HandlePointerEvent;
            PointableCanvasModule.UnregisterPointableCanvas(this);
            _registered = false;
        }

        private void HandlePointerEvent(PointerArgs args)
        {
            OnPointerEvent(args);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Register();
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                Unregister();
            }
        }
    }
}
