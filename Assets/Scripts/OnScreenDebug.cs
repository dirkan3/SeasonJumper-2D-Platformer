using UnityEngine;
using TMPro;

public class OnScreenDebug : MonoBehaviour
{
    public GameObject debugPanel;
    public TextMeshProUGUI debugText;
    public ScriptableStats stats;
    public PlayerController playerController;


    private bool consoleOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            consoleOpen = !consoleOpen;
            if (debugPanel != null)
            {
                debugPanel.SetActive(consoleOpen);
            }
            else
            {
                Debug.LogWarning("DebugPanel not in unity");
            }
        }

        if (consoleOpen && debugText != null && stats != null && playerController != null)
        {
            float fps = 1.0f / Time.deltaTime;
            debugText.text = $"FPS: {fps:F1}\n" +
                             $"Fixed Timestep: {Time.fixedDeltaTime:F3}\n" +
                             $"Ground Deceleration: {stats.GroundDeceleration:F3}\n" +
                             $"Frame Velocity X: {playerController.FrameVelocityX:F3}";
        }
    }
}