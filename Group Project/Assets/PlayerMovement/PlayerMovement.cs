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
    CapsuleCollider2D cc;

    // Walk & Crouch vars to switch between walk and crouch sizings on capsule
    Vector2 cc_size_Walk;
    Vector2 cc_offset_Walk;
    Vector2 cc_size_Crouch;
    Vector2 cc_offset_Crouch;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        cc = gameObject.GetComponent<CapsuleCollider2D>();
        cc_size_Walk = cc.size;
        cc_size_Crouch = new Vector2(cc_size_Walk.x, cc_size_Walk.y - 0.5f);
        cc_offset_Crouch = new Vector2(cc_offset_Walk.x, cc_offset_Walk.y - 0.25f);
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
        if (Input.GetKey(KeyCode.DownArrow)) // Crouch --- TODO: add controller axis
        {
            cc.size = cc_size_Crouch;
            cc.offset = cc_offset_Crouch;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow)) // Release crouch
        {
            cc.size = cc_size_Walk;
            cc.offset = cc_offset_Walk;
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
