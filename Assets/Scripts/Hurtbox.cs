using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{    
    private int damage;
    private bool isEnemy;

    //An object with this script can be a hurtbox or a hitbox (often both, in the case of a standard enemy)

    void Awake()
    {
        if (transform.parent.GetComponent<FlyingEnemy>())
        {
            isEnemy = true;
            FlyingEnemy enemy = transform.parent.GetComponent<FlyingEnemy>();
            damage = enemy.damage;
        }

        //A boss is an enemy, and will take damage
        else if (transform.CompareTag("Frog"))
            isEnemy = true;

        //non enemy hitbox (ex. frog tongue)
        else
        {
            damage = 1;
            isEnemy = false;
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log(collider.gameObject.name + "hitting" + transform.name);

        //hitbox hitting player
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


        //bullet hitting hurtbox
        else if(collider.gameObject.CompareTag("Bullet"))
        {          
            collider.gameObject.GetComponent<Bullet>().BulletFX(transform.position);

            if (!collider.transform.GetComponent<Bullet>().piercing) 
            {
                Destroy(collider.gameObject);                
            }

            //non enemies will not take damage
            if(!isEnemy)
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
               
            //regular enemy getting hit
            else
            {
                FlyingEnemy enemy = transform.parent.GetComponent<FlyingEnemy>();
                enemy.GetHit(collider.transform.GetComponent<Bullet>().damage);
            }
                
        }
    }
}
