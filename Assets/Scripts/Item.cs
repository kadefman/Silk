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
            FindObjectOfType<AudioManager>().Play("Pickup");
            Player playerScript = collider.transform.GetComponent<Player>();
           
            switch(type)
            {
                case Type.Silk:
                    collider.transform.GetComponent<Player>().AddSilk(value);
                    break;

                case Type.Health:
                    collider.transform.GetComponent<Player>().AddHealth(value);
                    break;

                case Type.Key:
                    GameManager.instance.OpenDoor(true);
                    break;

                case Type.Pierce:
                    playerScript.piercing = true;
                    break;

                case Type.Big:
                    playerScript.bigBullets = true;
                    break;

                case Type.Triple:
                    playerScript.tripleShot = true;
                    break;

                case Type.FastSpin:
                    playerScript.spinTime = 1.3f;
                    break;

                case Type.Speed:
                    playerScript.defaultSpeed = 3;
                    playerScript.speed = 3f;
                    break;

                case Type.Range:
                    playerScript.longRange = true;
                    break;

                case Type.Currency:
                    GameManager.instance.AddCurrency(1);
                    break;
            }

            Destroy(gameObject);
        }
    }
}
