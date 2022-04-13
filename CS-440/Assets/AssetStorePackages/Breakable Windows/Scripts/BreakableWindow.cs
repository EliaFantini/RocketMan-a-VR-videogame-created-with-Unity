using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Breakable Windows/Breakable Window")]
[RequireComponent(typeof(AudioSource))]
public class BreakableWindow : MonoBehaviour {

    
    [Tooltip("Layer should be TransparentFX or your own layer for breakable windows.")]
    public LayerMask layer;
    [Range(2,25)]
    public int partsX = 5;
    [Range(2, 25)]
    public int partsY = 5;

    [Space]
    public bool preCalculate = true;
    public bool addTorques = true;
    public bool hideSplintersInHierarchy = true;
    public bool useCollision = true;
    [Tooltip("Use 0 for breaking immediately if a collision is detected.")]
    public float health = 0;

    [Space]
    [Space]
    [Tooltip("Seconds after window is broken that physics have to be destroyed.")]
    public float destroyPhysicsTime = 5;
    public bool destroyColliderWithPhysics = true;

    [Space]
    [Tooltip("Seconds after window is broken that splinters have to be destroyed.")]
    public float destroySplintersTime = 0;

    [Space]
    public AudioClip breakingSound;


    [HideInInspector]
    public bool isBroken = false;
    [HideInInspector]
    public List<GameObject> splinters;
    private Vector3[] vertices;
    private Vector3[] normals;
    
    private bool allreadyCalculated = false;
    private GameObject splinterParent;
    int[] tris;

    void Start()
    {
        if (preCalculate == true && allreadyCalculated == false)
        {
            bakeVertices();
            bakeSplinters();
            allreadyCalculated = true;
        }

        if (transform.rotation.eulerAngles.x != 0 || transform.rotation.eulerAngles.z != 0)
            Debug.LogWarning("Warning: Window must not be rotated around x and z!");
    }

    private void bakeVertices(bool trianglesToo = false)
    {
        vertices = new Vector3[(partsX + 1) * (partsY + 1)];
        normals = new Vector3[(partsX + 1) * (partsY + 1)];
        

        for (int y = 0; y < partsY + 1; y++)
        {
            for (int x = 0; x < partsX + 1; x++)
            {
                float randomX = Random.value > 0.5f ? Random.value / partsX : -Random.value / partsX;
                float randomY = Random.value > 0.5f ? Random.value / partsY : -Random.value / partsY;
                vertices[y * (partsX + 1) + x] = new Vector3((float)x / (float)partsX - 0.5f + randomX, (float)y / (float)partsY - 0.5f + randomY, 0);
                normals[y * (partsX + 1) + x] = -Vector3.forward;
            }
        }

        if (trianglesToo == true)
        {
            tris = new int[partsX * partsY * 6];
            int pos = 0;
            for (int y = 0; y < partsY; y++)
            {
                for (int x = 0; x < partsX; x++)
                {
                    tris[pos + 0] = y * (partsX + 1) + x;
                    tris[pos + 1] = y * (partsX + 1) + x + 1;
                    tris[pos + 2] = (y + 1) * (partsX + 1) + x;

                    pos += 3;

                    tris[pos + 0] = (y + 1) * (partsX + 1) + x;
                    tris[pos + 1] = y * (partsX + 1) + x + 1;
                    tris[pos + 2] = (y + 1) * (partsX + 1) + x + 1;

                    pos += 3;
                }
            }
        }
    }

