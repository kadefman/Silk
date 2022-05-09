using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Silk, Health, Key };
    //public Color[] colors;
    public Type type;
    public int value;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (type == Type.Silk)
                collider.transform.GetComponent<Player>().AddSilk(value);

            else if (type == Type.Health)
                collider.transform.GetComponent<Player>().AddHealth(value);

            else if (type == Type.Key)
                GameManager.instance.OpenDoor(true);

            Destroy(gameObject);
        }
    }
}
