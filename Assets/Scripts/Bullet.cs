using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject prefabFx;
    public float speed;
    public float airTime;
    
    [HideInInspector] public bool longRange;
    [HideInInspector] public bool piercing;
    [HideInInspector] public int damage = 1;

    private Rigidbody2D rb;
    
    private float timer;

    void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        timer = 0f;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + ((Vector2)transform.up * speed * Time.deltaTime));
        if(!longRange)
            timer += Time.deltaTime;
        if (timer >= airTime)
            Destroy(gameObject);
    }

    //collision with walls
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        Destroy(gameObject);
    }

    public void BulletFX(Vector3 point)
    {
        Instantiate(prefabFx, point, Quaternion.identity);
    }
}
