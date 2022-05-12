using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{    
    private FlyingEnemy enemy;  
    
    public float displayTime;
    private int damage;
    

    void Awake()
    {
        enemy = transform.parent.GetComponent<FlyingEnemy>();
        damage = enemy.damage;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Player player = collider.gameObject.GetComponent<Player>();
            player.AddHealth(-damage);
            if (player.spinning)
                player.spinning = false; 
        }

        else if(collider.gameObject.CompareTag("Bullet"))
        {
            
            enemy.GetHit(collider.transform.GetComponent<Bullet>().damage);
            if (!collider.transform.GetComponent<Bullet>().piercing) 
            {
                Destroy(collider.gameObject);
                collider.gameObject.GetComponent<Bullet>().BulletHit(transform.position);
            }
        }
    }
}
