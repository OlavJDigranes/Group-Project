using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour
{

    // Attack Variables
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyMask;

    // Check what way the player is facing
    public bool facingLeft;

    // Update is called once per frame
    void Update()
    {
        // Check which if these specific keys were pressed and cast the corresponding attack
        //TODO - Assign each key to a variable that can be changed in the options menu

        // Primary Attack
        if(Input.GetKeyDown(KeyCode.J))
        {
            Attack('P');
        }
        // Secondary Attack
        else if(Input.GetKeyDown(KeyCode.K))
        {
            Attack('S');
        }
        // Ultimate Attack
        else if(Input.GetKeyDown(KeyCode.L))
        {
            Attack('U');
        }

        // Check if the player is facing left or right
        if (Input.GetKeyDown(KeyCode.D))
        {
            facingLeft = true;
        }
        else
        {
            facingLeft = false;
        }
    }

    void Attack(char attackType)
    {
        switch(attackType)
        {
            case 'P':
                // Set the current Attack Point to be 1f from the current player position
                attackPoint.position = new Vector2(GameObject.Find("Player").GetComponent<Transform>().position.x + 1f, attackPoint.position.y);
                break;

            case 'S':
                // Set the current Attack Point to be 2f from the current player position
                attackPoint.position = new Vector2(GameObject.Find("Player").GetComponent<Transform>().position.x + 2f, attackPoint.position.y);
                break;

            case 'U':
                // Set up the raycast hit variable
                RaycastHit2D hitEnemy;

                // Set the raycast to the correct direction
                if (facingLeft)
                {
                    // Raycast a bean in front of the player
                    hitEnemy = Physics2D.Raycast(transform.position, -Vector2.left);
                }
                else
                {
                    // Raycast a bean in front of the player
                    hitEnemy = Physics2D.Raycast(transform.position, -Vector2.right);
                }

                // If the raycast hit something
                if (hitEnemy.collider != null)
                {
                    // FOR DEBUGGING, check which enemy was hit by the raycast
                    Debug.Log("Ult has hit " + hitEnemy.collider.name);
                }
                break;
        }

        // Detect enemies in the attackPoint
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);

        // Go through each enemy hit
        foreach(Collider2D enemy in hitEnemies)
        {
            // TODO - When the enemy script gets created, Apply the damage to hit enemy

            // FOR DEBUGGING, show what enemies got hit
            Debug.Log("We hit " + enemy.name);
        }
    }

    // FOR DEBUGGING, show the wireframe of the attack point and its radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
