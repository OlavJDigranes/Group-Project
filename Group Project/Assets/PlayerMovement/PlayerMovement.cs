using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    //Base movement values
    private Vector2 cur_Movement;
    private const float movementSpeed = 3.25f;
    private const float jumpStrength = 5f;

    //Modifiers that will be used contextually
    private float movementModifier = 1f;
    private float jumpModifier = 1f;

    //Facing right bool for sprite orientation
    private bool isFacingRight = true;

    //Necessary Components on GameObject
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;
    private PlayerInput playerInput;

    //Stored player actions for easier access
    private InputAction walkAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    // Walk & Crouch vars to switch between walk and crouch sizings on capsule
    private Vector2 cc_size_Walk;
    private Vector2 cc_offset_Walk;
    private Vector2 cc_size_Crouch;
    private Vector2 cc_offset_Crouch;

    //Process components needed for functions
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        cc = gameObject.GetComponent<CapsuleCollider2D>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerInput.currentActionMap.Enable();
        cc_size_Walk = cc.size;
        cc_offset_Walk = cc.offset;
        cc_size_Crouch = new Vector2(cc_size_Walk.x, cc_size_Walk.y - 0.5f);
        cc_offset_Crouch = new Vector2(cc_offset_Walk.x, cc_offset_Walk.y - 0.25f);
    }


    // Start is called before the first frame update
    void Start()
    {
        walkAction = playerInput.actions["Walk"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];

        walkAction.performed += handleWalk;
        walkAction.canceled += handleWalk;
        jumpAction.performed += handleJump;
        //jumpAction.canceled += handleJump; --- Might need this later? Hold to climb on contextual surfaces or something (Rory)
        crouchAction.performed += handleCrouch;
        crouchAction.canceled += handleCrouch;
    }

    // Update is called once per frame
    void Update()
    {
        //if movement vector has been updated, move character
        if (cur_Movement.magnitude > 0)
        {
            rb.position += cur_Movement * Time.deltaTime * movementModifier;
        }
    }

    //Delegated jump event
    private void handleJump(InputAction.CallbackContext obj)
    {
        if (!obj.canceled) //only if pressed past threshold
        {
            rb.AddForce(new Vector2(0f, jumpStrength * jumpModifier), ForceMode2D.Impulse);
        }
    }

    //Delegated walk event
    private void handleWalk(InputAction.CallbackContext obj)
    {
        if (!obj.canceled) //only if pressed past threshold
        {
            if (walkAction.ReadValue<float>() > 0) //if axis positive
            {
                cur_Movement.x += movementSpeed;
                if (isFacingRight != true)
                {
                    isFacingRight = true;
                    sr.flipX = false;
                }
            }
            else //if axis negative
            {
                cur_Movement.x -= movementSpeed;
                if (isFacingRight != false)
                {
                    isFacingRight = false;
                    sr.flipX = true;
                }
            }
        }
        else //else if axis neutral
        {
            cur_Movement.x = 0;
        }
    }

    //Delegated Crouch event
    private void handleCrouch(InputAction.CallbackContext obj)
    {
        if (!obj.canceled) //only if pressed past threshold
        {
            cc.size = cc_size_Crouch;
            cc.offset = cc_offset_Crouch;
        }
        else
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
