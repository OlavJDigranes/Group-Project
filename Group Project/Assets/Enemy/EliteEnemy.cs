using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

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
    // Modifiable elite enemy AI (Aggressive or defensive) and attack type (Ranged or melee).
    [SerializeField]
    public string AttackType;
    public string AIType;
    public int Health;
    public int Damage;
    public int Speed;

    // The enemy's ability. It is set initially as the ability abstract but will be derived to a specific ability in the start function.
    private Ability EliteAbility;

    // Boolean to determine which way the enemy is facing.
    private bool facingRight;


    // Start is called before the first frame update
    void Start()
    {
        // Currently sets the abstract ability to a specific ability type for this enemy.
        // todo: add conditions to determine the ability to be used.
        EliteAbility = gameObject.AddComponent<DashAbility>();
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
    }
}
