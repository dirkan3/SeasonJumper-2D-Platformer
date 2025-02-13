using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    private static PlayerCollect _instance;

    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectables collectables = collision.GetComponent<ICollectables>();
        collectables?.Collect();
    }
}
