using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    //Base movement values
    private const float movementSpeed = 3.25f;
    private const float jumpStrength = 5f;

    //Modifiers that will be used contextually
    private float movementModifier = 1f;
    private float jumpModifier = 1f;

    //Facing right bool for sprite orientation
    private bool isFacingRight = true;

    Rigidbody2D rb;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        handleMovementInputs(Time.deltaTime);
    }

    //Function to handle movement inputs (keeps update method tidy)
    private void handleMovementInputs(float dt)
    {
        Vector2 moveVec = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("DPAD_Horizontal") > 0)   //Move right on screen
        {
            moveVec.x += movementSpeed;
            if (isFacingRight != true)
            {
                isFacingRight = true;
                sr.flipX = false; 
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("DPAD_Horizontal") < 0)    // Move left on screen
        {
            moveVec.x -= movementSpeed;
            if (isFacingRight != false)
            {
                isFacingRight = false;
                sr.flipX = true; 
            }
        }
        rb.position += moveVec * dt * movementModifier;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetButtonDown("Controller_Jump"))  // Jump
        {
            rb.AddForce(new Vector2(0f, jumpStrength * jumpModifier), ForceMode2D.Impulse);
        }
    }

    float GetMovementModifier()
    {
        return movementModifier;
    }
    void SetMovementModifier(float multiplier)
    {
        movementModifier = multiplier;
    }

    float GetJumpModifier()
    {
        return jumpModifier;
    }
    void SetJumpModifier(float multiplier)
    {
        jumpModifier = multiplier;
    }
}
