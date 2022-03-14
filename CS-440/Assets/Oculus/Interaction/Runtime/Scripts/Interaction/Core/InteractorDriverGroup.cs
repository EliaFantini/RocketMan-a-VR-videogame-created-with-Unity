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
    /// The InteractorDriverGroup coordinates between multiple InteractorDrivers to determine
    /// which InteractorDriver(s) should be enabled at a time.
    /// Passed in InteractorDrivers are prioritized in list order (first = highest priority).
    public class InteractorDriverGroup : MonoBehaviour, IInteractorDriver
    {
        [SerializeField, Interface(typeof(IInteractorDriver))]
        private List<MonoBehaviour> _interactorDrivers;

        private List<IInteractorDriver> InteractorDrivers;

        // The HoverStrategy defines how this InteractorDriverGroup coordinates hovering
        // between InteractorDrivers when no InteractorDriver is yet selected.
        // Upon selection, all other drivers are disabled.
        public enum InteractorDriverGroupStrategy
        {
            // Keep checking all drivers for hover in priority order, disabling all others
            PRIORITY = 0,
            // When any driver is hovered, disable all other drivers until this driver is no longer hovering
            FIRST = 1,
            // Allow for multiple hovered drivers at a time
            MULTIPLE = 2
        }

        [SerializeField]
        private InteractorDriverGroupStrategy _interactorDriverGroupStrategy =
            InteractorDriverGroupStrategy.PRIORITY;

        public bool IsRootInteractorDriver { get; set; } = true;

        private IInteractorDriver _currentDriver = null;
        private IInteractorDriver _selectingDriver = null;

        [SerializeField]
        private bool _selectionCanBeEmpty = true;

        protected virtual void Awake()
        {
            InteractorDrivers = _interactorDrivers.ConvertAll(mono => mono as IInteractorDriver);
        }

        protected virtual void Start()
        {
            foreach (IInteractorDriver interactorDriver in InteractorDrivers)
            {
                Assert.IsNotNull(interactorDriver);
            }

            foreach (IInteractorDriver interactorDriver in InteractorDrivers)
            {
                interactorDriver.IsRootInteractorDriver = false;
            }
        }

        protected virtual void Update()
        {
            if (!IsRootInteractorDriver) return;
            UpdateInteraction();
            UpdateHover();
            UpdateSelection(_selectionCanBeEmpty);
        }

        public void UpdateInteraction()
        {
            if (_selectingDriver != null)
            {
                _selectingDriver.UpdateInteraction();
                return;
            }

            switch (_interactorDriverGroupStrategy)
            {
                case InteractorDriverGroupStrategy.PRIORITY:
                    PriorityHoverStrategy();
                    break;

                case InteractorDriverGroupStrategy.FIRST:
                    FirstHoverStrategy();
                    break;

                case InteractorDriverGroupStrategy.MULTIPLE:
                    MultiHoverStrategy();
                    break;
            }
        }

        public void UpdateHover()
        {
            if (_selectingDriver != null || _currentDriver == null)
            {
                return;
            }

            _currentDriver.UpdateHover();
            switch (_interactorDriverGroupStrategy)
            {
                case InteractorDriverGroupStrategy.PRIORITY:
                case InteractorDriverGroupStrategy.FIRST:
                    if (_currentDriver != null)
                    {
                        _currentDriver.UpdateHover();
                    }
                    break;
                case InteractorDriverGroupStrategy.MULTIPLE:
                    foreach(IInteractorDriver driver in InteractorDrivers)
                    {
                        if (!driver.HasCandidate)
                        {
                            continue;
                        }
                        driver.UpdateHover();
                    }
                    break;
            }
        }

        public void UpdateSelection(bool selectionCanBeEmpty)
        {
            if (_selectingDriver != null)
            {
                _selectingDriver.UpdateSelection(selectionCanBeEmpty);
                if (_selectingDriver.IsSelecting)
                {
                    return;
                }

                _selectingDriver = null;
                UpdateInteraction();
                UpdateHover();
            }

            switch (_interactorDriverGroupStrategy)
            {
                case InteractorDriverGroupStrategy.PRIORITY:
                case InteractorDriverGroupStrategy.FIRST:
                    if (_currentDriver != null)
                    {
                        _currentDriver.UpdateSelection(selectionCanBeEmpty);
                        if (_currentDriver.IsSelecting)
                        {
                            SelectDriver(_currentDriver);
                        }
                    }

                    break;
                case InteractorDriverGroupStrategy.MULTIPLE:
                    foreach(IInteractorDriver driver in InteractorDrivers)
                    {
                        driver.UpdateSelection(selectionCanBeEmpty);
                        if (driver.IsSelecting)
                        {
                            _currentDriver = driver;
                            SelectDriver(_currentDriver);
                            return;
                        }
                    }

                    break;
            }
        }

        private void PriorityHoverStrategy()
        {
            _currentDriver = null;
            for (int i = 0; i < InteractorDrivers.Count; i++)
            {
                IInteractorDriver driver = InteractorDrivers[i];

                driver.Enable();
                driver.UpdateInteraction();

                // If we are in a hover state, we want to make the hover interactor visible
                if (driver.HasCandidate)
                {
                    _currentDriver = driver;
                    DisableAllDriversExcept(_currentDriver);
                    return;
                }
                else if (i != InteractorDrivers.Count - 1)
                {
                    // when we only allow only one hovering interactor,
                    // all but the least prioritized interactor
                    // should be hidden
                    driver.Disable();
                }
            }

            if (_currentDriver == null)
            {
                _currentDriver = InteractorDrivers[InteractorDrivers.Count - 1];
            }
        }

        private void FirstHoverStrategy()
        {
            if (_currentDriver != null)
            {
                _currentDriver.UpdateInteraction();
                if (_currentDriver.HasCandidate)
                {
                    return;
                }
            }

            _currentDriver = null;
            for (int i = 0; i < InteractorDrivers.Count; i++)
            {
                IInteractorDriver driver = InteractorDrivers[i];
                driver.Enable();
                driver.UpdateInteraction();

                // If we are in a hover state, we want to make the hover interactor visible
                if (driver.HasCandidate)
                {
                    _currentDriver = driver;
                    DisableAllDriversExcept(_currentDriver);
                    return;
                }
            }
        }

        private void MultiHoverStrategy()
        {
            _currentDriver = null;
            foreach (IInteractorDriver driver in InteractorDrivers)
            {
                driver.Enable();
                driver.UpdateInteraction();

                // If we are in a hover state, we want to make the hover interactor visible
                if (driver.HasCandidate)
                {
                    _currentDriver = driver;
                }
            }
        }

        private void SelectDriver(IInteractorDriver selectedDriver)
        {
            // Mark this as the selected driver
            _currentDriver = selectedDriver;
            _selectingDriver = selectedDriver;

            DisableAllDriversExcept(_selectingDriver);
        }

        private void DisableAllDriversExcept(IInteractorDriver enabledDriver)
        {
            foreach (IInteractorDriver driver in InteractorDrivers)
            {
                if (driver == enabledDriver) continue;
                driver.Disable();
            }
        }

        public bool IsSelectingInteractable => _selectingDriver != null && _selectingDriver.IsSelectingInteractable;
        public bool IsSelecting => _selectingDriver != null;
        public bool HasCandidate => _currentDriver != null && _currentDriver.HasCandidate;
        public bool ShouldSelect =>
            _currentDriver != null && _currentDriver.ShouldSelect;
        public bool IsHovering => _currentDriver != null && _currentDriver.IsHovering;

        public void Enable()
        {
            foreach (IInteractorDriver interactorDriver in InteractorDrivers)
            {
                interactorDriver.Enable();
            }
        }

        public void Disable()
        {
            foreach (IInteractorDriver interactorDriver in InteractorDrivers)
            {
                interactorDriver.Disable();
            }
        }

        public void AddInteractorDriver(IInteractorDriver interactorDriver)
        {
            InteractorDrivers.Add(interactorDriver);
            _interactorDrivers.Add(interactorDriver as MonoBehaviour);
            interactorDriver.IsRootInteractorDriver = false;
        }

        public void RemoveInteractorDriver(IInteractorDriver interactorDriver)
        {
            InteractorDrivers.Remove(interactorDriver);
            _interactorDrivers.Remove(interactorDriver as MonoBehaviour);
            interactorDriver.IsRootInteractorDriver = true;
        }

        #region Inject

        public void InjectAllInteractorDriverGroup(List<IInteractorDriver> interactorDrivers)
        {
            InjectInteractorDrivers(interactorDrivers);
        }

        public void InjectInteractorDrivers(List<IInteractorDriver> interactorDrivers)
        {
            InteractorDrivers = interactorDrivers;
            _interactorDrivers = interactorDrivers.ConvertAll(interactorDriver =>
                                                                interactorDriver as MonoBehaviour);
        }

        public void InjectOptionalInteractorDriverGroupStrategy(
            InteractorDriverGroupStrategy interactorDriverGroupStrategy)
        {
            _interactorDriverGroupStrategy = interactorDriverGroupStrategy;
        }

        #endregion
    }
}
