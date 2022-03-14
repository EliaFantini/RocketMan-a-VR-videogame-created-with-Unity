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

namespace Oculus.Interaction
{
    /// <summary>
    /// IInteractionDriver drives the update loop of an Interaction.
    /// </summary>
    public interface IInteractorDriver
    {
        // This flag determines whether this driver should controls its own update loop
        bool IsRootInteractorDriver { get; set; }

        bool HasCandidate { get; }
        bool ShouldSelect { get; }

        bool IsHovering { get; }
        bool IsSelecting { get; }
        bool IsSelectingInteractable { get; }

        void UpdateInteraction();
        void UpdateHover();
        void UpdateSelection(bool selectionCanBeEmpty);
        void Enable();
        void Disable();
    }
}
