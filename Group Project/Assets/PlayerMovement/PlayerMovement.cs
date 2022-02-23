using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    const float movementSpeed = 5f;
    const float jumpStrength = 5f;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        handleMovementInputs(Time.deltaTime);
    }

    private void handleMovementInputs(float dt)
    {
        Vector2 moveVec = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //move right
            moveVec.x += movementSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveVec.x -= movementSpeed;
        }
        rb.position += moveVec * dt;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.AddForce(new Vector2(0f, jumpStrength), ForceMode2D.Impulse);
        }

    }
}
