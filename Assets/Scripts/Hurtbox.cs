using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{    
    private FlyingEnemy enemy;  
    
    public int damage;
    public int origDamage;
    
    void Awake()
    {
        origDamage = damage;
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
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("take " + damage + " damage");
            Player player = collider.gameObject.GetComponent<Player>();
            player.AddHealth(-damage);
            if (player.spinning)
                player.spinning = false;

            TempDisable();

            /*if(transform.CompareTag("Tongue"))
            {
                Frog boss = transform.parent.parent.parent.GetComponent<Frog>();
            }*/
        }

        else if(collider.gameObject.CompareTag("Bullet"))
        {                     
            if (!collider.transform.GetComponent<Bullet>().piercing) 
            {
                Destroy(collider.gameObject);
                collider.gameObject.GetComponent<Bullet>().BulletHit(transform.position);
            }

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

    void TempDisable()
    {
        //damage = 0;
        //Invoke("EnableHitbox", .7f);
    }

    void EnableHitbox()
    {
        damage = origDamage;
    }
}
