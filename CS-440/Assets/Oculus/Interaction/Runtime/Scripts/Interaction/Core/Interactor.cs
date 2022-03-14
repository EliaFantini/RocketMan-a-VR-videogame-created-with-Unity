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
    /// <summary>
    /// Interactor provides a base template for any kind of interaction.
    /// Interactions can be wholly defined by three things: the concrete Interactor,
    /// the concrete Interactable, and the logic governing their coordination.
    ///
    /// Subclasses are responsible for implementing that coordination logic via template
    /// methods that operate on the concrete interactor and interactable classes.
    /// </summary>
    public abstract class Interactor<TInteractor, TInteractable> : MonoBehaviour, IInteractor<TInteractable>
                                    where TInteractor : class, IInteractor<TInteractable>
                                    where TInteractable : class, IInteractable<TInteractor>
    {
        [SerializeField, Interface(typeof(IInteractableFilter)), Optional]
        private List<MonoBehaviour> _interactableFilters;
        private List<IInteractableFilter> InteractableFilters = null;

        protected virtual void DoEveryUpdate() { }
        protected virtual void DoNormalUpdate() { }
        protected virtual void DoHoverUpdate() { }
        protected virtual void DoSelectUpdate(TInteractable interactable = null) { }

        public virtual bool ShouldSelect { get; protected set; }
        public virtual bool ShouldUnselect { get; protected set; }

        private InteractorState _state = InteractorState.Normal;
        public event Action<InteractorStateChangeArgs> WhenStateChanged = delegate { };
        public event Action WhenInteractorUpdated = delegate { };

        public InteractorState State
        {
            get
            {
                return _state;
            }
            private set
            {
                if (_state == value) return;
                InteractorState previousState = _state;
                _state = value;

                WhenStateChanged(new InteractorStateChangeArgs
                {
                    PreviousState = previousState,
                    NewState = _state
                });
            }
        }

        protected TInteractable _candidate;
        protected TInteractable _interactable;
        protected TInteractable _selectedInteractable;

        public TInteractable Candidate => _candidate;
        public TInteractable Interactable => _interactable;
        public TInteractable SelectedInteractable => _selectedInteractable;

        public bool HasCandidate => _candidate != null;
        public bool HasInteractable => _interactable != null;
        public bool HasSelectedInteractable => _selectedInteractable != null;

        private MultiAction<TInteractable> _whenInteractableSet = new MultiAction<TInteractable>();
        private MultiAction<TInteractable> _whenInteractableUnset = new MultiAction<TInteractable>();
        private MultiAction<TInteractable> _whenInteractableSelected = new MultiAction<TInteractable>();
        private MultiAction<TInteractable> _whenInteractableUnselected = new MultiAction<TInteractable>();
        public MAction<TInteractable> WhenInteractableSet => _whenInteractableSet;
        public MAction<TInteractable> WhenInteractableUnset => _whenInteractableUnset;
        public MAction<TInteractable> WhenInteractableSelected => _whenInteractableSelected;
        public MAction<TInteractable> WhenInteractableUnselected => _whenInteractableUnselected;

        protected virtual void InteractableSet(TInteractable interactable)
        {
            _whenInteractableSet.Invoke(interactable);
        }

        protected virtual void InteractableUnset(TInteractable interactable)
        {
            _whenInteractableUnset.Invoke(interactable);
        }

        protected virtual void InteractableSelected(TInteractable interactable)
        {
            _whenInteractableSelected.Invoke(interactable);
        }

        protected virtual void InteractableUnselected(TInteractable interactable)
        {
            _whenInteractableUnselected.Invoke(interactable);
        }

        private UniqueIdentifier _identifier;
        public int Identifier => _identifier.ID;

        protected virtual void Awake()
        {
            _identifier = UniqueIdentifier.Generate();
            InteractableFilters =
                _interactableFilters.ConvertAll(mono => mono as IInteractableFilter);
        }

        protected virtual void Start()
        {
            foreach (IInteractableFilter filter in InteractableFilters)
            {
                Assert.IsNotNull(filter);
            }
        }

        protected virtual void OnEnable()
        {
            Enable();
        }

        protected virtual void OnDisable()
        {
            Disable();
        }

        protected virtual void OnDestroy()
        {
            UniqueIdentifier.Release(_identifier);
        }

        private void CandidateUpdate()
        {
            if (State == InteractorState.Select) return;
            if (State == InteractorState.Disabled)
            {
                UnsetInteractable();
                return;
            }

            _candidate = ComputeCandidate();
        }

        public void UpdateInteractor()
        {
            if (State == InteractorState.Disabled) return;
            InteractableChangesUpdate();
            DoEveryUpdate();
            NormalUpdate();
            CandidateUpdate();
            HoverUpdate();
            SelectUpdate();
            WhenInteractorUpdated();
        }

        private void InteractableChangesUpdate()
        {
            if (State == InteractorState.Select)
            {
                if (_selectedInteractable != null &&
                    !_selectedInteractable.HasSelectingInteractor(this as TInteractor))
                {
                    TInteractable interactable = _selectedInteractable;
                    _selectedInteractable = null;
                    InteractableUnselected(interactable);
                }

                if (_interactable != null &&
                    !_interactable.HasInteractor(this as TInteractor))
                {
                    TInteractable interactable = _interactable;
                    _interactable = null;
                    InteractableUnset(interactable);
                }
            }

            if(State == InteractorState.Hover &&
               _interactable != null &&
               !_interactable.HasInteractor(this as TInteractor))
            {
                TInteractable interactable = _interactable;
                _interactable = null;
                InteractableUnset(interactable);
                State = InteractorState.Normal;
            }
        }

        public virtual void Select()
        {
            ShouldSelect = false;

            if (State == InteractorState.Select) return;

            TInteractable interactable = _interactable;
            if (interactable != null)
            {
                if (interactable.IsPotentialCandidateFor(this as TInteractor))
                {
                    SelectInteractable(interactable);
                }
                else
                {
                    State = InteractorState.Normal;
                }
            }
            else
            {
                // Selected with no interactable
                State = InteractorState.Select;
            }

            SelectUpdate();
        }

        public virtual void Unselect()
        {
            ShouldUnselect = false;

            if (State != InteractorState.Select) return;
            UnselectInteractable();
            UpdateInteractor();
        }

        // Returns the best interactable for selection or null
        protected abstract TInteractable ComputeCandidate();

        public virtual bool IsFilterPassedBy(TInteractable interactable)
        {
            if (InteractableFilters == null)
            {
                return true;
            }

            foreach (IInteractableFilter interactableFilter in InteractableFilters)
            {
                if (!interactableFilter.FilterInteractable(interactable))
                {
                    return false;
                }
            }
            return true;
        }

        public void Hover()
        {
            if (_candidate != null)
            {
                SetInteractable(_candidate);
                HoverUpdate();
            }
            else
            {
                UnsetInteractable();
            }
        }

        private void NormalUpdate()
        {
            if (State != InteractorState.Normal)
            {
                return;
            }
            DoNormalUpdate();
        }

        private void HoverUpdate()
        {
            if (State != InteractorState.Hover)
            {
                return;
            }
            DoHoverUpdate();
        }

        private void SelectUpdate()
        {
            if (State != InteractorState.Select)
            {
                return;
            }
            DoSelectUpdate(_selectedInteractable);
        }

        private void SetInteractable(TInteractable interactable)
        {
            if (_interactable == interactable) return;
            UnsetInteractable();
            _interactable = interactable;
            interactable.AddInteractor(this as TInteractor);
            InteractableSet(interactable);
            State = InteractorState.Hover;
        }

        private void UnsetInteractable()
        {
            TInteractable interactable = _interactable;
            if (interactable == null) return;
            _interactable = null;
            interactable.RemoveInteractor(this as TInteractor);
            InteractableUnset(interactable);
            State = InteractorState.Normal;
        }

        private void SelectInteractable(TInteractable interactable)
        {
            UnselectInteractable();
            _selectedInteractable = interactable;
            interactable.AddSelectingInteractor(this as TInteractor);
            InteractableSelected(interactable);
            State = InteractorState.Select;
        }

        private void UnselectInteractable()
        {
            TInteractable interactable = _selectedInteractable;
            if (interactable == null)
            {
                State = InteractorState.Normal;
                return;
            }
            interactable.RemoveSelectingInteractor(this as TInteractor);
            _selectedInteractable = null;
            InteractableUnselected(interactable);
            State = InteractorState.Hover;
        }

        public void Enable()
        {
            if (State != InteractorState.Disabled) return;
            State = InteractorState.Normal;
        }

        public void Disable()
        {
            if (State == InteractorState.Disabled) return;
            UnselectInteractable();
            UnsetInteractable();
            State = InteractorState.Disabled;
        }

        #region Inject
        public void InjectOptionalInteractableFilters(List<IInteractableFilter> interactableFilters)
        {
            InteractableFilters = interactableFilters;
            _interactableFilters = interactableFilters.ConvertAll(interactableFilter =>
                                    interactableFilter as MonoBehaviour);
        }
        #endregion
    }
}
