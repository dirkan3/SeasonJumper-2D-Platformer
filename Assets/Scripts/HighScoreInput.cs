using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameController gameController;

    private void Awake()
    {
        Hide();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void OnOKButtonPressed()
    {
        string playerName = nameInputField.text;
        int money = gameController.amountOfMoney;


        PlayerPrefs.SetString("PendingPlayerName", playerName);
        PlayerPrefs.SetInt("PendingPlayerScore", money);
        PlayerPrefs.Save();
        Debug.Log("High score added.");

        HighScoreTable.AddHighScoreEntry(money, playerName);


        Hide();
    }
}
