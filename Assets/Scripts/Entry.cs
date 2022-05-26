using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entry : MonoBehaviour
{
    public static bool shopLocked = false;

    public int roomNumber;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {          
            GameManager.instance.SetRoom(roomNumber);
            transform.parent.parent.GetComponent<PolygonCollider2D>().enabled = false;
            Debug.Log(GameManager.instance.bossRoomIndex);

            Vector3 dropOffset;
            Vector3 ent;

            Debug.Log("entering room " + GameManager.instance.roomIndex);

            if (GameManager.instance.roomIndex == 1 && !shopLocked)
            {
                ent = GameManager.instance.currentRoom.entrancePoint;
                dropOffset = new Vector3(-.96f, -.56f, 0);              
                Instantiate(GameManager.instance.invisWall, ent + dropOffset, Quaternion.identity);
                Destroy(transform.parent.parent.gameObject);
                shopLocked = true;
            }

            //we may need a bool for this if we go past the boss room
            else if (GameManager.instance.roomIndex == GameManager.instance.bossRoomIndex)
            {
                ent = GameManager.instance.rooms[GameManager.instance.roomIndex - 1].exitPoint;
                dropOffset = Vector3.down * 1.12f;
                Instantiate(GameManager.instance.invisWall, ent + dropOffset, Quaternion.identity);
                Destroy(transform.parent.parent.gameObject);
            }
        }                    
    }
}
