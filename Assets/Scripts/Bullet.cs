using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;
    public bool piercing;
    public GameObject prefabFx;

    private Rigidbody2D rb;

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.MovePosition(rb.position + ((Vector2)transform.up * speed * Time.deltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Destroy(gameObject);
    }

    public void BulletHit(Vector3 point)
    {
        Instantiate(prefabFx, point, Quaternion.identity);
    }
}
