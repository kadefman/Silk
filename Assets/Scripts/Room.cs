using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{   
    public Transform enemyDoor;
    public Transform magicKey;
    public Transform keyDoor;
    public int index;
    public int enemyCount;

    public Room(int enemies = 0, int ind = 0, Transform eDoor = null, Transform key = null, Transform kDoor = null)
    {
        enemyCount = enemies;
        enemyDoor = eDoor;
        magicKey = key;
        keyDoor = kDoor;
        index = ind;
    }
}
