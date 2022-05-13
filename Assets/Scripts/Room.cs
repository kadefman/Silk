using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{   
    //fork, symmRect (oscillating, maybe for hallway), rect in other 2 angles?, start Room
    public enum Shape { Diamond, Hexagon, Triangle, Hall, Random, Rect, SymmRect, Fork, Special }
    public enum TravelDirection { U, UR, DR, D, DL, UL }
    public enum WallDirection { UL, UR, R, DR, DL, L}

    public static int maxDifficulty = 5;
    public static float[] webRatios = new float[] { .1f, .2f, .3f, .5f, .7f };
    public static int[] minEnemies = new int[] { 1, 1, 2, 2, 3 };
    public static int[] maxEnemies = new int[] { 3, 5, 6, 6, 6 };

    //each small array must add to 1
    //5 difficulties
    //7 enemies: stillFly, movingFly, bee, angryBee, fastFly, bigBee, bigAngryBee
    public static float[,] enemyRarities = new float[5, 7] 
    {{.5f, .4f, .1f, 0, 0, 0, 0}, 
    {.3f,.4f,.3f, 0, 0, 0, 0},
    {0, .2f, .5f, .3f, 0, 0, 0}, 
    {0, 0, 0, .4f, .3f, .2f, 0}, 
    {0, 0, 0, .2f, .1f, .4f, .3f}};  

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
    public int difficulty;

    //int eDoorCount, int kDoorCount?
    //the items and enemies arguments will NOT BE OPTIONAL FOREVER
    public Room(Shape roomShape, TravelDirection enterDir, WallDirection enterWall)
    {
        shape = roomShape;
        entranceDir = enterDir;
        entranceWall = enterWall;
    }

    //starting room!
    public Room(Shape roomShape)
    {
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
