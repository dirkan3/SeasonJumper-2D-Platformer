using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartFade(SpriteRenderer spriteRenderer, Color targetColour, float duration)
    {
        StartCoroutine(FadeSpriteCoroutine(spriteRenderer, targetColour, duration));
    }

    private IEnumerator FadeSpriteCoroutine(SpriteRenderer spriteRenderer, Color targetColour, float duration)
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        Color startColour = spriteRenderer.color;
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {

            if (spriteRenderer == null)
            {
                yield break; 
            }
            spriteRenderer.color = Color.Lerp(startColour, targetColour, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = targetColour;
        }
    }
}
