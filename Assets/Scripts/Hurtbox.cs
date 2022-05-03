using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    private int damage;

    void Awake()
    {
        damage = transform.parent.GetComponent<FlyingEnemy>().damage;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.gameObject.GetComponent<Player>().AddHealth(-damage);

        }
    }
}
