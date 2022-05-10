using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Silk, Health, Key };
 
    public Type type;   
    public int value;
    


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Play("Pickup");
            if (type == Type.Silk)
            {
                value = 20;
                collider.transform.GetComponent<Player>().AddSilk(value);
            }
                

            else if (type == Type.Health)
            {
                value = 10;
                collider.transform.GetComponent<Player>().AddHealth(value);
            }
                

            else if (type == Type.Key)
                GameManager.instance.OpenDoor(true);

            Destroy(gameObject);
        }
    }
}
