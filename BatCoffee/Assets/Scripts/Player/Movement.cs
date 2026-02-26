using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;
    public float bumpForce = 5f; // Force applied when hitting an obstacle

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from the player
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Calculate movement vector
        Vector3 move = new Vector3(moveX, moveY, 0f);

        // Apply movement
        transform.position += move * moveSpeed * Time.deltaTime;

        // Mirror the sprite based on movement direction
        if (moveX < 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (moveX > 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
    }

}
