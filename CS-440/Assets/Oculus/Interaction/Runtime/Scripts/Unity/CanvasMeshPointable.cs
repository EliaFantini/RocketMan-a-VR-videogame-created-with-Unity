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
using UnityEngine.Assertions;
using Oculus.Interaction.UnityCanvas;

namespace Oculus.Interaction
{
    public class CanvasMeshPointable : MonoBehaviour, IPointable
    {
        [SerializeField]
        private CanvasRenderTextureMesh _canvasRenderTextureMesh;

        [SerializeField, Interface(typeof(IPointable))]
        private MonoBehaviour _pointable;

        private IPointable Pointable;

        public event Action<PointerArgs> OnPointerEvent = delegate { };

        protected bool _started = false;

        protected virtual void Awake()
        {
            Pointable = _pointable as IPointable;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_canvasRenderTextureMesh);
            Assert.IsNotNull(Pointable);
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (!_started)
            {
                return;
            }

            Pointable.OnPointerEvent += InvokePointerEvent;
        }

        protected virtual void OnDisable()
        {
            if (!_started)
            {
                return;
            }

            Pointable.OnPointerEvent -= InvokePointerEvent;
        }

        private void InvokePointerEvent(PointerArgs args)
        {
            Vector3 transformPosition =
                _canvasRenderTextureMesh.ImposterToCanvasTransformPoint(args.Position);

            OnPointerEvent(new PointerArgs(args.Identifier,
                                           args.PointerEvent,
                                           transformPosition,
                                           args.Rotation));
        }

        #region Inject

        public void InjectAllCanvasMeshPointable(CanvasRenderTextureMesh canvasRenderTextureMesh,
                                                 IPointable pointable)
        {
            InjectCanvasRenderTextureMesh(canvasRenderTextureMesh);
            InjectPointable(pointable);
        }

        public void InjectPointable(IPointable pointable)
        {
            _pointable = pointable as MonoBehaviour;
            Pointable = pointable;
        }

        public void InjectCanvasRenderTextureMesh(CanvasRenderTextureMesh canvasRenderTextureMesh)
        {
            _canvasRenderTextureMesh = canvasRenderTextureMesh;
        }

        #endregion
    }
}
