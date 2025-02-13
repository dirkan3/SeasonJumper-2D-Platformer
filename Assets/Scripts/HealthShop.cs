using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthShop : MonoBehaviour
{
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && InputManager.InteractionWasPressed)
        {
            FindObjectOfType<PlayerInteraction>()?.InteractWithHealthShop(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

        }
    }
}

