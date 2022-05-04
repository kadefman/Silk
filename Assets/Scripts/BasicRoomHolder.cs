using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoomHolder : MonoBehaviour
{
    public static List<Room> rooms = new List<Room>();
    public Transform key;
    public Transform keyDoor;
    public Transform eDoor;

    private void Start()
    {
        rooms.Add(new Room(0,0));
        rooms.Add(new Room(3, 1, eDoor));
        rooms.Add(new Room(0,2));
        rooms.Add(new Room(10, 3, null, key, keyDoor));
        rooms.Add(new Room(0,4));
        GameManager.instance.rooms = rooms;
    }
}
