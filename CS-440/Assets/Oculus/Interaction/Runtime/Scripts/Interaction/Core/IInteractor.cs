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

namespace Oculus.Interaction
{
    public struct InteractorStateChangeArgs
    {
        public InteractorState PreviousState;
        public InteractorState NewState;
    }

    /// <summary>
    /// IInteractorView defines the view for an object that can interact with other objects.
    /// </summary>
    public interface IInteractorView
    {
        int Identifier { get; }
        bool HasInteractable { get; }
        bool HasSelectedInteractable { get; }

        InteractorState State { get; }
        event Action<InteractorStateChangeArgs> WhenStateChanged;
        event Action WhenInteractorUpdated;
    }

    /// <summary>
    /// IInteractor defines an object that can interact with other objects
    /// and can handle selection events to change its state.
    /// </summary>
    public interface IInteractor : IInteractorView
    {
        void Enable();
        void Disable();

        void UpdateInteractor();
        void Hover();
        void Select();
        void Unselect();

        bool HasCandidate { get; }
        bool ShouldSelect { get; }
        bool ShouldUnselect { get; }
    }

    /// <summary>
    /// IInteractorView{out TInteractable} defines an InteractorView with concretely typed
    /// Interactable members.
    /// </summary>
    public interface IInteractorView<out TInteractable> : IInteractorView
    {
        MAction<TInteractable> WhenInteractableSet { get; }
        MAction<TInteractable> WhenInteractableUnset { get; }
        MAction<TInteractable> WhenInteractableSelected { get; }
        MAction<TInteractable> WhenInteractableUnselected { get; }
        TInteractable Candidate { get; }
        TInteractable Interactable { get; }
        TInteractable SelectedInteractable { get; }
    }

    /// <summary>
    /// IInteractor{out TInteractable} defines an IInteractor with concretely typed
    /// Interactable members.
    /// </summary>
    public interface IInteractor<TInteractable> : IInteractor, IInteractorView<TInteractable>
    {
        bool IsFilterPassedBy(TInteractable interactable);
    }
}
