using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public float defaultFadeLength = 2;
    public bool fadeOnStart = true;

    private Renderer fadeRenderer;
    private Material fadeMaterial;
    [HideInInspector]
    public bool isFading = false;

    public static Fader singleton;

    public Color FadeColor { get => fadeMaterial.GetColor("_Color"); set => fadeMaterial.SetColor("_Color",value); }

    private void Awake()
    {
        if (singleton && singleton != this)
            Destroy(gameObject);
        else
            singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeRenderer = GetComponent<Renderer>();
        fadeMaterial = fadeRenderer.material;

        if(fadeOnStart)
        {
            FadeOut(defaultFadeLength);
        }
    }

    public void FadeIn(float duration)
    {
        if (!isFading)
            StartCoroutine(FadeRoutine(duration,0,1));
    }

    public void FadeOut(float duration)
    {
        if(!isFading)
            StartCoroutine(FadeRoutine(duration, 1, 0));
    }

    public IEnumerator FadeRoutine(float duration, float alphaStart, float alphaEnd)
    {
        isFading = true;
        float ElapsedTime = 0.0f;
        FadeColor = new Color(FadeColor.r, FadeColor.g, FadeColor.b,alphaStart);

        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;
            FadeColor = new Color(FadeColor.r, FadeColor.g, FadeColor.b, Mathf.Lerp(alphaStart,alphaEnd,ElapsedTime/duration));
            yield return null;
        }
        
        isFading = false;
        FadeColor = new Color(FadeColor.r, FadeColor.g, FadeColor.b, alphaEnd);
    }

    private void Update()
    {
        if (FadeColor.a == 0)
            fadeRenderer.enabled = false;
        else
            fadeRenderer.enabled = true;
    }
}
