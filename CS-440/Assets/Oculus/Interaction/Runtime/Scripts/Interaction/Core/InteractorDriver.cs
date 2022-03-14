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
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// InteractorDriver provides a means to drive the update loop of an Interactor.
    /// Optionally can be provided a Selector to drive selection of the Interactor.
    /// Optionally can be provided an ActiveState to enable or disable the Interactor.
    /// </summary>
    public class InteractorDriver : MonoBehaviour, IInteractorDriver
    {
        [SerializeField, Interface(typeof(IInteractor))]
        private MonoBehaviour _interactor;

        public IInteractor Interactor;

        [SerializeField, Interface(typeof(ISelector)), Optional]
        private MonoBehaviour _selector;
        private ISelector Selector = null;

        [SerializeField, Interface(typeof(IActiveState)), Optional]
        private MonoBehaviour _activeState;
        private IActiveState ActiveState = null;

        public bool IsRootInteractorDriver { get; set; } = true;

        public bool IsSelectingInteractable => Interactor.HasSelectedInteractable;

        public bool IsHovering => Interactor.State == InteractorState.Hover;
        public bool IsSelecting => Interactor.State == InteractorState.Select;
        public bool HasCandidate => Interactor.HasCandidate;
        public bool ShouldSelect => _performSelect || Interactor.ShouldSelect;

        private bool _performSelect = false;
        private bool _performUnselect = false;
        private bool _selected = false;

        protected bool _started = false;

        protected virtual void Awake()
        {
            Interactor = _interactor as IInteractor;
            Selector = _selector as ISelector;
            ActiveState = _activeState as IActiveState;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Interactor);

            if (_selector != null)
            {
                Assert.IsNotNull(Selector);
            }

            if (_activeState != null)
            {
                Assert.IsNotNull(ActiveState);
            }
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                if (Selector != null)
                {
                    Selector.WhenSelected += HandleSelected;
                    Selector.WhenUnselected += HandleUnselected;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                if (Selector != null)
                {
                    Selector.WhenSelected -= HandleSelected;
                    Selector.WhenUnselected -= HandleUnselected;

                    _performSelect = _performUnselect = false;

                    if (_selected)
                    {
                        _selected = false;
                        Interactor.Unselect();
                    }
                }
            }
        }

        protected virtual void Update()
        {
            if (!IsRootInteractorDriver) return;
            UpdateInteraction();
            UpdateHover();
            UpdateSelection(true);
        }

        private bool UpdateActiveState()
        {
            if (ActiveState == null || ActiveState.Active)
            {
                return true;
            }
            _performSelect = _performUnselect = false;
            Interactor.Disable();
            return false;
        }

        public void UpdateInteraction()
        {
            if (!UpdateActiveState())
            {
                return;
            }
            Interactor.Enable();
            Interactor.UpdateInteractor();
        }

        public void UpdateHover()
        {
            if (!UpdateActiveState())
            {
                return;
            }
            Interactor.Enable();
            Interactor.Hover();
        }

        public void UpdateSelection(bool selectionCanBeEmpty)
        {
            if (!UpdateActiveState())
            {
                return;
            }
            Interactor.Enable();

            _performSelect |= Interactor.ShouldSelect;
            if (_performSelect)
            {
                _selected = true;
                if (selectionCanBeEmpty || Interactor.HasInteractable)
                {
                    Interactor.Select();
                }
            }

            _performUnselect |= Interactor.ShouldUnselect;
            if (_performUnselect)
            {
                _selected = false;
                Interactor.Unselect();
            }

            _performSelect = _performUnselect = false;
        }

        public void HandleSelected()
        {
            _performSelect = true;
        }

        public void HandleUnselected()
        {
            _performUnselect = true;
        }

        public void Enable()
        {
            if (ActiveState != null && !ActiveState.Active) return;
            Interactor.Enable();
        }

        public void Disable()
        {
            _performSelect = _performUnselect = false;
            Interactor.Disable();
        }

        #region Inject

        public void InjectAllInteractorDriver(IInteractor interactor)
        {
            InjectInteractor(interactor);
        }

        public void InjectInteractor(IInteractor interactor)
        {
            _interactor = interactor as MonoBehaviour;
            Interactor = interactor;
        }

        public void InjectOptionalSelector(ISelector selector)
        {
            _selector = selector as MonoBehaviour;
            Selector = selector;
        }

        public void InjectOptionalActiveState(IActiveState activeState)
        {
            _activeState = activeState as MonoBehaviour;
            ActiveState = activeState;
        }
        #endregion
    }
}
