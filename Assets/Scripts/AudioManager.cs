using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("-----------Audio Source --------------")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-----------Audio Clips --------------")]

    public AudioClip background;
    public AudioClip death;
    public AudioClip portal;
    public AudioClip pause;
    public AudioClip unpause;
    public AudioClip jump;
    public AudioClip healthShop;
    public AudioClip hit;
    public AudioClip dash;
    public AudioClip gold;
    public AudioClip goal;
    public AudioClip dorr;


    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator Start()
    {
        yield return null;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            if (MusicSource != null && background != null)
            {
                if (!MusicSource.isPlaying)
                {
                    MusicSource.clip = background;
                    MusicSource.Play();
                }
            }
            else
            {
                Debug.LogWarning("Music not assigned");
            }
        }
        else
        {
            if (MusicSource != null)
            {
                MusicSource.Stop();
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource == null)
        {
            return;
        }

        if (clip == null)
        {
            return;
        }

        SFXSource.PlayOneShot(clip);
    }

}
