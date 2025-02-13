using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretArea : MonoBehaviour
{
    public float fadeDuration = 1f;
    SpriteRenderer spriteRenderer;
    Color hiddenColour;
    Coroutine currentCorutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hiddenColour = spriteRenderer.color;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentCorutine != null)
            {
                StopCoroutine(currentCorutine);
            }
            FadeManager.Instance.StartFade(spriteRenderer, new Color(hiddenColour.r, hiddenColour.g, hiddenColour.b, 0f), fadeDuration);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentCorutine != null)
            {
                StopCoroutine(currentCorutine);
            }
            FadeManager.Instance.StartFade(spriteRenderer, hiddenColour, fadeDuration);
        }
    }
}
