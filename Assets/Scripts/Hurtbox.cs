using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{    
    private FlyingEnemy enemy;  
    
    private int damage;
    
    void Awake()
    {
        if (transform.parent.GetComponent<FlyingEnemy>())
        {
            enemy = transform.parent.GetComponent<FlyingEnemy>();
            damage = enemy.damage;
        }

        //tongue
        else
            damage = 1;       
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
            if (!collider.transform.GetComponent<Bullet>().piercing) 
            {
                Destroy(collider.gameObject);
                collider.gameObject.GetComponent<Bullet>().BulletHit(transform.position);
            }

            if(transform.CompareTag("Frog"))
            {
                Frog boss = transform.parent.GetComponent<Frog>();
                boss.health -= collider.transform.GetComponent<Bullet>().damage;
                if (boss.health <= 0)
                    boss.Die();
            }
               
            else
                enemy.GetHit(collider.transform.GetComponent<Bullet>().damage);
        }
    }
}
