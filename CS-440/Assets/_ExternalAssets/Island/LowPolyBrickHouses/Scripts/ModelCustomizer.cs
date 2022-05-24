using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCustomizer : MonoBehaviour {
#if UNITY_EDITOR

    private GameObject[] parts;
    [SerializeField]
    private bool[] bools;
    [SerializeField]

    void Reset()
    {
        //create arrays
        int n = transform.childCount;
        parts = new GameObject[n];
        bools = new bool[n];
        //fill arrays
        for (int i = 0; i < n; i++)
        {
            parts[i] = transform.GetChild(i).gameObject;
            bools[i] = parts[i].activeSelf;
        }
        //refresh visible
        RefreshVisibility();
    }

    public void RefreshVisibility()
    {
        int n = parts.Length;
        for (int i = 0; i < n; i++)
        {
            parts[i].SetActive(bools[i]);
        }
    }

    public string[] GetNames()
    {
        int n = parts.Length;
        string[] strings = new string[n];
        for (int i = 0; i < n; i++)
        {
            strings[i] = parts[i].name;
        }
        return strings;
    }

    public bool[] GetBools()
    {
        return bools;
    }

    public void SetBool(int i, bool b){
        bools[i] = b;
        RefreshVisibility();
    }

    public void ApplyMaterial(Material material)
    {
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>(true))
        {
            Material[] matArray = meshRenderer.sharedMaterials;

            for (int i = 0; i < matArray.Length; i++)
            {
                matArray[i] = material;
            }

            meshRenderer.sharedMaterials = matArray;
        }
    }

    public void DoReset()
    {
        Reset();
    }
#endif
}
