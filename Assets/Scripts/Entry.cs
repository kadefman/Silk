using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entry : MonoBehaviour
{
    public int roomNumber;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            GameManager.instance.SetRoom(roomNumber);
            transform.parent.parent.GetComponent<PolygonCollider2D>().enabled = false;
            Debug.Log(GameManager.instance.bossRoomIndex);


            Vector3 dropOffset;
            Vector3 ent = GameManager.instance.currentRoom.entrancePoint;

            if (GameManager.instance.roomIndex == 1)
            {
                dropOffset = new Vector3(-.96f, -.56f, 0);              
                Instantiate(GameManager.instance.invisWall, ent + dropOffset, Quaternion.identity);
                Destroy(transform.parent.gameObject);
            }

            else if (GameManager.instance.roomIndex == GameManager.instance.bossRoomIndex)
            {
                dropOffset = Vector3.down * 1.12f;
                Instantiate(GameManager.instance.invisWall, ent + dropOffset, Quaternion.identity);
                Destroy(transform.parent.gameObject);
            }
        }                    
    }
}
