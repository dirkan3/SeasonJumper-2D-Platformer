using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 2f; 
    public int damage = 1;

    private bool movingRight = true;
    public Transform groundDetection;

    private bool hasFlipped = false;
    private float flipCooldown = 0.2f;
    private float flipTimer = 0f;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        Debug.DrawRay(groundDetection.position, Vector2.down * distance, Color.red);

        if (hasFlipped)
        {
            flipTimer += Time.deltaTime;
            if (flipTimer >= flipCooldown)
            {
                hasFlipped = false;
                flipTimer = 0f;
            }
        }
        if (!hasFlipped)
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);
            if (groundInfo.collider == null)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        if (movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = true;
        }
        hasFlipped = true;
    }
}