    private void generateSingleSplinter(int[] tris, Transform parent)
    {
        Vector3[] v = new Vector3[3];
        Vector3[] n = new Vector3[3];
        int[] t = new int[6];

        v[0] = Vector3.zero;
        v[1] = vertices[tris[1]] - vertices[tris[0]];
        v[2] = vertices[tris[2]] - vertices[tris[0]];

        n[0] = normals[t[0]];
        n[1] = normals[t[1]];
        n[2] = normals[t[2]];

        t[0] = 0;
        t[1] = 1;
        t[2] = 2;
        t[3] = 2;
        t[4] = 1;
        t[5] = 0;

        Mesh m = new Mesh();
        m.vertices = v;
        m.normals = n;
        m.triangles = t;

        GameObject obj = new GameObject();
        obj.transform.position = new Vector3(vertices[tris[0]].x * transform.localScale.x + transform.position.x, vertices[tris[0]].y * transform.localScale.y + transform.position.y, transform.position.z);
        obj.transform.RotateAround(transform.position, transform.up, transform.rotation.eulerAngles.y);
        obj.transform.localScale = transform.localScale;
        obj.transform.rotation = transform.rotation;
        obj.layer = layer.value;
        obj.name = "Glass Splinter";
        if (destroySplintersTime > 0)
            Destroy(obj, destroySplintersTime);


        if (preCalculate == true)
        {
            obj.transform.parent = parent;
        }

        if (hideSplintersInHierarchy) obj.hideFlags = HideFlags.HideInHierarchy;
        splinters.Add(obj);

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        mf.mesh = m;
        
        MeshCollider col = obj.AddComponent<MeshCollider>();
        col.inflateMesh = true;
        col.convex = true;
        if (destroyPhysicsTime > 0 && destroyColliderWithPhysics) Destroy(col, destroyPhysicsTime);
        
        Rigidbody rigid = obj.AddComponent<Rigidbody>();
        rigid.centerOfMass = (v[0] + v[1] + v[2]) / 3f;
        if (addTorques && preCalculate == false) rigid.AddTorque(new Vector3(Random.value > 0.5f ? Random.value * 50 : -Random.value * 50, Random.value > 0.5f ? Random.value * 50 : -Random.value * 50, Random.value > 0.5f ? Random.value * 50 : -Random.value * 50));
        if (destroyPhysicsTime > 0) Destroy(rigid, destroyPhysicsTime);

        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        mr.materials = GetComponent<Renderer>().materials;
    }

    private void bakeSplinters()
    {
        int[] t = new int[3];
        splinters = new List<GameObject>();
        splinterParent = new GameObject("Splinters");
        splinterParent.transform.parent = transform;

        if (preCalculate) splinterParent.SetActive(false);

        for (int y = 0; y < partsY; y++)
        {
            for (int x = 0; x < partsX; x++)
            {
                t[0] = y * (partsX + 1) + x;
                t[1] = y * (partsX + 1) + x + 1;
                t[2] = (y + 1) * (partsX + 1) + x;

                generateSingleSplinter(t, splinterParent.transform);

                t[0] = (y + 1) * (partsX + 1) + x;
                t[1] = y * (partsX + 1) + x + 1;
                t[2] = (y + 1) * (partsX + 1) + x + 1;

                generateSingleSplinter(t, splinterParent.transform);
            }
        }
    }

    /// <summary>
    /// Breaks the window and returns an array of all splinter gameobjects.
    /// </summary>
    /// <returns>Returns an array of all splinter gameobjects.</returns>
    public GameObject[] breakWindow()
    {
        if (isBroken == false)
        {
            if (allreadyCalculated == true)
            {
                splinterParent.SetActive(true);
                if (addTorques)
                {
                    for (int i = 0; i < splinters.Count; i++)
                    {
                        splinters[i].GetComponent<Rigidbody>().AddTorque(new Vector3(Random.value > 0.5f ? Random.value * 50 : -Random.value * 50, Random.value > 0.5f ? Random.value * 50 : -Random.value * 50, Random.value > 0.5f ? Random.value * 50 : -Random.value * 50));
                    }
                }
            }
            else
            {
                bakeVertices();
                bakeSplinters();
            }

            Physics.IgnoreLayerCollision(layer.value, layer.value, true);
            Destroy(GetComponent<Collider>());
            Destroy(GetComponent<MeshRenderer>());
            Destroy(GetComponent<MeshFilter>());

            isBroken = true;            
        }

        if (breakingSound != null)
        {
            GetComponent<AudioSource>().clip = breakingSound;
            GetComponent<AudioSource>().Play();
        }

        return splinters.ToArray();
    }


    void OnCollisionEnter(Collision col)
    {
        if (useCollision == true)
        {
            if (health > 0)
            {
                health -= col.impulse.magnitude;
                if (health < 0)
                {
                    health = 0;
                    breakWindow();
                }
            }
            else breakWindow();
        }        
    }
}
