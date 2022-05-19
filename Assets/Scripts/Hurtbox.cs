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

    private void OnEnable()
    {
        //damage = origDamage;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.name + "hitting" + transform.name);
        if (collider.gameObject.CompareTag("BossPlat"))
            return;

        if (collider.gameObject.CompareTag("Player"))
        {
            //Debug.Log("take " + damage + " damage");
            Player player = collider.gameObject.GetComponent<Player>();

            if (player.invincible)
            {
                //Debug.Log("you should not take damage");
                return;
            }
                

            player.AddHealth(-damage);
            if (player.spinning)
                player.spinning = false;

            player.Invincible();
        }

        else if(collider.gameObject.CompareTag("Bullet"))
        {                     
            
            if (!collider.transform.GetComponent<Bullet>().piercing) 
            {
                Destroy(collider.gameObject);
                collider.gameObject.GetComponent<Bullet>().BulletHit(transform.position);
            }

            //tongue getting hit
            if(transform.CompareTag("Tongue"))
                return;

            //frog getting hit
            if(transform.CompareTag("Frog"))
            {
                Frog boss = transform.parent.GetComponent<Frog>();
                boss.health -= collider.transform.GetComponent<Bullet>().damage;
                if (boss.health <= 0)
                {
                    boss.StartCoroutine("Die");
                }                    
            }
               
            else
                enemy.GetHit(collider.transform.GetComponent<Bullet>().damage);
        }
    }
}
