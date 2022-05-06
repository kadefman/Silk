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
    public Scene templateScene;

    private void Start()
    {       
        if (templateScene == Scene.kadefSample)
        {
            rooms.Add(new Room());
            rooms.Add(new Room(3, eDoor));
            rooms.Add(new Room());
            rooms.Add(new Room(10, null, key, keyDoor));
            rooms.Add(new Room());
        }

        else if(templateScene == Scene.aquaMockup)
            rooms.Add(new Room(4, eDoor));

        GameManager.instance.rooms = rooms;
        GameManager.instance.SetRoom(0);
    }
}
