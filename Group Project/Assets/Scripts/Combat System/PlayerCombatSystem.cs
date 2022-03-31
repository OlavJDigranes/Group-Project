using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatSystem : MonoBehaviour
{
    // Attack Variables
    private Transform attackPoint;
    private float attackRange = 0.5f;
    public LayerMask enemyMask;

    private bool facingRight;

    // Player Input
    private PlayerInput playerInput;

    // Action Variables
    private InputAction primaryAttack;
    private InputAction secondaryAttack;
    private InputAction ultimateAttack;
    private InputAction directionCheck;

    private void Awake()
    {
        // Assign AttackPoint
        attackPoint = gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>();
        Debug.Log(attackPoint.name);

        playerInput = GetComponent<PlayerInput>();

        // Attack Actions
        primaryAttack = playerInput.actions["Attack P"];
        secondaryAttack = playerInput.actions["Attack S"];
        ultimateAttack = playerInput.actions["Attack U"];

        // Walk Actions
        directionCheck = playerInput.actions["Walk"];

        // Attack Casts
        primaryAttack.performed += PrimaryCast;
        secondaryAttack.performed += SecondaryCast;
        ultimateAttack.performed += UltimateCast;

        // Walk Casts
        directionCheck.performed += CheckDirection;
    }

    public void CheckDirection(InputAction.CallbackContext obj)
    {
        // Check if the player is moving on the positive axis
        if(directionCheck.ReadValue<float>() > 0)
        {
            // If moving on positive axis, set facingRight to true
            facingRight = true;
        }
        else
        {
            // Otherwise set facingRight to false
            facingRight = false;
        }
    }

    public void PrimaryCast(InputAction.CallbackContext obj)
    {
        if(facingRight)
        {
            // Set the current Attack Point to be 1f from the current player position
            attackPoint.position = new Vector2(gameObject.transform.position.x + 1f, attackPoint.position.y);
        }
        else
        {
            // Set the current Attack Point to be -1f from the current player position
            attackPoint.position = new Vector2(gameObject.transform.position.x + -1f, attackPoint.position.y);
        }

        Attack(10);
    }

    public void SecondaryCast(InputAction.CallbackContext obj)
    {
        if(facingRight)
        {
            // Set the current Attack Point to be 2f from the current player position
            attackPoint.position = new Vector2(gameObject.transform.position.x + 2f, attackPoint.position.y);
        }
        else
        {
            // Set the current Attack Point to be -2f from the current player position
            attackPoint.position = new Vector2(gameObject.transform.position.x + -2f, attackPoint.position.y);
        }

        Attack(8);
    }

    public void UltimateCast(InputAction.CallbackContext obj)
    {
        // Set up the raycast hit variable
        RaycastHit2D hitEnemy;

        // Set a filter on the raycast to only hit enemies
        int layerMask = LayerMask.GetMask("Enemies");

        // Set the raycast to the correct direction
        if (!facingRight)
        {
            // Raycast a bean in front of the player
            hitEnemy = Physics2D.Raycast(attackPoint.position, -Vector2.right, 120f, layerMask);
        }
        else
        {
            // Raycast a bean in front of the player
            hitEnemy = Physics2D.Raycast(attackPoint.position, -Vector2.left, 120f, layerMask);
        }

        // If the raycast hit something
        if (hitEnemy.collider != null)
        {
            // FOR DEBUGGING, check which enemy was hit by the raycast
            Debug.Log("Ult has hit " + hitEnemy.collider.name);

            // Get common enemy HP
            int enemyHP = hitEnemy.collider.GetComponent<CommonEnemy>().health;

            // Update the enemy HP
            hitEnemy.collider.GetComponent<CommonEnemy>().health = enemyHP - 50;

            // Show the enemy HP
            Debug.Log(hitEnemy.collider.name + "'s Health: " + hitEnemy.collider.GetComponent<CommonEnemy>().health);

        }
    }

    private void Attack(int damageDealt)
    {
        // Detect enemies in the attackPoint
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);

        // Go through each enemy hit
        foreach (Collider2D enemy in hitEnemies)
        {
            // Get the enemies health
            int enemyHP = enemy.GetComponent<CommonEnemy>().health;

            // FOR DEBUGGING, show what enemies got hit
            Debug.Log("We hit " + enemy.name);

            // Apply the damage to the enemy
            enemy.GetComponent<CommonEnemy>().health = enemyHP - damageDealt;

            // FOR DEBUGGING, show the enemy health
            Debug.Log(enemy.name + "'s Health: " + enemy.GetComponent<CommonEnemy>().health);
        }
    }
}
