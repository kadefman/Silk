using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entry : MonoBehaviour
{
    public int roomNumber;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Found someone!");
        if (collision.transform.CompareTag("Player"))
        {
            GameManager.instance.SetRoom(roomNumber);
            Destroy(gameObject);
        }                    
    }
}
