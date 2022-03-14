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
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Oculus.Interaction
{
    /// <summary>
    /// This component makes it possible to connect Transformables in the
    /// inspector to Unity Events that are invoked on Transformable Updates
    /// </summary>
    public class TransformableUnityEventWrapper : MonoBehaviour
    {
        [SerializeField, Interface(typeof(ITransformable))]
        private MonoBehaviour _transformable;

        private ITransformable Transformable { get; set; }

        [SerializeField]
        private UnityEvent _onAdd;
        [SerializeField]
        private UnityEvent _onRemove;
        [SerializeField]
        private UnityEvent _onTransfer;
        [SerializeField]
        private UnityEvent _onUpdate;

        public UnityEvent OnAdd => _onAdd;
        public UnityEvent OnRemove => _onRemove;
        public UnityEvent OnTransfer => _onTransfer;
        public UnityEvent OnUpdate => _onUpdate;

        protected bool _started = false;

        protected virtual void Awake()
        {
            Transformable = _transformable as ITransformable;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Transformable);
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Transformable.WhenTransformableUpdated += HandleWhenTransformableUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                Transformable.WhenTransformableUpdated -= HandleWhenTransformableUpdated;
            }
        }

        private void HandleWhenTransformableUpdated(TransformableArgs args)
        {
            switch (args.TransformableEvent)
            {
                case TransformableEvent.Add:
                    _onAdd.Invoke();
                    break;
                case TransformableEvent.Update:
                    break;
                case TransformableEvent.Remove:
                    _onRemove.Invoke();
                    break;
                case TransformableEvent.Transfer:
                    _onTransfer.Invoke();
                    break;
                default:
                    break;
            }
        }

        #region Inject

        public void InjectAllTransformableUnityEventWrapper(ITransformable transformable)
        {
            InjectTransformable(transformable);
        }

        public void InjectTransformable(ITransformable transformable)
        {
            _transformable = transformable as MonoBehaviour;
            Transformable = transformable;
        }

        #endregion
    }
}
