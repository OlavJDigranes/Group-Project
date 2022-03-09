using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EliteEnemy : MonoBehaviour
{
    [SerializeField]
    public string AttackType;
    public string AIType;
    public int Health;
    public int Damage;
    public int Speed;

    private DashAbility EliteAbility;
    private Rigidbody2D rb;
    private bool facingRight;

    


    // Start is called before the first frame update
    void Start()
    {
        EliteAbility = gameObject.AddComponent<DashAbility>();
        EliteAbility.name = "Dash";
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        EliteAbility.dashAttack(gameObject, facingRight);
        EliteAbility.cooldown -= Time.deltaTime;
        Debug.Log(EliteAbility.cooldown);
    }
}
