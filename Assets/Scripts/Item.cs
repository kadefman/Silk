using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Silk, Health };
    public Color[] colors;
    public Type type;
    

    void Start()
    { 

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(type == Type.Silk)
                collider.transform.GetComponent<Player>().AddSilk(50);

            else if(type == Type.Health)
                collider.transform.GetComponent<Player>().AddHealth(2);

            Destroy(gameObject);
        }
    }
}
