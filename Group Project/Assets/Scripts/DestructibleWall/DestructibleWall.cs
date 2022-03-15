using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    //Reference to required components
    SpriteRenderer sr;
    BoxCollider2D bc;
    Rigidbody2D rb;
    Color color;

    [SerializeField]
    private int hp;

    // Start is called before the first frame update
    void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        color = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Attacked()
    {
        --hp;
        color.a -= (1/hp);
        sr.color = color;
        Debug.Log(hp);
    }
}
