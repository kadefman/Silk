using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Silk, Health, Key, Pierce, Big, Triple, FastSpin, Speed, Range, Currency };
 
    public Type type;   
    public int value;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            //FindObjectOfType<AudioManager>().Play("Pickup");
            Player playerScript = collider.transform.GetComponent<Player>();
           
            switch(type)
            {
                case Type.Silk:
                    collider.transform.GetComponent<Player>().AddSilk(value);
                    FindObjectOfType<AudioManager>().Play("Pickup");
                    break;

                case Type.Health:
                    collider.transform.GetComponent<Player>().AddHealth(value);
                    FindObjectOfType<AudioManager>().Play("Pickup");
                    break;

                case Type.Key:
                    GameManager.instance.OpenDoor(true);
                    FindObjectOfType<AudioManager>().Play("Pickup");
                    break;

                case Type.Pierce:
                    playerScript.piercing = true;
                    FindObjectOfType<AudioManager>().Play("Pickup 2");
                    break;

                case Type.Big:
                    playerScript.bigBullets = true;
                    FindObjectOfType<AudioManager>().Play("Pickup 2");
                    break;

                case Type.Triple:
                    playerScript.tripleShot = true;
                    FindObjectOfType<AudioManager>().Play("Pickup 2");
                    break;

                case Type.FastSpin:
                    playerScript.spinTime = 1.3f;
                    FindObjectOfType<AudioManager>().Play("Pickup 2");
                    break;

                case Type.Speed:
                    playerScript.defaultSpeed = 3;
                    FindObjectOfType<AudioManager>().Play("Pickup 2");
                    playerScript.speed = 3f;
                    break;

                case Type.Range:
                    playerScript.longRange = true;
                    FindObjectOfType<AudioManager>().Play("Pickup 2");
                    break;

                case Type.Currency:
                    GameManager.instance.AddCurrency(value);
                    if (value == 3)
                    {
                        FindObjectOfType<AudioManager>().Play("Big Currency");
                    }
                    else
                        FindObjectOfType<AudioManager>().Play("Currency");
                    break;
            }

            Destroy(gameObject);
        }
    }
}
