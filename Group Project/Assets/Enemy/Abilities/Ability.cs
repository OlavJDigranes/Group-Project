using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// Abstract ability class.
/// </summary>
public abstract class Ability : MonoBehaviour
{
    // Damage inflicted by this ability.
    protected int damage;

    // Cooldown timer thats triggered on ability use.
    protected float cooldown;

    // Duration of the ability.
    protected float duration;

    // Easy, readable method of determining if the ability has a duration.
    protected bool hasDuration;

    public int GetDamage() { return damage; }
    public float GetCooldown () { return cooldown; }
    public float GetDuration() { return duration; }
    public bool HasDuration() { return hasDuration; }


    public void UpdateDuration(float dt) { duration -= dt; }
    public void UpdateCooldown(float dt) { cooldown -= dt; }


    /// <summary>
    /// Virtual method that makes the enemy use it's ability.
    /// </summary>
    /// <param name="eliteEnemy">The enemy game object (It is automatically passed as a reference so all changes to this game object will affect the original).</param>
    /// <param name="facingRight">Boolean that determines the enemy's facing direction (used to determine, for example, which way to dash or aim).</param>
    public virtual void UseAbility(GameObject eliteEnemy, bool facingRight) { Debug.Log("Error: An elite enemy is not using a derived ability!"); }

    /// <summary>
    /// Virtual method that ends the effects of an ability that has a use time
    /// </summary>
    /// <param name="eliteEnemy"> The elite enemy object whose ability is being ended. </param>
    public virtual void StopAbility(GameObject eliteEnemy) {Debug.Log("Error: Elite enemy attempted to stop an ability with no use time!"); }

    /// <summary>
    /// Setup parameters of the ability.
    /// </summary>
    /// <param name="monsterLevel"> Level of the monster (Used to scale the stats of the ability to make them deadlier as the game progresses).</param>
    public virtual void Init(int monsterLevel) {Debug.Log("Error: Init function called on a non-existent ability!"); }

    /// <summary>
    /// Checks if an ability is suitable for use for the elite enemy. Each check is different per ability but will generally check for player position relative
    /// to the enemy and the ability cooldown.
    /// </summary>
    /// <param name="eliteEnemyPosition">Position of the enemy. </param>
    /// <param name="playerPosition">Position of the player. </param>
    public virtual bool CheckAbilityUsage(Vector2 eliteEnemyPosition, Vector2 playerPosition) {Debug.Log("Abstract ability check function called."); return false; }
}