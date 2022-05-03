using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        player = GameObject.Find("Spider");
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            //Debug.Log("item");
            GameManager.instance.AddSilk(50);
            Destroy(gameObject);
        }
    }
}
