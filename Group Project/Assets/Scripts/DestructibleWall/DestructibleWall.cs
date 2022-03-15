using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    //Reference to wall's rigidbody
    BoxCollider2D bc;
    Rigidbody2D rb;

    [SerializeField]
    int hp;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contacts in collision.contacts)
        {
            //contacts.
        }
    }

    void Attacked()
    {
        --hp;
        Debug.Log(hp);
    }
}
