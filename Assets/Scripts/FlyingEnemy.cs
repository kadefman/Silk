using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public enum Type {Fly, Bee};
    public Type type;
    public Color[] colors;

    public float speed;
    public int damage;

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
