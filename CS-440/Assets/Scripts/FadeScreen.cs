using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public delegate void Delegate( string argument);

public class FadeScreen : MonoBehaviour
{
    public float fadeDuration = 2f;
    public Color fadeColor = Color.black;
    private Renderer rend;
    public bool fadeOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (fadeOnStart)
        {
            fadeIn();
        }
    }

    public void fadeIn()
    {
        StartCoroutine(FadeRoutine(1, 0));
    }

    public void fadeOut(Delegate functionToCall, string strParameter)
    {
        StartCoroutine(FadeRoutineWithFinalFunctionCall(functionToCall, strParameter, 0, 1));
    }

    public void fade(float levelOfFade)
    {
        Color newColor = fadeColor;
        newColor.a = levelOfFade;
        rend.material.SetColor("_Color", newColor);
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while(timer <= fadeDuration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut,timer/fadeDuration);
            rend.material.SetColor("_Color", newColor);
            timer += Time.deltaTime;
            yield return null;
        }
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor("_Color", newColor2);
    }
    public IEnumerator FadeRoutineWithFinalFunctionCall(Delegate functionToCall, string strParameter, float alphaIn, float alphaOut)
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);
            rend.material.SetColor("_Color", newColor);
            timer += Time.deltaTime;
            yield return null;
        }
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor("_Color", newColor2);
        functionToCall(strParameter);
    }

}
