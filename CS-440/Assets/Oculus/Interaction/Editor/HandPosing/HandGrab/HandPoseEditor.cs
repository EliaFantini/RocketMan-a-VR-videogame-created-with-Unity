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
using System;
using UnityEditor;
using UnityEngine;

namespace Oculus.Interaction.HandPosing.Editor
{
    [CustomPropertyDrawer(typeof(HandPose))]
    public class HandPoseEditor : PropertyDrawer
    {
        private bool _foldedFreedom = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel++;

            DrawEnumProperty<Handedness>(property, "Handedness:", "_handedness", false);
            DrawFingersFreedomMenu(property);

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private void DrawFingersFreedomMenu(SerializedProperty property)
        {
            _foldedFreedom = EditorGUILayout.Foldout(_foldedFreedom, "Fingers Freedom", true);
            if (_foldedFreedom)
            {
                SerializedProperty fingersFreedom = property.FindPropertyRelative("_fingersFreedom");
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < Constants.NUM_FINGERS; i++)
                {
                    SerializedProperty finger = fingersFreedom.GetArrayElementAtIndex(i);
                    HandFinger fingerID = (HandFinger)i;
                    JointFreedom current = (JointFreedom)finger.intValue;
                    JointFreedom selected = (JointFreedom)EditorGUILayout.EnumPopup($"{fingerID}: ", current);
                    finger.intValue = (int)selected;
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawEnumProperty<TEnum>(SerializedProperty parentProperty, string title, string fieldName, bool isFlags) where TEnum : Enum
        {
            SerializedProperty fieldProperty = parentProperty.FindPropertyRelative(fieldName);
            TEnum value = (TEnum)Enum.ToObject(typeof(TEnum), fieldProperty.intValue);
            Enum selectedValue = isFlags ?
                EditorGUILayout.EnumFlagsField(title, value)
                : EditorGUILayout.EnumPopup(title, value);
            fieldProperty.intValue = (int)Enum.ToObject(typeof(TEnum), selectedValue);
        }
    }
}
