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
using UnityEditor;
using UnityEngine;

namespace Oculus.Interaction.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HandWristOffset))]
    public class HandWristOffsetEditor : UnityEditor.Editor
    {
        private Transform _gripPoint;
        private HandWristOffset _wristOffset;

        private SerializedProperty _offsetPositionProperty;
        private SerializedProperty _rotationProperty;
        private SerializedProperty _handProperty;

        private Pose _cachedPose;

        private const float THICKNESS = 2f;

        private void Awake()
        {
            _wristOffset = target as HandWristOffset;

            _offsetPositionProperty = serializedObject.FindProperty("_offset");
            _rotationProperty = serializedObject.FindProperty("_rotation");
            _handProperty = serializedObject.FindProperty("_hand");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Transform point = EditorGUILayout.ObjectField("Optional Calculate Offset To", _gripPoint, typeof(Transform), true) as Transform;
            if (point != _gripPoint)
            {
                _gripPoint = point;
                if (_gripPoint != null)
                {
                    Pose offset = _wristOffset.transform.RelativeOffset(_gripPoint);
                    _rotationProperty.quaternionValue = FromOVRHandDataSource.WristFixupRotation * offset.rotation;
                    _offsetPositionProperty.vector3Value = FromOVRHandDataSource.WristFixupRotation * offset.position;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void OnSceneGUI()
        {
            GetEditorOffset(ref _cachedPose);
            Pose wristPose = _wristOffset.transform.GetPose();
            wristPose.rotation = wristPose.rotation * FromOVRHandDataSource.WristFixupRotation;
            _cachedPose.Postmultiply(wristPose);
            DrawAxis(_cachedPose);
        }

        private void DrawAxis(in Pose pose)
        {
            float scale = HandleUtility.GetHandleSize(pose.position);

#if UNITY_2020_2_OR_NEWER
            Handles.color = Color.red;
            Handles.DrawLine(pose.position, pose.position + pose.right * scale, THICKNESS);
            Handles.color = Color.green;
            Handles.DrawLine(pose.position, pose.position + pose.up * scale, THICKNESS);
            Handles.color = Color.blue;
            Handles.DrawLine(pose.position, pose.position + pose.forward * scale, THICKNESS);
#else
            Handles.color = Color.red;
            Handles.DrawLine(pose.position, pose.position + pose.right * scale);
            Handles.color = Color.green;
            Handles.DrawLine(pose.position, pose.position + pose.up * scale);
            Handles.color = Color.blue;
            Handles.DrawLine(pose.position, pose.position + pose.forward * scale);
#endif
        }

        private void GetEditorOffset(ref Pose pose)
        {
            pose.position = _offsetPositionProperty.vector3Value;
            pose.rotation = _rotationProperty.quaternionValue;

            IHand hand = _handProperty?.objectReferenceValue as IHand;
            if (hand != null)
            {
                if (hand.Handedness == Handedness.Left)
                {
                    pose.position = -pose.position;
                    pose.rotation = Quaternion.Inverse(pose.rotation);
                }
            }
        }
    }
}
