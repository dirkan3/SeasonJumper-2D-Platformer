
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMove : MonoBehaviour
{
    public float holdDuration = 1f;
    public Image fillCircle;       

    private float holdTimer = 0f;
    private bool isHolding = false;

    private TrailRenderer tr;

    public string homeSceneName = "Home";

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        HandleTeleportInput();
    }

    private void HandleTeleportInput()
    {
        if (Input.GetKey(KeyCode.T))
        {
            holdTimer += Time.deltaTime;
           
            if (fillCircle != null)
            {
                fillCircle.fillAmount = holdTimer / holdDuration;
            }

            else
            {
                Debug.LogWarning("fillCircle is not assigned!");
            }

            if (holdTimer >= holdDuration && !isHolding)
            {
                isHolding = true;
                TeleportToHome(); 
            }
        }
        else
        {
            holdTimer = 0f;
            if (fillCircle != null)
            {
                fillCircle.fillAmount = 0f;
            }
            isHolding = false;
        }
    }

    public void TeleportToHome()
    {

        if (tr != null)
        {
            tr.emitting = false;
        }

        audioManager.PlaySFX(audioManager.portal);

        SceneManager.sceneLoaded += OnHomeSceneLoaded;
        SceneManager.LoadScene(homeSceneName);
    }

    private void OnHomeSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == homeSceneName)
        {
            // Reset the player position in the Home scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(-2, -2, 0); 

                if (tr != null)
                {
                    tr.emitting = true;
                }
            }
            else
            {
                Debug.LogWarning("Player not in Home scene");
            }
        }

        SceneManager.sceneLoaded -= OnHomeSceneLoaded;
    }
}