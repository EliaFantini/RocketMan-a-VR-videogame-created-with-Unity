/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class InteractableTransformableConnection : MonoBehaviour
    {
        [SerializeField]
        private Transformable _transformable;

        [SerializeField, Interface(typeof(IInteractable), typeof(IPointable))]
        private MonoBehaviour _pointableInteractable;

        private IPointable Pointable;
        private IInteractable Interactable;

        protected bool _started = false;

        #region Editor

        private void Reset()
        {
            _transformable = this.GetComponentInParent<Transformable>();
        }

        #endregion

        protected virtual void Awake()
        {
            Pointable = _pointableInteractable as IPointable;
            Interactable = _pointableInteractable as IInteractable;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_transformable);
            Assert.IsNotNull(Pointable);
            Assert.IsNotNull(Interactable);
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _transformable.AddPointable(Pointable);
                _transformable.WhenTransformableUpdated += HandleWhenTransformableUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                _transformable.RemovePointable(Pointable);
                _transformable.WhenTransformableUpdated -= HandleWhenTransformableUpdated;
            }
        }

        private void HandleWhenTransformableUpdated(TransformableArgs args)
        {
            if (args.TransformableEvent != TransformableEvent.Transfer)
            {
                return;
            }

            Interactable.RemoveInteractorById(args.GrabIdentifier);
        }

        #region Inject

        public void InjectAllInteractableTransformableConnection<T>(Transformable transformable,
                                                                    T pointableInteractable)
            where T : IInteractable, IPointable
        {
            InjectTransformable(transformable);
            InjectPointableInteractable(pointableInteractable);
        }

        public void InjectTransformable(Transformable transformable)
        {
            _transformable = transformable;
        }

        public void InjectPointableInteractable<T>(T pointableInteractable) where T:IInteractable, IPointable
        {
            _pointableInteractable = pointableInteractable as MonoBehaviour;
            Pointable = _pointableInteractable as IPointable;
            Interactable = _pointableInteractable as IInteractable;
        }

        #endregion
    }
}
