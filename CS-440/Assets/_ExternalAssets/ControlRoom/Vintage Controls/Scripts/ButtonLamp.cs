using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLamp : MonoBehaviour
{
    public enum eColor
    {
            Red,
            Yellow,
            Green,
            Blue,
            White,
            Cyan,
            Magenta,
            Black,
            Grey,
    }

    public bool on;
    public Transform lamp;
    public eColor lightColor;

    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = lamp.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            switch (lightColor)
            {
                case eColor.Red:
                    rend.material.SetColor("_EmissionColor", new Color(1f, 0f, 0.02f, 1f));
                    break;
                case eColor.Yellow:
                    rend.material.SetColor("_EmissionColor", new Color(1f, 0.65f, 0f, 1f));
                    break;
                case eColor.Green:
                    rend.material.SetColor("_EmissionColor", new Color(0.15f, 1f, 0f, 1f));
                    break;
                case eColor.Blue:
                    rend.material.SetColor("_EmissionColor", new Color(0f, 0.33f, 1f, 1f));
                    break;
                case eColor.White:
                    rend.material.SetColor("_EmissionColor", Color.white);
                    break;
                case eColor.Cyan:
                    rend.material.SetColor("_EmissionColor", Color.cyan);
                    break;
                case eColor.Magenta:
                    rend.material.SetColor("_EmissionColor", Color.magenta);
                    break;
                case eColor.Black:
                    rend.material.SetColor("_EmissionColor", Color.black);
                    break;
                case eColor.Grey:
                    rend.material.SetColor("_EmissionColor", Color.grey);
                    break;
                default:
                    break;
            }
            
        }
        else
        {
            rend.material.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }
    }
}
