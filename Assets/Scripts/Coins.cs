using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coins : MonoBehaviour, ICollectables
{
    public static event Action<int> OnCoinsCollect;
    public int worth = 1;

    public string coinID;

    private bool isCollected = false;

    private void Start()
    {
 
        if (string.IsNullOrEmpty(coinID))
        {
            coinID = SceneManager.GetActiveScene().name + "_" + transform.position.ToString();
        }

        if (GameController.Instance != null && GameController.Instance.IsCoinCollected(coinID))
        {
            Destroy(gameObject);
        }
    }
    public void Collect()
    {
        if (!isCollected)
        {
            isCollected = true;
            GetComponent<Collider2D>().enabled = false;
            OnCoinsCollect?.Invoke(worth);

            if (GameController.Instance != null)
            {
                GameController.Instance.MarkCoinAsCollected(coinID);
            }
            Destroy(gameObject);
        }
    }
}
