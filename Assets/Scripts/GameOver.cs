using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject GameOverScreen;

    public void RestartGame()
    {
        Debug.Log("I pressed RestartGame");
        SceneManager.LoadScene(0);
    }

    public void GoToHighScoreScreen()
    {
        Debug.Log("I pressed Add HighScore");
    }

    public void LostGame()
    {
        GameOverScreen.SetActive(true);
    }
}
