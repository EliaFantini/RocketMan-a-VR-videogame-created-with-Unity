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

namespace Oculus.Interaction
{
    /// <summary>
    /// A interface for a registry that houses a set of concrete Interactables.
    /// </summary>
    public interface IInteractableRegistry<TInteractor, TInteractable>
                                          where TInteractable : IInteractable<TInteractor>
    {
        void Register(TInteractable interactable);
        void Unregister(TInteractable interactable);
        IEnumerable<TInteractable> List();
        IEnumerable<TInteractable> List(TInteractor interactor);
    }
}
