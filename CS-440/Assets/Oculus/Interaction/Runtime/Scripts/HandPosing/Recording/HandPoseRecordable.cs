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
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.HandPosing.Recording
{
    public class HandPoseRecordable : MonoBehaviour
    {
        /// <summary>
        /// List of valid HandGrabInteractables in which the object can be held.
        /// </summary>
        [SerializeField]
        private List<HandGrabInteractable> _interactables = new List<HandGrabInteractable>();

#if UNITY_EDITOR
        /// <summary>
        /// Creates an Inspector button to remove the HandGrabInteractable collection, destroying its
        /// associated gameObjects.
        /// </summary>
        [InspectorButton("RemoveInteractables")]
        [SerializeField]
        private string _clearInteractables;
        /// <summary>
        /// Button to forcefully refresh the HandGrabInteractables collection during edit mode
        /// </summary>
        [InspectorButton("RefreshInteractables")]
        [SerializeField]
        private string _refreshInteractables;

        [Space(20f)]
        /// <summary>
        /// This ScriptableObject stores the HandGrabInteractables generated at Play-Mode so it survives
        /// the Play-Edit cycle.
        /// Create a collection and assign it even in Play Mode and make sure to store here the
        /// interactables, then restore it in Edit-Mode to be serialized.
        /// </summary>
        [SerializeField, Optional]
        [Tooltip("Collection for storing generated HandGrabInteractables during Play-Mode, so they can be restored in Edit-Mode")]
        private HandGrabInteractableDataCollection _posesCollection = null;
        /// <summary>
        /// Creates an Inspector button to store the current HandGrabInteractables to the posesCollection.
        /// Use it during Play-Mode.
        /// </summary>
        [InspectorButton("SaveToAsset")]
        [SerializeField]
        private string _storePoses;
        /// <summary>
        /// Creates an Inspector button that restores the saved HandGrabInteractables inn posesCollection.
        /// Use it in Edit-Mode.
        /// </summary>
        [InspectorButton("LoadFromAsset")]
        [SerializeField]
        private string _loadPoses;
#endif

        /// <summary>
        /// Creates a new HandGrabInteractable at the exact pose of a given hand.
        /// Mostly used with Hand-Tracking at Play-Mode
        /// </summary>
        /// <param name="rawPose">The user controlled hand pose.</param>
        /// <param name="snapPoint">The user controlled hand pose.</param>
        /// <returns>The generated HandGrabPoint.</returns>
        public HandGrabPoint AddHandGrabPoint(HandPose rawPose, Pose snapPoint)
        {
            HandGrabInteractable interactable = GenerateHandGrabInteractable();
            HandGrabPointData pointData = new HandGrabPointData()
            {
                handPose = rawPose,
                scale = 1f,
                gripPose = snapPoint,
            };
            return interactable.LoadPoint(pointData);
        }

        /// <summary>
        /// Creates a default HandGrabInteractable for this snap interactable.
        /// </summary>
        /// <returns>An non-populated HandGrabInteractable.</returns>
        private HandGrabInteractable GenerateHandGrabInteractable()
        {
            HandGrabInteractable record = HandGrabInteractable.Create(this.transform);
            _interactables.Add(record);
            return record;
        }

        public float DistanceToObject(Vector3 point)
        {
            float minDistance = float.PositiveInfinity;
            foreach (Renderer rend in this.GetComponentsInChildren<Renderer>())
            {
                if (rend.bounds.Contains(point))
                {
                    return 0f;
                }
                else
                {
                    Vector3 surfacePoint = rend.bounds.ClosestPoint(point);
                    float distance = Vector3.Distance(surfacePoint, point);
                    if (distance <= minDistance)
                    {
                        minDistance = distance;
                    }
                }
            }
            return minDistance;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Creates a new HandGrabInteractable from the stored data.
        /// Mostly used to restore a HandGrabInteractable that was stored during Play-Mode.
        /// </summary>
        /// <param name="data">The data of the HandGrabInteractable.</param>
        /// <returns>The generated HandGrabInteractable.</returns>
        private HandGrabInteractable LoadHandGrabInteractable(HandGrabInteractableData data)
        {
            HandGrabInteractable interactable = GenerateHandGrabInteractable();
            interactable.LoadData(data);
            return interactable;
        }

        /// <summary>
        /// Load the HandGrabInteractable from a Collection.
        ///
        /// This method is called from a button in the Inspector and will load the posesCollection.
        /// </summary>
        private void LoadFromAsset()
        {
            if (_posesCollection != null)
            {
                foreach (HandGrabInteractableData handPose in _posesCollection.InteractablesData)
                {
                    LoadHandGrabInteractable(handPose);
                }
            }
        }

        /// <summary>
        /// Stores the interactables to a SerializedObject (the empty object must be
        /// provided in the inspector or one will be auto-generated). First it translates the HandGrabInteractable to a serialized
        /// form HandGrabInteractableData).
        ///
        /// This method is called from a button in the Inspector.
        /// </summary>
        private void SaveToAsset()
        {
            List<HandGrabInteractableData> savedPoses = new List<HandGrabInteractableData>();
            foreach (HandGrabInteractable snap in this.GetComponentsInChildren<HandGrabInteractable>(false))
            {
                savedPoses.Add(snap.SaveData());
            }
            if (_posesCollection == null)
            {
                GenerateCollectionAsset();
            }
            _posesCollection?.StoreInteractables(savedPoses);
        }

        private void GenerateCollectionAsset()
        {
            _posesCollection = ScriptableObject.CreateInstance<HandGrabInteractableDataCollection>();
            string parentDir = Path.Combine("Assets", "HandGrabInteractableDataCollection");
            if (!Directory.Exists(parentDir))
            {
                Directory.CreateDirectory(parentDir);
            }
            UnityEditor.AssetDatabase.CreateAsset(_posesCollection, Path.Combine(parentDir, $"{this.name}_HandGrabCollection.asset"));
            UnityEditor.AssetDatabase.SaveAssets();
        }

        private void RemoveInteractables()
        {
            foreach (HandGrabInteractable interactable in _interactables)
            {
                UnityEditor.Undo.DestroyObjectImmediate(interactable.gameObject);
            }
            _interactables.Clear();
        }

        /// <summary>
        /// Refreshes a list of all the Active HandGrabInteractable nested under this Recordable.
        /// </summary>
        private void RefreshInteractables()
        {
            List<HandGrabInteractable> interactables = new List<HandGrabInteractable>(this.GetComponentsInChildren<HandGrabInteractable>(false));
            _interactables = interactables;
        }
#endif
    }
}
