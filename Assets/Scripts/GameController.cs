using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    AudioManager audioManager;
    public HighScoreInput highScoreInput;
    public static GameController Instance;
    public PlayerHealth playerHealth;
    public GameObject gameOverScreen;
    public GameObject gameWonScreen;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gameOverSurvivedText;
    public TextMeshProUGUI gameWonSurvivedText;
    public TextMeshProUGUI LevelText;

    public GameObject SummerDoor;
    public GameObject AutumnDoor;
    public GameObject WinterDoor;

    private static bool summerDoorDestroyed = false;
    private static bool autumnDoorDestroyed = false;
    private static bool winterDoorDestroyed = false;

    private bool newGameRequested = false;

    public string menuSceneName = "Menu";
    public int amountOfMoney = 0;

    private bool isGameOver = false;
    private bool isGameWon = false;
    private PausMenu pauseMenu;
    private Coroutine levelTextCoroutine;
    private HashSet<string> collectedCoinIDs = new HashSet<string>();

    public int storedPlayerHealth = -1;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        pauseMenu = FindObjectOfType<PausMenu>();
    }

    private void OnEnable()
    {
        Coins.OnCoinsCollect += IncreaseMoney;
        Keys.OnKeyCollect += HandleKeyCollection;
        PlayerHealth.OnPlayerDied += ShowGameOverScreen;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Portal.OnPortal += HandlePortalTransition;
    }

    private void OnDisable()
    {
        Coins.OnCoinsCollect -= IncreaseMoney;
        Keys.OnKeyCollect -= HandleKeyCollection;
        PlayerHealth.OnPlayerDied -= ShowGameOverScreen;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Portal.OnPortal -= HandlePortalTransition;
    }

    private void HandlePortalTransition(string sceneName)
    {
        if (levelTextCoroutine != null)
        {
            StopCoroutine(levelTextCoroutine);
        }

        levelTextCoroutine = StartCoroutine(ShowAndHideLevelText(sceneName));
    }

    private IEnumerator ShowAndHideLevelText(string sceneName)
    {
        LevelText.text = sceneName;
        LevelText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.5f); // Show text for 3 seconds

        LevelText.gameObject.SetActive(false);
    }

    public bool IsCoinCollected(string coinID)
    {
        return collectedCoinIDs.Contains(coinID);
    }

    public void MarkCoinAsCollected(string coinID)
    {
        if (!collectedCoinIDs.Contains(coinID))
        {
            collectedCoinIDs.Add(coinID);
        }
    }

    private void IncreaseMoney(int money)
    {
        audioManager.PlaySFX(audioManager.gold);
        amountOfMoney += money;
        coinText.text = $": {amountOfMoney}";
    }

    private void ShowGameOverScreen()
    {
        if (isGameOver) return;  

        isGameOver = true;

        gameOverScreen.SetActive(true);
        gameOverSurvivedText.text = $"You collected {amountOfMoney} coins.";

        pauseMenu?.PauseForGameOver();
    }

    public void ShowGameWonScreen()
    {
        if (isGameWon) return;

        isGameWon = true;
        gameWonScreen.SetActive(true);

        gameWonSurvivedText.text = $"You collected {amountOfMoney} coins!";

        pauseMenu?.PauseForGameWon();
    }


    public void ResetGame()
    {
        Debug.Log("ResetGame called!");

        isGameOver = false;
        isGameWon = false;

        gameOverScreen.SetActive(false);
        gameWonScreen.SetActive(false);

        Time.timeScale = 1f;
        amountOfMoney = 0;

        if (playerHealth != null)
        {
            Debug.Log("Resetting player health...");
            playerHealth.ResetHealth(); 
        }

        ReconnectHealthUI();

        ResetPersistentData();

        LevelMove levelMove = FindObjectOfType<LevelMove>();
        if (levelMove != null)
        {
            levelMove.TeleportToHome();
        }
        else
        {
            Debug.LogError("LevelMove script not found!");
        }

        SceneManager.LoadScene("Home");
    }

    private void ReconnectHealthUI()
    {
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        HealthUI healthUI = FindObjectOfType<HealthUI>();

        if (playerHealth == null || healthUI == null)
        {
            Debug.LogError("PlayerHealth / HealthUI is not found!");
            return;
        }
        playerHealth.healthUI = healthUI;  
    }

    public void GoToHighScoreScreen() // Add a new highScore
    {
        Debug.Log("I pressed Add HighScore");
        highScoreInput.Show();
    }

    public void ReturnToMenu() // QuitGame
    {
        Debug.Log("I pressed Quit");

        Time.timeScale = 1f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            Debug.Log("Player destroyed");
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
        Instance.ResetPersistentData();
        SceneManager.LoadScene(menuSceneName);
    }

    private void ResetPersistentData()
    {
        collectedCoinIDs.Clear();

        summerDoorDestroyed = false;
        autumnDoorDestroyed = false;
        winterDoorDestroyed = false;

        amountOfMoney = 0;

        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        var persistentObjects = FindObjectsOfType<MonoBehaviour>().Where(obj => obj.gameObject.CompareTag("Persistent"));
        foreach (var obj in persistentObjects)
        {
            Destroy(obj.gameObject);
        }
    }

    public void StartNewGame()
    {
        Debug.Log("Starting a new game...");

        newGameRequested = true;

        amountOfMoney = 0;

        SceneManager.LoadScene("Home");
    }


    // handle doors between levels and some playerhealth

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Home")
        {
            PlayerHealth ph = FindObjectOfType<PlayerHealth>();
            if (ph != null)
            {
                if (storedPlayerHealth > 0)
                {
                    ph.currentHealth = storedPlayerHealth;
                }
                else
                {
                    // else, default to full health.
                    ph.currentHealth = ph.maxHealth;
                    storedPlayerHealth = ph.maxHealth;
                }
                if (ph.healthUI != null)
                {
                    ph.healthUI.SetMaxHearts(ph.maxHealth);
                    ph.healthUI.UpdateHeart(ph.currentHealth);
                }
            }
        }
        else
        {
            ReconnectHealthAndUI();
        }

        // Door handling logic...
        string currentSceneName = scene.name;
        SummerDoor = GameObject.Find("SummerDoor");
        AutumnDoor = GameObject.Find("AutumnDoor");
        WinterDoor = GameObject.Find("WinterDoor");

        if (currentSceneName == "SummerLevel" && summerDoorDestroyed && SummerDoor != null)
        {
            Destroy(SummerDoor);
        }
        else if (currentSceneName == "AutumnLevel" && autumnDoorDestroyed && AutumnDoor != null)
        {
            Destroy(AutumnDoor);
        }
        else if (currentSceneName == "WinterLevel" && winterDoorDestroyed && WinterDoor != null)
        {
            Destroy(WinterDoor);
        }
    }

    private void ReconnectHealthAndUI()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        HealthUI healthUI = FindObjectOfType<HealthUI>();
        if (playerHealth != null && healthUI != null)
        {
            Debug.Log("Assigning HealthUI to PlayerHealth...");
            playerHealth.healthUI = healthUI;
        }
        else
        {
            Debug.LogError("PlayerHealth or HealthUI not found");
        }
    }



    private void HandleKeyCollection()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "SummerLevel" && SummerDoor != null && !summerDoorDestroyed)
        {
            Destroy(SummerDoor);
            summerDoorDestroyed = true;
        }
        else if (currentSceneName == "AutumnLevel" && AutumnDoor != null && !autumnDoorDestroyed)
        {
            Destroy(AutumnDoor);
            autumnDoorDestroyed = true;
        }
        else if (currentSceneName == "WinterLevel" && WinterDoor != null && !winterDoorDestroyed)
        {
            Destroy(WinterDoor);
            winterDoorDestroyed = true;
        }
    }
}