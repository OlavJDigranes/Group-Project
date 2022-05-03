using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Generic enemy stats
    protected int health;
    protected int contactDamage;
    protected float moveSpeed;

    // Generic enemy drops
    protected float expOnDeath;
    protected float goldOnDeath;

    // Start is called before the first frame update
    public virtual void Start() {}

    // Update is called once per frame
    public virtual void Update() {}

    // Getters, primarily used to retrieve enemy stats for enemy-to-player interactions
    public int GetHealth() { return health; }
    public int GetDamage() {return contactDamage; }
    public float GetExp() {return expOnDeath; }
    public float GetGold() {return goldOnDeath; }

    // Gets the damage of the monster's ability.
    public float GetAbilityDamage()
    {
        // Boss enemy, who has multiple abilities, so query which ability.
        if(gameObject.TryGetComponent<BossEnemy>(out var boss))
        {
            return GetBossAbilityDamage(gameObject.GetComponent<BossEnemy>().i);
        }
        
        // Elite enemy, who has one ability.
        else
        {
            return gameObject.GetComponent<Ability>().GetDamage();
        }
    }
    
    // Same as get ability damage but retrieves the damage for a specific ability.
    public float GetBossAbilityDamage(int i)
    {
        return gameObject.GetComponents<Ability>()[i].GetDamage();
    }

    // Called when enemy health is 0 or less.
    public void KillEnemy(GameObject player)
    {
        // Quick check to ensure that enemy doesn't accidentally die early.
        if (health > 0) { return; }

        // Kill enemy, give player it's gold and exp.
        Destroy(gameObject); 
        //  player.GetComponent<PlayerScriptComponent>().AddGold(GetGold());
        //  player.GetComponent<PlayerScriptComponent>().AddExp(GetExp());
    }
}
