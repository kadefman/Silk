using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoomHolder : MonoBehaviour
{
    public enum Scene { aquaMockup, kadefSample}

    public static List<Room> rooms = new List<Room>();
    public Transform key;
    public Transform keyDoor;
    public Transform eDoor;
    public Scene scene;

    private void Start()
    {       
        if (scene == Scene.kadefSample)
        {
            rooms.Add(new Room(0, 0));
            rooms.Add(new Room(3, 1, eDoor));
            rooms.Add(new Room(0, 2));
            rooms.Add(new Room(10, 3, null, key, keyDoor));
            rooms.Add(new Room(0, 4));
        }

        else
            rooms.Add(new Room(4, 0, eDoor));

        GameManager.instance.rooms = rooms;
        GameManager.instance.SetRoom(0);
    }
}
