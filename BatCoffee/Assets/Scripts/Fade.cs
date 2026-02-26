using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Fade : MonoBehaviour
{
    private SpriteRenderer sr;
    private float fadeDuration = 1f;
    private float visibleTime = 0.005f;

    private Coroutine currentFade;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SetOpacity(0f);
    }

    public void FadeReveal()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(Reveal());
    }

    IEnumerator Reveal()
    {
        yield return StartCoroutine(FadeObj(0f, 1f));

        yield return new WaitForSeconds(visibleTime);

        yield return StartCoroutine(FadeObj(1f, 0f));
    }

    IEnumerator FadeObj(float start, float end)
    {
        float time = 0f;
        Color color = sr.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(start, end, time / fadeDuration);
            sr.color = color;
            yield return null;
        }

        color.a = end;
        sr.color = color;
    }

    public void SetOpacity(float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }
}
