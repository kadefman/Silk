using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public float displayTime;

    private FlyingEnemy enemy;
    private int damage;
    public AudioSource audioSource;
    public AudioClip damageSFX;
    //public AudioClip deathSFX;

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
            //audioSource.pitch = Random.Range(.95f, 1.1f);
           // audioSource.PlayOneShot(damageSFX, .8f);
            enemy.AddHealth(collider.transform.GetComponent<Bullet>().damage*-1);
            if (!collider.transform.GetComponent<Bullet>().piercing)
                Destroy(collider.gameObject);
            StartCoroutine(ShowDamage());
        }
    }

    private IEnumerator ShowDamage()
    {
        transform.GetComponent<SpriteRenderer>().enabled = true;
        audioSource.pitch = Random.Range(.95f, 1.1f);
        audioSource.PlayOneShot(damageSFX, .8f);
        float timer = displayTime;
        while(timer >= float.Epsilon)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        transform.GetComponent<SpriteRenderer>().enabled = false;
    }
}
