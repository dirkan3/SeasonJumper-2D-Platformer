using System;
using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float BounceForce = 20f;
    public int damage = 1;
    public bool _isImmune = false;
    private float immunityDuration = 1.5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isImmune) return;

        if (collision.CompareTag("Player"))
        {
            // Attempt to get the PlayerController component
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                HandlePlayerBounce(player);
                StartCoroutine(Invunerable());
            }         
        }
    }

    private void HandlePlayerBounce(PlayerController player)
    {
        player.ExecuteJump(BounceForce);
    }

    private IEnumerator Invunerable()
    {
        _isImmune = true;
        yield return new WaitForSeconds(immunityDuration);
        _isImmune = false;
    }
}