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
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Oculus.Interaction.InterfaceSupport
{
    /// <summary>
    /// This property drawer is the meat of the interface support implementation. When
    /// the value of field with this attribute is modified, the new value is tested
    /// against the interface expected. If the component matches, the new value is
    /// accepted. Otherwise, the old value is maintained.
    /// </summary>
    [CustomPropertyDrawer(typeof(InterfaceAttribute))]
    public class InterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "InterfaceType Attribute can only be used with MonoBehaviour Components.");
                return;
            }

            Type[] attTypes = GetInterfaceTypes(property);

            // Pick a specific component
            MonoBehaviour oldComponent = property.objectReferenceValue as MonoBehaviour;
            string oldComponentName = "";

            GameObject temporaryGameObject = null;

            string attTypesName = GetTypesName(attTypes);
            if (Event.current.type == EventType.Repaint)
            {
                if (oldComponent == null)
                {
                    temporaryGameObject = new GameObject("None (" + attTypesName + ")");
                    oldComponent = temporaryGameObject.AddComponent<InterfaceMono>();
                }
                else
                {
                    oldComponentName = oldComponent.name;
                    oldComponent.name = oldComponentName + " (" + attTypesName + ")";
                }
            }

            MonoBehaviour currentComponent = EditorGUI.ObjectField(position, label, oldComponent, typeof(MonoBehaviour), true) as MonoBehaviour;

            if (Event.current.type == EventType.Repaint)
            {
                if (temporaryGameObject != null)
                    GameObject.DestroyImmediate(temporaryGameObject);
                else
                    oldComponent.name = oldComponentName;
            }

            // If a component is assigned, make sure it is the interface we are looking for.
            if (currentComponent != null)
            {
                // Make sure component is of the right interface
                if(!IsAssignableFromTypes(currentComponent.GetType(), attTypes))
                    // Component failed. Check game object.
                    foreach (Type attType in attTypes)
                    {
                        currentComponent = currentComponent.gameObject.GetComponent(attType) as MonoBehaviour;
                        if (currentComponent == null)
                        {
                            break;
                        }
                    }

                // Item failed test. Do not override old component
                if (currentComponent == null)
                {
                    if (oldComponent != null && !IsAssignableFromTypes(oldComponent.GetType(), attTypes))
                    {
                        temporaryGameObject = new GameObject("None (" + attTypesName + ")");
                        MonoBehaviour temporaryComponent = temporaryGameObject.AddComponent<InterfaceMono>();
                        currentComponent = EditorGUI.ObjectField(position, label, temporaryComponent, typeof(MonoBehaviour), true) as MonoBehaviour;
                        GameObject.DestroyImmediate(temporaryGameObject);
                    }
                }
            }

            property.objectReferenceValue = currentComponent;
            property.serializedObject.ApplyModifiedProperties();
        }

        private bool IsAssignableFromTypes(Type source, Type[] targets)
        {
            foreach (Type t in targets)
            {
                if (!t.IsAssignableFrom(source))
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetTypesName(Type[] attTypes)
        {
            if (attTypes.Length == 1)
            {
                return GetTypeName(attTypes[0]);
            }

            string typesString = "";
            for (int i = 0; i < attTypes.Length; i++)
            {
                if (i > 0)
                {
                    typesString += ", ";
                }

                typesString += GetTypeName(attTypes[i]);
            }

            return typesString;
        }

        private static string GetTypeName(Type attType)
        {
            if (!attType.IsGenericType)
            {
                return attType.Name;
            }

            var genericTypeNames = attType.GenericTypeArguments.Select(GetTypeName);
            return $"{attType.Name}<{string.Join(", ", genericTypeNames)}>";
        }

        private Type[] GetInterfaceTypes(SerializedProperty property)
        {
            InterfaceAttribute att = (InterfaceAttribute)attribute;
            Type[] t = att.Types;
            if (!String.IsNullOrEmpty(att.TypeFromFieldName))
            {
                var thisType = property.serializedObject.targetObject.GetType();
                while (thisType != null)
                {
                    var referredFieldInfo = thisType.GetField(att.TypeFromFieldName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (referredFieldInfo != null)
                    {
                        t = new Type[1] { referredFieldInfo.FieldType };
                        break;
                    }

                    thisType = thisType.BaseType;
                }
            }

            return t ?? singleMonoBehaviourType;
        }

        private static readonly Type[] singleMonoBehaviourType = new Type[1] {typeof(MonoBehaviour)};
    }


    public sealed class InterfaceMono : MonoBehaviour { }
}
