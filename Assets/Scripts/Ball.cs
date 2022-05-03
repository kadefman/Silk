using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;

    private Collider2D coll;

    void Start()
    {
        coll = transform.GetComponent<Collider2D>();
        Vector2 randVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        transform.GetComponent<Rigidbody2D>().AddForce(speed * randVector.normalized);
    }
    void Update()
    {
        
    }
}
