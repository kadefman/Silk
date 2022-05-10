using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{   
    //fork, symmRect (oscillating, maybe for hallway), rect in other 2 angles?, start Room
    public enum Shape { Diamond, Hexagon, Triangle, Hall, Random, Rect, SymmRect, Fork }
    public enum TravelDirection { U, UR, DR, D, DL, UL }
    public enum WallDirection { UL, UR, R, DR, DL, L}

    public Shape shape;
    public TravelDirection entranceDir;   
    public WallDirection entranceWall;
    public Vector2 entrancePoint;
    public TravelDirection exitDir;
    public WallDirection exitWall;
    public Vector2 exitPoint;
    public Transform enemyDoor;
    public Transform magicKey;
    public Transform keyDoor;      
    public int enemyCount;
    public int itemCount;

    //int eDoorCount, int kDoorCount?
    //the items and enemies arguments will NOT BE OPTIONAL FOREVER
    public Room(Shape roomShape, TravelDirection enterDir, WallDirection enterWall, int enemies = 0, int items= 0)
    {
        shape = roomShape;
        entranceDir = enterDir;
        entranceWall = enterWall;
        enemyCount = enemies;
        itemCount = items;
    }

    //starting room!
    public Room(Shape roomShape)
    {
        shape = roomShape;
        //do I even need anything else here?
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
