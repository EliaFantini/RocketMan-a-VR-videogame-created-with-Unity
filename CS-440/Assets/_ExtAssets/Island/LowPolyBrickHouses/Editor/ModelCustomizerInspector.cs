using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModelCustomizer))]
[CanEditMultipleObjects]
public class ModelCustomizerInspector : Editor {

    private ModelCustomizer customizer;
    private bool[] bools;
    private string[] names;
    private bool start = true;
    private Material material = null;

    public override void OnInspectorGUI()
    {
        if (targets.Length > 1)
        {
            material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);

            if (GUILayout.Button("Apply material to all children") && material != null)
            {
                Undo.RecordObjects(targets.SelectMany(t => ((ModelCustomizer)t).gameObject.GetComponentsInChildren<MeshRenderer>()).ToArray(), "Apply Model Customizer materials");
                foreach (UnityEngine.Object obj in targets)
                {
                    ModelCustomizer modelCustomizer = (ModelCustomizer)obj;
                    modelCustomizer.ApplyMaterial(material);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Part customization is not allowed for multiple objects.", MessageType.Info);
        }
        else
        {
            customizer = target as ModelCustomizer;

            material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);

            if (GUILayout.Button("Apply material to all children") && material != null)
            {
                customizer.ApplyMaterial(material);
            }

            EditorGUILayout.Space();

            if (start)
            {
                customizer.DoReset();
                start = false;
            }
            bools = customizer.GetBools();
            names = customizer.GetNames();
            for (int i = 0; i < names.Length; i++)
            {
                //draw and handle bools
                EditorGUI.BeginChangeCheck();
                bool b = EditorGUILayout.Toggle(names[i], bools[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(customizer, "toggled model part");
                    EditorUtility.SetDirty(customizer);
                    customizer.SetBool(i, b);
                }
            }
        }
    }
}
