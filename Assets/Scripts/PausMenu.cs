using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;
    public GameObject GameOverUI;
    public GameObject GameWonUI;
    AudioManager audioManager;

    private bool isGameOver = false;
    private bool isGameWon = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver && !isGameWon)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
        audioManager.PlaySFX(audioManager.pause);
    }
    public void Resume()
    {
        PauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        audioManager.PlaySFX(audioManager.unpause);
    }

    public void PauseForGameOver()
    {
        isGameOver = true;
        DisplayMenu(GameOverUI);
    }

    public void PauseForGameWon()
    {
        isGameWon = true;
        DisplayMenu(GameWonUI);
    }

    private void DisplayMenu(GameObject uiObject)
    {
        PauseMenuUI.SetActive(false);
        GameOverUI.SetActive(false);
        GameWonUI.SetActive(false);

        if (uiObject != null)
        {
            uiObject.SetActive(true);
        }

        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}


