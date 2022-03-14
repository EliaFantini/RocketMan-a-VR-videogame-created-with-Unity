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
    public struct InteractableStateChangeArgs
    {
        public InteractableState PreviousState;
        public InteractableState NewState;
    }

    public interface IComponent
    {
        GameObject gameObject { get; }
    }

    /// <summary>
    /// An IInteractableView defines the view for an object that can be
    /// interacted with.
    /// </summary>
    public interface IInteractableView : IComponent
    {
        InteractableState State { get; }
        event Action<InteractableStateChangeArgs> WhenStateChanged;

        int MaxInteractors { get; }
        int MaxSelectingInteractors { get; }

        int InteractorsCount { get; }
        int SelectingInteractorsCount { get; }

        event Action WhenInteractorsCountUpdated;
        event Action WhenSelectingInteractorsCountUpdated;
    }

    /// <summary>
    /// An object that can be interacted with, an IInteractable can, in addition to
    /// an IInteractableView, be enabled or disabled.
    /// </summary>
    public interface IInteractable : IInteractableView
    {
        void Enable();
        void Disable();
        void RemoveInteractorById(int id);
        new int MaxInteractors { get; set; }
        new int MaxSelectingInteractors { get; set; }
    }

    /// <summary>
    /// An IInteractableView{out TInteractor} defines additional members for IInteractableView
    /// that expose the concrete types interacting with this object.
    /// </summary>
    public interface IInteractableView<out TInteractor> : IInteractableView
    {
        IEnumerable<TInteractor> Interactors { get; }
        IEnumerable<TInteractor> SelectingInteractors { get; }
        MAction<TInteractor> WhenInteractorAdded { get; }
        MAction<TInteractor> WhenInteractorRemoved { get; }
        MAction<TInteractor> WhenSelectingInteractorAdded { get; }
        MAction<TInteractor> WhenSelectingInteractorRemoved { get; }
    }

    /// <summary>
    /// An Interactable{TInteractor} can have its set of Concrete Interactors
    /// modified by an external party.
    /// </summary>
    public interface IInteractable<TInteractor> : IInteractable, IInteractableView<TInteractor>
    {
        bool IsPotentialCandidateFor(TInteractor interactor);
        bool HasInteractor(TInteractor interactor);
        void AddInteractor(TInteractor interactor);
        void RemoveInteractor(TInteractor interactor);

        bool HasSelectingInteractor(TInteractor interactor);
        void AddSelectingInteractor(TInteractor interactor);
        void RemoveSelectingInteractor(TInteractor interactor);
    }
}
