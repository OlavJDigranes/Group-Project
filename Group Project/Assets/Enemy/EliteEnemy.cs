using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

//todo checklist:
// -> Abilities for enemies (such as projectiles, teleportation, summoning enemies, speed/power enhancements..).
// -> Enemy movement:
// ->   -> Collision with the platforms.
// ->   -> "AI" (Aggressive: chase the player, Defensive: Keep distance from the player).
// ->   -> Jumping
// -> Melee enemies (animated weapon attack or on-touch damage?)
// -> Ranged enemies
// -> Collision triggers with player attacks
// -> Damaging the player on collision (+ Separate damage when using their ability).

public class EliteEnemy : MonoBehaviour
{
    // Modifiable elite enemy health, damage and speed.
    // Modifiable elite enemy AI (Aggressive: 0 or defensive: 1) and attack type (Melee: 0 or ranged: 1).
    [SerializeField]
    public int AttackType;
    public int AIType;
    public int Health;
    public int Damage;
    public int Speed;

    // The enemy's ability. It is set initially as the ability abstract but will be derived to a specific ability in the start function.
    private Ability EliteAbility;

    // Boolean to determine which way the enemy is facing.
    private bool facingRight;

    // Boolean that determines if the ability relies on a timer to function, used to, for example remove new hitboxes or to finish an attack.
    private bool timedAbilty = false;


    // Start is called before the first frame update
    void Start()
    {
        // Determine the ability the enemy should use based on it's attack type and AI type.
        // Uses a nested switch statement to handle this.
        switch (AttackType)
        {
            // Melee
            case 0:
                switch (AIType)
                {
                    // Melee aggressive enemy
                    case 0:
                        EliteAbility = gameObject.AddComponent<DashAbility>();
                        break;

                    // Melee defensive enemy
                    case 1:
                        EliteAbility = gameObject.AddComponent<ShieldBashAbility>();
                        timedAbilty = true;
                        EliteAbility.duration = 999.0f;
                        break;

                    default:
                        Debug.Log("Error: invalid attack type value was assigned for this enemy.");
                        break;
                }
                break;

            // Ranged
            case 1:
                switch (AIType)
                {
                    // Ranged aggressive enemy
                    case 0:
                        // Some ability...
                        break;

                    case 1:
                        // Another ability...
                        break;

                    default:
                        Debug.Log("Error: Invalid AI type value was assigned for this enemy.");
                        break;
                }
                break;
        }
        // Init the facing direction of the enemy.
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Access the enemy's ability component and call it's attack function (which runs on a cooldown-based system)
        EliteAbility.UseAbility(gameObject, facingRight);
        // Decrement the ability cooldown by dt.
        EliteAbility.cooldown -= Time.deltaTime;

        // If the elite ability relies on a after-use timer, decrement it by dt.
        // Once this timer hits 0, run another function to finish/despawn the attack.
        if (timedAbilty)
        {
            EliteAbility.duration -= Time.deltaTime;

            if (EliteAbility.duration <= 0.0f)
            {
                EliteAbility.StopAbility(gameObject);
            }
        }
    }

    //todo: collision with shield bash (dynamic method of differentiating between elite enemy hitbox and shield bash hitbox).
    public void OnCollisionEnter2D(Collision2D col)
    {
    }
}
