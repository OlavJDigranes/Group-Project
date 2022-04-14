using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatSystem : MonoBehaviour
{
    // Attack Variables
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyMask;

    private bool facingRight;

    private PlayerInput playerInput;

    // Action Variables
    private InputAction primaryAttack;
    private InputAction secondaryAttack;
    private InputAction ultimateAttack;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        primaryAttack = playerInput.actions["Attack P"];
        secondaryAttack = playerInput.actions["Attack S"];
        ultimateAttack = playerInput.actions["Attack U"];

        primaryAttack.performed += PrimaryCast;
        secondaryAttack.performed += SecondaryCast;
        ultimateAttack.performed += UltimateCast;
    }

    public void PrimaryCast(InputAction.CallbackContext obj)
    {
        // Set the current Attack Point to be 1f from the current player position
        attackPoint.position = new Vector2(gameObject.GetComponentInParent<Transform>().position.x + 1f, attackPoint.position.y);

        Attack();
    }

    public void SecondaryCast(InputAction.CallbackContext obj)
    {
        // Set the current Attack Point to be 2f from the current player position
        attackPoint.position = new Vector2(gameObject.GetComponentInParent<Transform>().position.x + 2f, attackPoint.position.y);

        Attack();
    }

    public void UltimateCast(InputAction.CallbackContext obj)
    {
        // Set up the raycast hit variable
        RaycastHit2D hitEnemy;

        // Set the raycast to the correct direction
        if (facingRight)
        {
            // Raycast a bean in front of the player
            hitEnemy = Physics2D.Raycast(transform.position, -Vector2.right);
        }
        else
        {
            // Raycast a bean in front of the player
            hitEnemy = Physics2D.Raycast(transform.position, -Vector2.left);
        }

        // If the raycast hit something
        if (hitEnemy.collider != null)
        {
            // FOR DEBUGGING, check which enemy was hit by the raycast
            Debug.Log("Ult has hit " + hitEnemy.collider.name);

        }
    }

    private void Attack()
    {
        // Detect enemies in the attackPoint
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);

        // Go through each enemy hit
        foreach (Collider2D enemy in hitEnemies)
        {
            // TODO - When the enemy script gets created, Apply the damage to hit enemy

            // FOR DEBUGGING, show what enemies got hit
            Debug.Log("We hit " + enemy.name);
        }
    }
}
