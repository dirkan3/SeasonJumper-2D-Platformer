using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform PointA;
    public Transform PointB;
    public float moveSpeed = 2f;

    private Vector3 nextPosition;
    private Vector3 previousPosition;

    void Start()
    {
        nextPosition = PointB.position;
        previousPosition = transform.position;
    }

    void Update()
    {
        // Move the platform between points in Update to maintain smooth visuals
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == PointA.position) ? PointB.position : PointA.position;
        }
    }

    private void FixedUpdate()
    {
        Vector3 platformMovement = transform.position - previousPosition;
        previousPosition = transform.position;

        MovePlayerOnPlatform(platformMovement);
    }

    private void MovePlayerOnPlatform(Vector3 platformMovement)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Rigidbody2D playerRb = collider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Apply platform movement smoothly through velocity
                    playerRb.velocity += (Vector2)platformMovement / Time.fixedDeltaTime;
                }
            }
        }
    }
}


