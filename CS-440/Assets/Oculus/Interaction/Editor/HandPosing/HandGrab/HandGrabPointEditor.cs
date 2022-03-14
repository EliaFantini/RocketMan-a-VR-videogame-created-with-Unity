/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using Oculus.Interaction.Input;
using Oculus.Interaction.HandPosing.Visuals;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Oculus.Interaction.HandPosing.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HandGrabPoint))]
    public class HandGrabPointEditor : UnityEditor.Editor
    {
        private HandGrabPoint _handGrabPoint;

        private HandGhostProvider _ghostVisualsProvider;
        private HandGhost _handGhost;
        private HandPuppet _ghostPuppet;
        private Handedness _lastHandedness;

        private HandPose _poseTransfer;
        private bool _editingFingers;

        private SerializedProperty _handPoseProperty;

        private const float GIZMO_SCALE = 0.005f;
        private static readonly Color FLEX_COLOR = new Color(0f, 1f, 1f, 0.5f);
        private static readonly Color SPREAD_COLOR = new Color(0.5f, 0.3f, 1f, 0.5f);

        private void Awake()
        {
            _handGrabPoint = target as HandGrabPoint;
            _handPoseProperty = serializedObject.FindProperty("_handPose");
            AssignMissingGhostProvider();
        }

        private void OnDestroy()
        {
            DestroyGhost();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_handGrabPoint.HandPose != null
                && _handPoseProperty != null)
            {
                EditorGUILayout.PropertyField(_handPoseProperty);
                EditorGUILayout.Space();
                DrawGhostMenu(_handGrabPoint.HandPose, false);
            }
            else if (_handGhost != null)
            {
                DestroyGhost();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGhostMenu(HandPose handPose, bool forceCreate)
        {
            GUIStyle boldStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            EditorGUILayout.LabelField("Interactive Edition", boldStyle);

            HandGhostProvider provider = EditorGUILayout.ObjectField("Ghost Provider", _ghostVisualsProvider, typeof(HandGhostProvider), false) as HandGhostProvider;
            if (forceCreate
                || provider != _ghostVisualsProvider
                || _handGhost == null
                || _lastHandedness != handPose.Handedness)
            {
                RegenerateGhost(provider);
            }
            _lastHandedness = handPose.Handedness;

            if (_handGrabPoint.SnapSurface == null)
            {
                _editingFingers = true;
            }
            else if (GUILayout.Button(_editingFingers ? "Follow Surface" : "Edit fingers"))
            {
                _editingFingers = !_editingFingers;
            }
        }

        public void OnSceneGUI()
        {
            SceneView view = SceneView.currentDrawingSceneView;
            if (view == null || _handGhost == null)
            {
                return;
            }

            if (_editingFingers)
            {
                GhostEditFingers();
                if (AnyPuppetBoneChanged())
                {
                    TransferGhostBoneRotations();
                }
            }
            else
            {
                GhostFollowSurface();
            }
        }


        private void EditSnapPoint(Transform snapPoint)
        {
            Vector3 pos = snapPoint.position;
            Quaternion rot = snapPoint.rotation;
            Handles.TransformHandle(ref pos, ref rot);

            if (pos != snapPoint.position)
            {
                Undo.RecordObject(snapPoint.transform, "SnapPoint Position");
                snapPoint.position = pos;
            }

            if (rot != snapPoint.rotation)
            {
                Undo.RecordObject(snapPoint.transform, "SnapPoint Rotation");
                snapPoint.rotation = rot;
            }
        }

        #region generation
        /// <summary>
        /// Generates a new HandGrabPointData that mirrors the provided one. Left hand becomes right hand and vice-versa.
        /// The mirror axis is defined by the surface of the snap point, if any, if none a best-guess is provided
        /// but note that it can then moved manually in the editor.
        /// </summary>
        /// <param name="originalPoint">The point to mirror</param>
        /// <param name="originalPoint">The target HandGrabPoint to set as mirrored of the originalPoint</param>
        public static void MirrorHandGrabPoint(HandGrabPoint originalPoint, HandGrabPoint mirrorPoint)
        {
            HandPose handPose = originalPoint.HandPose;

            Handedness oppositeHandedness = handPose.Handedness == Handedness.Left ? Handedness.Right : Handedness.Left;

            HandGrabPointData mirrorData = originalPoint.SaveData();
            mirrorData.handPose.Handedness = oppositeHandedness;

            if (originalPoint.SnapSurface != null)
            {
                mirrorData.gripPose = originalPoint.SnapSurface.MirrorPose(mirrorData.gripPose);
            }
            else
            {
                mirrorData.gripPose = mirrorData.gripPose.MirrorPoseRotation(Vector3.forward, Vector3.up);
                Vector3 translation = Vector3.Project(mirrorData.gripPose.position, Vector3.right);
                mirrorData.gripPose.position = mirrorData.gripPose.position - 2f * translation;
            }

            mirrorPoint.LoadData(mirrorData, originalPoint.RelativeTo);
            if (originalPoint.SnapSurface != null)
            {
                SnapSurfaces.ISnapSurface mirroredSurface = originalPoint.SnapSurface.CreateMirroredSurface(mirrorPoint.gameObject);
                mirrorPoint.InjectOptionalSurface(mirroredSurface);
            }
        }

        public static void CloneHandGrabPoint(HandGrabPoint originalPoint, HandGrabPoint targetPoint)
        {
            HandGrabPointData mirrorData = originalPoint.SaveData();
            targetPoint.LoadData(mirrorData, originalPoint.RelativeTo);
            if (originalPoint.SnapSurface != null)
            {
                SnapSurfaces.ISnapSurface mirroredSurface = originalPoint.SnapSurface.CreateDuplicatedSurface(targetPoint.gameObject);
                targetPoint.InjectOptionalSurface(mirroredSurface);
            }
        }
        #endregion

        #region ghost

        private void AssignMissingGhostProvider()
        {
            if (_ghostVisualsProvider != null)
            {
                return;
            }

            HandGhostProvider[] providers = Resources.FindObjectsOfTypeAll<HandGhostProvider>();
            if (providers != null && providers.Length > 0)
            {
                _ghostVisualsProvider = providers[0];
            }
        }

        private void RegenerateGhost(HandGhostProvider provider)
        {
            _ghostVisualsProvider = provider;
            DestroyGhost();
            CreateGhost();
        }

        private void CreateGhost()
        {
            if (_ghostVisualsProvider == null)
            {
                return;
            }

            HandGhost ghostPrototype = _ghostVisualsProvider.GetHand(_handGrabPoint.HandPose.Handedness);
            _handGhost = GameObject.Instantiate(ghostPrototype, _handGrabPoint.transform);
            _handGhost.gameObject.hideFlags = HideFlags.HideAndDontSave;
            _handGhost.SetPose(_handGrabPoint);
            _ghostPuppet = _handGhost.GetComponent<HandPuppet>();
        }

        private void DestroyGhost()
        {
            if (_handGhost != null)
            {
                GameObject.DestroyImmediate(_handGhost.gameObject);
            }
        }

        private void GhostFollowSurface()
        {
            if (_handGhost == null)
            {
                return;
            }

            Pose ghostTargetPose = _handGrabPoint.RelativeGrip;

            if (_handGrabPoint.SnapSurface != null)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                Pose recorderPose = _handGrabPoint.transform.GetPose();
                if (_handGrabPoint.SnapSurface.CalculateBestPoseAtSurface(ray, recorderPose, out Pose target))
                {
                    ghostTargetPose.position = _handGrabPoint.RelativeTo.InverseTransformPoint(target.position);
                    ghostTargetPose.rotation = Quaternion.Inverse(_handGrabPoint.RelativeTo.rotation) * target.rotation;
                }
            }

            _handGhost.SetGripPose(ghostTargetPose, _handGrabPoint.RelativeTo);
        }

        private void GhostEditFingers()
        {
            HandPuppet puppet = _handGhost.GetComponent<HandPuppet>();
            if (puppet != null && puppet.JointMaps != null)
            {
                DrawBonesRotator(puppet.JointMaps);
            }
        }

        private void TransferGhostBoneRotations()
        {
            _poseTransfer = _handGrabPoint.HandPose;
            _ghostPuppet.CopyCachedJoints(ref _poseTransfer);
            _handGrabPoint.HandPose.CopyFrom(_poseTransfer);
        }

        private void DrawBonesRotator(List<HandJointMap> bones)
        {
            foreach (HandJointMap bone in bones)
            {
                int metadataIndex = FingersMetadata.HandJointIdToIndex(bone.id);
                HandFinger finger = FingersMetadata.HAND_FINGER_ID[metadataIndex];

                if (_handGrabPoint.HandPose.FingersFreedom[(int)finger] == JointFreedom.Free)
                {
                    continue;
                }

                Handles.color = FLEX_COLOR;
                Quaternion rotation = Handles.Disc(bone.transform.rotation, bone.transform.position,
                    bone.transform.forward, GIZMO_SCALE, false, 0);

                if (FingersMetadata.HAND_JOINT_CAN_SPREAD[metadataIndex])
                {
                    Handles.color = SPREAD_COLOR;
                    rotation = Handles.Disc(rotation, bone.transform.position,
                        bone.transform.up, GIZMO_SCALE, false, 0);
                }

                if (bone.transform.rotation != rotation)
                {
                    Undo.RecordObject(bone.transform, "Bone Rotation");
                    bone.transform.rotation = rotation;
                }

            }
        }

        /// <summary>
        /// Detects if we have moved the transforms of the fingers so the data can be kept up to date.
        /// To be used in Edit-mode.
        /// </summary>
        /// <returns>True if any of the fingers has moved from the previous frame.</returns>
        private bool AnyPuppetBoneChanged()
        {
            bool hasChanged = false;
            foreach (HandJointMap bone in _ghostPuppet.JointMaps)
            {
                if (bone.transform.hasChanged)
                {
                    bone.transform.hasChanged = false;
                    hasChanged = true;
                }
            }
            return hasChanged;
        }

        #endregion

    }
}
