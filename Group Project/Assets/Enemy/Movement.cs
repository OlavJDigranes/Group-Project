using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    protected float moveSpeed;

    public void Init(int enemyMoveSpeed)
    {
        moveSpeed = enemyMoveSpeed;
    }

    public virtual bool FaceTowardsPlayer(GameObject eliteEnemy, GameObject player)  { return false; }

    public virtual void Move(GameObject eliteEnemy, bool facingRight)  {}

}