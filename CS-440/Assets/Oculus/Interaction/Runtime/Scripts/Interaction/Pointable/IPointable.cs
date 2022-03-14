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

namespace Oculus.Interaction
{
    public enum PointerEvent
    {
        Hover,
        Unhover,
        Select,
        Unselect,
        Move
    }

    public struct PointerArgs
    {
        public int Identifier { get; }
        public PointerEvent PointerEvent { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public PointerArgs(int identifier, PointerEvent pointerEvent, Vector3 position, Quaternion rotation)
        {
            this.Identifier = identifier;
            this.PointerEvent = pointerEvent;
            this.Position = position;
            this.Rotation = rotation;
        }
    }

    public interface IPointable
    {
        event Action<PointerArgs> OnPointerEvent;
    }
}
