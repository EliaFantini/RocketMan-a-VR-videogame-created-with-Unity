using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : MonoBehaviour
{

    [Range(0, 15)]
    public int screenImage;

    public bool on;
    public Transform screen;

    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
      rend = screen.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            rend.material.SetColor("_EmissionColor", new Color(1.517f, 1.517f, 1.517f, 1f));
            rend.material.SetTextureOffset("_MainTex", new Vector2(0.25f * (screenImage % 4), 0.25f * (screenImage / 4)));
        }
        else
        {
            rend.material.SetColor("_EmissionColor", new Color(0.3f, 0.3f, 0.3f, 0.3f));
            rend.material.SetTextureOffset("_MainTex", new Vector2(0.25f * (8 % 4), 0.25f * (8 / 4)));
        }
        
    }
}
