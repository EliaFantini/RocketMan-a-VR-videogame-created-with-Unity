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

namespace Oculus.Interaction
{
    /// <summary>
    /// Interactables implementing IPointable can forward the implementation to this delegate.
    /// </summary>
    public class PointableDelegate<TInteractor> : IPointable where TInteractor : class, IInteractorView
    {
        public event Action<PointerArgs> OnPointerEvent = delegate { };

        private IInteractableView<TInteractor> _interactable;
        public delegate void ComputePointer(TInteractor interactor, out Vector3 position, out Quaternion rotation);

        private ComputePointer _computePointer;
        private Dictionary<TInteractor, Action> _interactorActionMap;

        public PointableDelegate(IInteractableView<TInteractor> interactable, ComputePointer computePointer)
        {
            _interactable = interactable;
            _computePointer = computePointer;
            _interactorActionMap = new Dictionary<TInteractor, Action>();

            _interactable.WhenInteractorAdded.Action += HandleTrackedInteractorAdded;
            _interactable.WhenInteractorRemoved.Action += HandleTrackedInteractorRemoved;
            _interactable.WhenSelectingInteractorAdded.Action += HandleActiveInteractorAdded;
            _interactable.WhenSelectingInteractorRemoved.Action += HandleActiveInteractorRemoved;
        }

        ~PointableDelegate()
        {
            _interactable.WhenInteractorAdded.Action -= HandleTrackedInteractorAdded;
            _interactable.WhenInteractorRemoved.Action -= HandleTrackedInteractorRemoved;
            _interactable.WhenSelectingInteractorAdded.Action -= HandleActiveInteractorAdded;
            _interactable.WhenSelectingInteractorRemoved.Action -= HandleActiveInteractorRemoved;
        }

        private void HandleTrackedInteractorAdded(TInteractor interactor)
        {
            _computePointer(interactor, out Vector3 collisionPoint, out Quaternion collisionRotation);
            OnPointerEvent(new PointerArgs(interactor.Identifier, PointerEvent.Hover, collisionPoint, collisionRotation));

            Action interactorUpdateAction = () => { HandleInteractorUpdate(interactor); };
            _interactorActionMap.Add(interactor, interactorUpdateAction);
            interactor.WhenInteractorUpdated += interactorUpdateAction;
        }

        private void HandleTrackedInteractorRemoved(TInteractor interactor)
        {
            _computePointer(interactor, out Vector3 collisionPoint, out Quaternion collisionRotation);

            Action interactorUpdateAction = _interactorActionMap[interactor];
            _interactorActionMap.Remove(interactor);
            interactor.WhenInteractorUpdated -= interactorUpdateAction;

            OnPointerEvent(new PointerArgs(interactor.Identifier, PointerEvent.Unhover, collisionPoint, collisionRotation));
        }

        private void HandleActiveInteractorAdded(TInteractor interactor)
        {
            _computePointer(interactor, out Vector3 collisionPoint, out Quaternion collisionRotation);
            OnPointerEvent(new PointerArgs(interactor.Identifier, PointerEvent.Select, collisionPoint, collisionRotation));
        }

        private void HandleActiveInteractorRemoved(TInteractor interactor)
        {
            _computePointer(interactor, out Vector3 collisionPoint, out Quaternion collisionRotation);
            OnPointerEvent(new PointerArgs(interactor.Identifier, PointerEvent.Unselect, collisionPoint, collisionRotation));
        }

        private void HandleInteractorUpdate(TInteractor interactor)
        {
            _computePointer(interactor, out Vector3 collisionPoint, out Quaternion collisionRotation);
            OnPointerEvent(new PointerArgs(interactor.Identifier, PointerEvent.Move, collisionPoint, collisionRotation));
        }
    }
}
