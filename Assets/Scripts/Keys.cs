using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour, ICollectables
{
    public static event Action OnKeyCollect;

    private bool isCollected = false;

    public void Collect()
    {
        if (!isCollected)
        {
            isCollected = true;
            GetComponent<Collider2D>().enabled = false;
            OnKeyCollect?.Invoke(); 
            Destroy(gameObject); 
        }
    }
}
