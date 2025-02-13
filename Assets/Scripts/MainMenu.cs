using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStartGameButtonPressed()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.StartNewGame();
        }
        else
        {

            SceneManager.LoadScene("Home");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
