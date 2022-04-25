using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeleteBarrier : MonoBehaviour
{
    private BoxCollider2D bc;
    private Rigidbody2D rb;

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
        if (collision.gameObject)
    }
}
