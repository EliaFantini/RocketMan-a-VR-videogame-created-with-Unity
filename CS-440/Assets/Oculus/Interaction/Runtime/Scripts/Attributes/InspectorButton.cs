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
using System.Reflection;
using System;

namespace Oculus.Interaction
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        private const float BUTTON_WIDTH = 80;

        public float ButtonWidth { get; set; } = BUTTON_WIDTH;

        public readonly string methodName;

        public InspectorButtonAttribute(string methodName)
        {
            this.methodName = methodName;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private MethodInfo _method = null;

        public override void OnGUI(Rect positionRect, UnityEditor.SerializedProperty prop, GUIContent label)
        {
            InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
            Rect rect = positionRect;
            if (GUI.Button(rect, label.text))
            {
                Type eventType = prop.serializedObject.targetObject.GetType();
                string eventName = inspectorButtonAttribute.methodName;
                if (_method == null)
                {
                    _method = eventType.GetMethod(eventName,
                        BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.Instance
                        | BindingFlags.Static);
                }
                _method?.Invoke(prop.serializedObject.targetObject, null);
            }
        }
    }
#endif
}
