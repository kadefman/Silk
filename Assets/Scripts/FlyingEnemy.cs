using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public enum Type {Fly, Bee};
    public Type type;
    //public Color[] colors;

    public float speed;
    public int damage;
    public int health;
    public int silkReward;

    private Collider2D coll;
    private Rigidbody2D rb;

    public Transform sprite; 

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        coll = transform.GetComponent<Collider2D>();
        Vector2 randVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForce(speed * randVector.normalized);
    }
    void Update()
    {
        if(sprite != null) {
            var lookAtPoint = sprite.position+ new Vector3(rb.velocity.x, rb.velocity.y, 0);
            lookAtPoint.z = sprite.position.z;
            //sprite.LookAt(lookAtPoint);
            // sprite.LookAt(sprite.position + new Vector3 (rb.velocity.x, rb.velocity.y,0));
            float angle = 0;

            Vector3 relative = transform.InverseTransformPoint(lookAtPoint);
            angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            transform.Rotate(0, 0, -angle);
        }
        
    }

    public void AddHealth(int i)
    {
        health += i;
        if (health <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.AddSilk(silkReward);
            GameManager.instance.enemyCount--;
            if (GameManager.instance.enemyCount == 0)
                GameManager.instance.OpenDoor(false);
        }           
    }
}
