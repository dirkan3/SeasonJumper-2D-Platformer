using UnityEngine;
using TMPro;

public class Sign : MonoBehaviour
{
    [Header("Sign Data")]
    [TextArea]
    public string infoText; 

    [Header("UI Elements")]
    [SerializeField] private GameObject infoPrompt;  
    [SerializeField] private GameObject infoDisplay;

    private void Awake()
    {
        // Make sure the UI elements start off hidden.
        if (infoPrompt != null)
            infoPrompt.SetActive(false);
        if (infoDisplay != null)
            infoDisplay.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (infoPrompt != null)
                infoPrompt.SetActive(true);

            PlayerInteraction.Instance?.SetCurrentSign(this);
        }
    }

 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (infoPrompt != null)
                infoPrompt.SetActive(false);


            if (PlayerInteraction.Instance != null && PlayerInteraction.Instance.CurrentSign == this)
            {
                PlayerInteraction.Instance.ClearCurrentSign();
            }

            if (infoDisplay != null)
                infoDisplay.SetActive(false);
        }
    }
    public void DisplayInfo()
    {
        if (infoPrompt != null)
            infoPrompt.SetActive(false);

        // Show the info display panel and set its text.
        if (infoDisplay != null)
        {
            infoDisplay.SetActive(true);
            TextMeshProUGUI textComponent = infoDisplay.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = infoText;
            }
        }
    }
}