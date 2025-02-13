using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public enum PortalType { NextLevel, PreviousLevel }
    public PortalType portalType;
    [SerializeField] Animator transitionAnim;
    [SerializeField] private string targetSceneName;

    public static event Action<string> OnPortal;

    private static Portal _instance;
    AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (portalType == PortalType.NextLevel)
            {
                StartCoroutine(LoadNextLevel());
            }
            else if (portalType == PortalType.PreviousLevel)
            {
                StartCoroutine(LoadPreviousLevel());
            }
        }
    }

    IEnumerator LoadNextLevel()
    {

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            transitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(1.5f);

            audioManager.PlaySFX(audioManager.portal); // audio
            SceneManager.LoadScene(nextSceneIndex);

            if (!string.IsNullOrEmpty(targetSceneName))
            {
                transitionAnim.SetTrigger("End");

                OnPortal?.Invoke(targetSceneName);

                SceneManager.LoadScene(targetSceneName);
            }
            else { Debug.LogWarning("Target scene name is not set"); }
        }
        else
        {
            Debug.LogWarning("Next level does not exist");
        }
    }

    IEnumerator LoadPreviousLevel()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

        if (previousSceneIndex >= 0)
        {
            transitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(2f);

            audioManager.PlaySFX(audioManager.portal);   // audio
            SceneManager.LoadScene(previousSceneIndex);

            if (!string.IsNullOrEmpty(targetSceneName))
            {
                transitionAnim.SetTrigger("End");
                //yield return new WaitForSeconds(1.5f);

                OnPortal?.Invoke(targetSceneName);

                SceneManager.LoadScene(targetSceneName);
            }
            else { Debug.LogWarning("Target scene name is not set"); }
        }
        else
        {
            Debug.LogWarning("Previous level does not exist");
        }
    }
}