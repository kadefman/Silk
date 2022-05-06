using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{   
    public enum Shape { Rect, Diamond, Circle, Hall, Other }

    public Shape shape;
    public Transform enemyDoor;
    public Transform magicKey;
    public Transform keyDoor;
    public int enemyCount;
    public int itemCount;

    //int eDoorCount, int kDoorCount?
    //the items and enemies arguments will NOT BE OPTIONAL FOREVER
    public Room(Shape roomShape, int enemies = 0, int items= 0)
    {
        shape = roomShape;
        enemyCount = enemies;
        itemCount = items;
    }

    //basic overload for template levels (not for use with generator)
    public Room(int enemies = 0, Transform eDoor = null, Transform key = null, Transform kDoor = null)
    {
        enemyCount = enemies;
        enemyDoor = eDoor;
        magicKey = key;
        keyDoor = kDoor;
    }
}
