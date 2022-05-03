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
    private const float dashStrength = 2.5f;
    private const float jumpStrength = 5f;

    //Modifiers that will be used contextually
    private float movementModifier = 1f;
    private float jumpModifier = 1f;

    //Facing right bool for sprite orientation
    private bool isFacingRight = true;

    //Bool to indicate player has jumped
    private bool hasJumped = false;
    private bool hasDoubleJumped = false;

    //Bool to indicate player has dashed
    private bool hasDashed = false;
    private float dashTimer = 0f;

    //Necessary Components on GameObject
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;
    private PlayerInput playerInput;

    //Stored player actions for easier access
    private InputAction walkAction;
    private InputAction dashAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    // Walk & Crouch vars to switch between walk and crouch sizings on capsule
    private Vector2 cc_size_Walk;
    private Vector2 cc_offset_Walk;
    private Vector2 cc_size_Crouch;
    private Vector2 cc_offset_Crouch;

    // Camera
    private Camera cam;
    private float yAdj_jump = 0;
    private float lerpTime = 0;

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
        dashAction = playerInput.actions["Dash"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];

        walkAction.performed += handleWalk;
        walkAction.canceled += handleWalk;
        dashAction.performed += handleDash;
        dashAction.canceled += handleDash;
        jumpAction.performed += handleJump;
        //jumpAction.canceled += handleJump; --- Might need this later? Hold to climb on contextual surfaces or something (Rory)
        crouchAction.performed += handleCrouch;
        crouchAction.canceled += handleCrouch;
        if (gameObject.transform.GetChild(0).GetComponentInChildren<Camera>() != null)
        {
            cam = gameObject.transform.GetChild(0).GetComponentInChildren<Camera>();
        }
    }

    private void handleDash(InputAction.CallbackContext obj)
    {
        if (!obj.canceled && !hasDashed)
        {
            rb.AddForce(new Vector2(isFacingRight ? dashStrength : -dashStrength, 0f), ForceMode2D.Impulse);
            hasDashed = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if movement vector has been updated, move character
        if (cur_Movement.magnitude > 0)
        {
            rb.position += cur_Movement * Time.deltaTime * movementModifier;
        }
        if (dashTimer >= 1.8f)
        {
            dashTimer = 0f;
            hasDashed = false;
        }
        if (hasDashed)
        {
            dashTimer += Time.deltaTime;
        }
        if (hasJumped && rb.velocity.y > 0)
        {
            if (lerpTime < 1)
            {
                lerpTime += Time.deltaTime;
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, 3 - lerpTime, cam.transform.localPosition.z);
            }
        }
        if (rb.velocity.y <= 0 && lerpTime > 0)
        {
            lerpTime -= Time.deltaTime;
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, 3 - lerpTime, cam.transform.localPosition.z);
        }

        Gizmos.color = Color.yellow;
        float step = 0.2f;
        float r = Mathf.Min(cc.size.x, cc.size.y) / 2;
        float d = Mathf.Max(cc.size.x, cc.size.y) / 2 - r;
        Vector3 lp = Vector3.positiveInfinity;
        for (float a = -Mathf.PI; a <= Mathf.PI + step; a += step)
        {
            float x = Mathf.Cos(a);
            float y = Mathf.Sin(a);
            Vector3 np = new Vector3(x, y) * r;
            np += (cc.direction == CapsuleDirection2D.Vertical ? Vector3.up : Vector3.zero) * d * Mathf.Sign(y);
            np += (cc.direction == CapsuleDirection2D.Horizontal ? Vector3.right : Vector3.zero) * d * Mathf.Sign(x);
            np = transform.TransformPoint(np);
            if (lp != Vector3.positiveInfinity) Gizmos.DrawLine(lp, np);
            lp = np;
        }
    }

    //Delegated jump event
    private void handleJump(InputAction.CallbackContext obj)
    {
        if (!obj.canceled && !hasDoubleJumped) //only if pressed past threshold
        {
            rb.AddForce(new Vector2(0f, jumpStrength * jumpModifier), ForceMode2D.Impulse);
            if (!hasJumped)
            {
                hasJumped = true;
                yAdj_jump = gameObject.transform.position.y;
            }
            else if (!hasDoubleJumped)
            {
                hasDoubleJumped = true;
            }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasJumped)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                //Debug.DrawLine(rb.position, contact.point, Color.green, 2f, true);
                if (Vector2.Angle(Vector2.down, -contact.normal) <= 45)
                {
                    hasJumped = false;
                    hasDoubleJumped = false;
                }
            } 
        }
    }
}
