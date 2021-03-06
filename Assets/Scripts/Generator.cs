using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public static float xOffset = .96f;
    public static float yOffset = 1.12f;

    public List<Room> rooms;
    public GameObject[] tiles;
    public GameObject[] enemies;
    public GameObject outerWall;
    public GameObject innerWall;
    public GameObject entranceMark;
    public GameObject genMark;
    public GameObject player;
    public GameObject cam;
    public GameObject startRoom;
    public GameObject bossRoom;

    public int roomCount;
    public bool autoBoss;
    public int[] difficultyStarts;
    public int minHallSize;
    public int maxHallSize;
    public int minRoomSize;
    public int maxRoomSize;
    
    //diff curve stuff
    public int minEnemies;
    public int maxEnemies;    
    public int difficulty;
    public float webRatio;
    public float[] enemyRates;

    private Room thisRoom;
    private Room prevRoom;
    private GameObject roomObject;
    private List<GameObject> roomObjects;

    private List<Vector2> possibleExits;    
    private List<Vector2> tilePoints;
    private List<Vector2> wallPoints;
    private List<Vector2> inWallPoints;
    private List<Vector2> enemyPoints;

    private Vector2 playerPos = new Vector2(-.96f, -7.28f);
    private Vector2 rayOffset = new Vector2(0f, 0.1f);

    private int currentIndex = 0;
    private int maxWalls;
    private int bossRoomIndex;
    private int collisionCount;
    
    void Start()
    {       
        //need even no. of rooms - number will round down. even rooms are safe except boss.
        if (roomCount < 4 || autoBoss)
            roomCount = 4;

        if(roomCount%2==1)
            roomCount--;

        bossRoomIndex = roomCount - 1;
        GameManager.instance.bossRoomIndex = bossRoomIndex;

        currentIndex = 0;
        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        player.transform.position = playerPos;
        CreateNextRoom();
        cam.transform.parent = player.transform;  
        GameManager.instance.rooms = rooms;
        collisionCount = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ClearLevel();
            CreateNextRoom();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CollisionTest();           
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            ClearLevel();
        }
    }

    void ClearLevel()
    {
        currentIndex = 0;
        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    //where do I do the startRoom thing? Address this in Room script too
    void CreateNextRoom()
    {
        Debug.Log("Creating room " + currentIndex + " ------------");
        GameManager.instance.generating = true;

        if (currentIndex == 0)
            CreateStartRoom();

        else if (currentIndex == bossRoomIndex)
            TryCreateBossRoom();

        else
        {
            PrepareRoom();
            ChooseExits(currentIndex);

            int height;
            int length;
            if(thisRoom.shape == Room.Shape.Hall)
            {
                height = Random.Range(minHallSize, maxHallSize);
                length = Random.Range(minHallSize, maxHallSize);                
            }
            else
            {
                height = Random.Range(minRoomSize, maxRoomSize);
                length = Random.Range(minRoomSize, maxRoomSize);
            }

            Vector2 genPoint = ChooseGenOffset(height, length);

            BuildShape(height, length, genPoint);

            //index was brought back 2, try again
            if (CollisionTest())
            {
                Invoke("Redo", .05f);
                return;
            }

            SetDifficulty();
            PlaceTiles();
            AddEnemies();
        }

        currentIndex++;

        if (roomObjects.Count < roomCount)
            CreateNextRoom();
        else
            GameManager.instance.generating = false;
    }

    void CreateStartRoom()
    {
        roomObject = Instantiate(startRoom, Vector2.zero, Quaternion.identity, transform);
        roomObjects.Add(roomObject);
        Room thisRoom = new Room(Room.Shape.Special);
        rooms.Add(thisRoom);
        thisRoom.exitDir = Room.TravelDirection.UR;
        thisRoom.exitWall = Room.WallDirection.UR;
        thisRoom.exitPoint = Vector2.zero;
        thisRoom.entrancePoint = new Vector2(0, -1);

        //activate tile rotations
        foreach (Transform t in roomObject.transform)
        {
            HexTile hexScript = t.GetComponent<HexTile>();
            if (hexScript != null)
                hexScript.ChooseSprite(hexScript.type);
        }
    }

    void TryCreateBossRoom()
    {
        int length = 4;
        Vector2 genPoint = thisRoom.exitPoint + 4 * new Vector2(-.96f, .56f);
        for (int x = 0; x <= 8; x++)
        {
            int lineHeight;
            if (x <= length)
                lineHeight = x + length + 1;
            else
                lineHeight = 3 * length + 1 - x;

            for (int y = 0; y < lineHeight; y++)
            {
                float pointX = genPoint.x + xOffset * x;
                float pointY;
                if (x < length)
                    pointY = genPoint.y + (yOffset * (y - .5f * x));
                else
                    pointY = genPoint.y + (yOffset * (1 + y - .5f * (2 * length + 2 - x)));

                Vector2 point = new Vector2(pointX, pointY);
                if (!(y == 0 & x == length))
                {
                    tilePoints.Add(point);
                }
            }
        }

        foreach (Vector2 point in tilePoints)
        {
            RaycastHit2D hit = Physics2D.Linecast(point, point + rayOffset, LayerMask.GetMask("Tall"));

            //go back 2 rooms, try again
            if (hit.transform != null && collisionCount<50)
            {
                Debug.Log("COLLISION! Can't make room " + currentIndex);
                Debug.Log("COLLISION AT " + hit.transform.position);
                //collisionCount++;

                Destroy(roomObjects[roomObjects.Count - 1]);
                Destroy(roomObjects[roomObjects.Count - 2]);
                Destroy(roomObjects[roomObjects.Count - 3]);
                Destroy(roomObjects[roomObjects.Count - 4]);
                roomObjects.RemoveRange(roomObjects.Count - 4, 4);
                rooms.RemoveRange(rooms.Count - 5, 5);
                currentIndex -= 4;
                Invoke("Redo", .05f);
                return;
            }
        }
        //Debug.Log(collisionCount + " collisions");
        CreateBossRoom();
    }

    void CreateBossRoom()
    {
        prevRoom = rooms[currentIndex - 1];
        roomObject = Instantiate(bossRoom, prevRoom.exitPoint, Quaternion.identity, transform);
        roomObjects.Add(roomObject);
        Room thisRoom = new Room(Room.Shape.Special);
        rooms.Add(thisRoom);

        //activate tile rotations
        foreach (Transform t in roomObject.transform)
        {
            HexTile hexScript = t.GetComponent<HexTile>();
            if (hexScript != null)
                hexScript.ChooseSprite(hexScript.type);
        }

        GameObject ent = Instantiate(entranceMark, prevRoom.exitPoint, Quaternion.identity, roomObject.transform);
        for (int i = 0; i < 3; i++)
            ent.transform.GetChild(0).GetChild(i).GetComponent<Entry>().roomNumber = currentIndex;
        for (int i = 3; i < 6; i++)
            ent.transform.GetChild(0).GetChild(i).GetComponent<Entry>().roomNumber = currentIndex - 1;

        //case U from main method
        ent.transform.GetChild(0).Rotate(Vector3.back, -60);
        Debug.Log($"Finished! {collisionCount} collisions");
    }

    void Redo()
    {
        collisionCount++;
        CreateNextRoom();
    }

    private void PrepareRoom()
    {
         //take some stuff from chooseExits?
        tilePoints = new List<Vector2>();
        wallPoints = new List<Vector2>();
        inWallPoints = new List<Vector2>();
        enemyPoints = new List<Vector2>();
        possibleExits = new List<Vector2>();
    }

    private bool CollisionTest()
    {
        //collisionCount = 0;
        List<Vector2> spotsToCheck = new List<Vector2>(tilePoints);
        spotsToCheck.AddRange(wallPoints);

        foreach (Vector2 point in spotsToCheck)
        {
            RaycastHit2D hit = Physics2D.Linecast(point, point + rayOffset, LayerMask.GetMask("Tall"));

            //go back 2 rooms, try again
            if (hit.transform != null && collisionCount<50)
            {
                Debug.Log("COLLISION! Can't make room " + currentIndex);
                //Debug.Log("COLLISION AT " + hit.transform.position);

                Destroy(roomObjects[roomObjects.Count - 1]);
                Destroy(roomObjects[roomObjects.Count - 2]);
                roomObjects.RemoveRange(roomObjects.Count - 2, 2);
                rooms.RemoveRange(rooms.Count - 3, 3);
                currentIndex -= 2;
                return true;
            }
        }
        //Debug.Log(collisionCount + " collisions");
        return false;
    }

    private void ChooseExits(int roomIndex)
    {
        //set up entrances
        prevRoom = rooms[currentIndex - 1];
        int newEntranceNum = ((int)prevRoom.exitDir + 3) % 6;
        Room.TravelDirection newEntrance = (Room.TravelDirection)newEntranceNum;
        int newWallNum = ((int)prevRoom.exitWall + 3) % 6;
        Room.WallDirection enterWall = (Room.WallDirection)newWallNum;
        //Debug.Log("coming from wallDir" + (int)enterWall);

        //pick exits:
        //hall, auto exit direction
        if (roomIndex % 2 == 0)
        {
            thisRoom = new Room(Room.Shape.Hall, newEntrance, enterWall);
            thisRoom.exitDir = prevRoom.exitDir;
            thisRoom.exitWall = prevRoom.exitWall;
        }

        else if(roomIndex == bossRoomIndex - 2)
        {
            //this should always be valid
            thisRoom = new Room(Room.Shape.Hexagon, newEntrance, enterWall);
            thisRoom.exitDir = Room.TravelDirection.U;
            thisRoom.exitWall = Room.WallDirection.UL;
        }

        //hex, 1 of 5 possible exit directions, 1 of 5 exit walls
        else
        {
            thisRoom = new Room(Room.Shape.Hexagon, newEntrance, enterWall);
            int rand;

            //prepare for boss room - exitDir is only UL U or UR
            if (roomIndex == bossRoomIndex - 4)
                rand = Random.Range(5, 8) % 6;
            else
                rand = Random.Range(0, 6);

            int exitInt = rand;
            if (exitInt == newEntranceNum)
                exitInt = newEntranceNum == 5 ? 0 : newEntranceNum + 1;
            thisRoom.exitDir = (Room.TravelDirection)exitInt;

            //choose wall
            rand = Random.Range(0, 2);
            int exitWallInt = (exitInt + rand) % 6;
            if (exitWallInt == newWallNum)
            {
                rand = (rand + 1) % 2;
                exitWallInt = (exitInt + rand) % 6;
            }

            thisRoom.exitWall = (Room.WallDirection)exitWallInt;
        }

        thisRoom.entrancePoint = prevRoom.exitPoint;
        rooms.Add(thisRoom);
    }

    private Vector2 ChooseGenOffset(int height, int length)
    {
        Vector2 genOffset = Vector2.zero;   

        switch(thisRoom.shape)
        {
            case Room.Shape.Hall:
                switch(thisRoom.exitDir)
                {
                    case Room.TravelDirection.U:
                        genOffset = new Vector2(0, yOffset);
                        break;

                    case Room.TravelDirection.UR:
                        genOffset = new Vector2(xOffset, .5f * yOffset);
                        break;

                    case Room.TravelDirection.DR:
                        genOffset = new Vector2(xOffset, .5f * -yOffset);
                        break;

                    case Room.TravelDirection.D:
                        genOffset = new Vector2(0, -yOffset);
                        break;

                    case Room.TravelDirection.DL:
                        genOffset = new Vector2(-xOffset, .5f * -yOffset);
                        break;

                    case Room.TravelDirection.UL:
                        genOffset = new Vector2(-xOffset, .5f * yOffset);
                        break;

                }
                break;

            case Room.Shape.Hexagon:
                if (length < 2)
                    length = 2;

                int randOffset = Random.Range(1, length);
                //Debug.Log("Hex randOffset " + randOffset);
                //Debug.Log("coming from wallDir" + (int)thisRoom.entranceWall);

                //choose genPoint from which the room is built
                switch (thisRoom.entranceWall)
                {
                    case Room.WallDirection.L:
                        Debug.Log("coming from L");
                        genOffset = new Vector2(0, -randOffset * yOffset);
                        break;

                    case Room.WallDirection.UL:
                        Debug.Log("coming from UL");
                        genOffset = new Vector2(-randOffset * xOffset, -yOffset * (length + .5f * randOffset));
                        break;

                    case Room.WallDirection.DL:
                        Debug.Log("coming from DL");
                        genOffset = new Vector2(-randOffset * xOffset, randOffset * .5f * yOffset);
                        break;

                    case Room.WallDirection.R:
                        Debug.Log("coming from R");
                        genOffset = new Vector2(-2 * length * xOffset, -randOffset * yOffset);
                        break;

                    case Room.WallDirection.UR:
                        Debug.Log("coming from UR");
                        genOffset = new Vector2(-xOffset * (randOffset + length), -yOffset * (length + .5f * (length - randOffset)));
                        break;


                    case Room.WallDirection.DR:
                        Debug.Log("coming from DR");
                        genOffset = new Vector2(-xOffset * (randOffset + length), yOffset * .5f * (length - randOffset));
                        break;

                    default:
                        genOffset = Vector2.zero;
                        Debug.Log("You didn't pick a valid entrance direction");
                        break;
                }
                break;

            case Room.Shape.Diamond:

                break;

            case Room.Shape.Triangle:

                break;

            case Room.Shape.Rect:

                break;

            default:

                break;
        }
        return genOffset;
    }

    private void BuildShape(int height, int length, Vector2 genOffset)
    {
        //Debug.Log("genOffset " + genOffset + "entrancePoint " + thisRoom.entrancePoint);
        Vector2 genPoint = genOffset + thisRoom.entrancePoint;
        //Instantiate(genMark, genPoint, Quaternion.identity);
        int orientation;
        
        switch (thisRoom.shape)
        {
            case Room.Shape.Hall:
                Debug.Log("Hall, length " + length);
                maxWalls = 0;

                if (length < 3)
                    length = 3;

                for (int i = 0; i <= length; i++)
                {
                    Vector2 thisPoint = genPoint + i * genOffset;
                    if (i < length)
                        tilePoints.Add(thisPoint);
                    else
                    {
                        //exit needs to be a "wall point"
                        wallPoints.Add(thisPoint);
                        possibleExits.Add(thisPoint);
                        break;
                    }

                    switch (thisRoom.exitDir)
                    {
                        case Room.TravelDirection.U:
                            wallPoints.Add(thisPoint + new Vector2(-xOffset, yOffset * (thisRoom.exitWall == Room.WallDirection.UL ? -.5f : .5f)));
                            wallPoints.Add(thisPoint + new Vector2(xOffset, yOffset * (thisRoom.exitWall == Room.WallDirection.UL ? .5f : -.5f)));
                            break;

                        case Room.TravelDirection.UR:
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.R ? yOffset * Vector2.up : new Vector2(-xOffset, .5f * yOffset)));
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.R ? yOffset * Vector2.down : new Vector2(xOffset, -.5f * yOffset)));
                            break;

                        case Room.TravelDirection.DR:
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.R ? yOffset * Vector2.up : new Vector2(xOffset, .5f * yOffset)));
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.R ? yOffset * Vector2.down : new Vector2(-xOffset, -.5f * yOffset)));
                            break;

                        case Room.TravelDirection.D:
                            wallPoints.Add(thisPoint + new Vector2(-xOffset, yOffset * (thisRoom.exitWall == Room.WallDirection.DR ? -.5f : .5f)));
                            wallPoints.Add(thisPoint + new Vector2(xOffset, yOffset * (thisRoom.exitWall == Room.WallDirection.DR ? .5f : -.5f)));
                            break;

                        case Room.TravelDirection.DL:
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.L ? yOffset * Vector2.up : new Vector2(-xOffset, .5f * yOffset)));
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.L ? yOffset * Vector2.down : new Vector2(xOffset, -.5f * yOffset)));
                            break;

                        case Room.TravelDirection.UL:
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.L ? yOffset * Vector2.up : new Vector2(xOffset, .5f * yOffset)));
                            wallPoints.Add(thisPoint + (thisRoom.exitWall == Room.WallDirection.L ? yOffset * Vector2.down : new Vector2(-xOffset, -.5f * yOffset)));
                            break;
                    }
                }
                break;

            case Room.Shape.Hexagon:
                Debug.Log("Hexagon, side " + length);
                maxWalls = length-1;
                for (int x = 0; x <= 2 * length; x++)
                {
                    int lineHeight;
                    if (x <= length)
                        lineHeight = x + length + 1;
                    else
                        lineHeight = 3 * length + 1 - x;

                    for (int y = 0; y < lineHeight; y++)
                    {
                        float pointX = genPoint.x + xOffset * x;
                        float pointY;
                        if (x < length)
                            pointY = genPoint.y + (yOffset * (y - .5f * x));
                        else
                            pointY = genPoint.y + (yOffset * (1 + y - .5f * (2 * length + 2 - x)));

                        Vector2 thisPoint = new Vector2(pointX, pointY);

                        //We'll handle corners another time
                        if (x == 0)
                        {
                            wallPoints.Add(thisPoint);
                            if (thisRoom.exitWall == Room.WallDirection.L && y != 0 && y != lineHeight - 1)
                                possibleExits.Add(thisPoint);
                        }

                        else if (x == 2 * length)
                        {
                            wallPoints.Add(thisPoint);
                            if (thisRoom.exitWall == Room.WallDirection.R && y != 0 && y != lineHeight - 1)
                                possibleExits.Add(thisPoint);
                        }

                        else if (y == 0)
                        {
                            wallPoints.Add(thisPoint);
                            if (thisRoom.exitWall == Room.WallDirection.DL && x < length && x != 0)
                                possibleExits.Add(thisPoint);

                            else if (thisRoom.exitWall == Room.WallDirection.DR && x > length && x != 2 * length)
                                possibleExits.Add(thisPoint);
                        }

                        else if (y == lineHeight - 1)
                        {
                            wallPoints.Add(thisPoint);
                            if (thisRoom.exitWall == Room.WallDirection.UL && x < length && x != 0)
                                possibleExits.Add(thisPoint);

                            else if (thisRoom.exitWall == Room.WallDirection.UR && x > length && x != 2 * length)
                                possibleExits.Add(thisPoint);
                        }

                        else
                            tilePoints.Add(thisPoint);
                    }
                }

                break;

            case Room.Shape.Diamond:
                orientation = Random.Range(0, 3);
                string[] debugStrings = new string[] { "upDown, ", "downUp, ", "horizontal, " };
                Debug.Log("Diamond type " + debugStrings[orientation] + length + " by " + height);

                //slanted walls all around
                if (orientation == 2)
                {
                    for (int x = 0; x <= length + height + 2; x++)
                    {
                        int lineHeight;

                        if (x < length + 2 && x < height + 2)
                            lineHeight = x + 1;
                        else if (x >= length + 2 && x >= height + 2)
                            lineHeight = length + height + 3 - x;
                        else
                            lineHeight = Mathf.Min(length, height) + 2;


                        //Debug.Log($"{x}, {length}, {height}, {lineHeight}");

                        for (int y = 0; y < lineHeight; y++)
                        {
                            float pointX = genPoint.x + xOffset * x;
                            float pointY;
                            if (x <= length)
                                pointY = genPoint.y + (yOffset * (y - .5f * x));
                            else
                                pointY = genPoint.y + (yOffset * (y - .5f * (2 * length + 2 - x)));

                            Vector2 thisPoint = new Vector2(pointX, pointY);

                            if (x == 0 || x == length + height + 2 || y == 0 || y == lineHeight - 1)
                                wallPoints.Add(thisPoint);

                            else
                                tilePoints.Add(thisPoint);
                        }
                    }
                }

                //vertical walls on sides
                else
                {
                    for (int x = 0; x <= length + 1; x++)
                    {
                        for (int y = 0; y <= height + 1; y++)
                        {
                            float pointX = genPoint.x + xOffset * x;
                            float pointY = genPoint.y + (yOffset * (y + (orientation == 0 ? -.5f * x : .5f * x)));
                            Vector2 thisPoint = new Vector2(pointX, pointY);

                            if (x == 0 || x == length + 1 || y == 0 || y == height + 1)
                                wallPoints.Add(thisPoint);

                            else
                                tilePoints.Add(thisPoint);
                        }
                    }
                }
                break;

            case Room.Shape.Triangle:
                orientation = Random.Range(0, 2);
                Debug.Log("Triangle type " + (orientation == 0 ? "upDown, " : "downUp, ") + "side = " + length);

                for (int x = 0; x <= length + 2; x++)
                {
                    int lineHeight = orientation == 0 ? x + 1 : length + 3 - x;

                    for (int y = 0; y < lineHeight; y++)
                    {
                        float pointX = genPoint.x + xOffset * x;
                        float pointY = genPoint.y + (yOffset * (y + (orientation == 0 ? -.5f * x : .5f * x)));
                        Vector2 thisPoint = new Vector2(pointX, pointY);

                        if (x == 0 || x == length + 2 || y == 0 || y == lineHeight - 1)
                            wallPoints.Add(thisPoint);

                        else
                            tilePoints.Add(thisPoint);
                    }
                }
                break;

            case Room.Shape.Rect:
                orientation = Random.Range(0, 2);
                Debug.Log("Rectangle type " + (orientation == 0 ? "upDown, " : "downUp, ") + length + "by " + height);

                for (int x = 0; x <= length + 1; x++)
                {
                    for (int y = 0; y <= height + 1; y++)
                    {
                        float pointX = genPoint.x + xOffset * x;
                        float pointY;
                        if (x % 2 == 1)
                            pointY = genPoint.y + (yOffset * (y + (orientation == 0 ? -.5f : .5f)));
                        else
                            pointY = genPoint.y + yOffset * y;

                        Vector2 thisPoint = new Vector2(pointX, pointY);

                        if (x == 0 || x == length + 1 || y == 0 || y == height + 1)
                            wallPoints.Add(thisPoint);

                        else
                            tilePoints.Add(thisPoint);
                    }
                }
                break;

            default:

                break;
        }
    }

    private void SetDifficulty()
    {
        if (currentIndex % 2 == 0)
            difficulty = -1;
        else
        {
            for(int i=0; i<Room.maxDifficulty; i++)
            {
                if (currentIndex >= difficultyStarts[i])
                    difficulty = i;
            }
        }

        if(difficulty == -1)
        {
            webRatio = 0;
            minEnemies = 0;
            maxEnemies = 0;
        }

        else
        {
            webRatio = Room.webRatios[difficulty];
            minEnemies = Room.minEnemies[difficulty];
            maxEnemies = Room.maxEnemies[difficulty];
            enemyRates = new float[7];
            for (int i = 0; i < 7; i++)
            {
                enemyRates[i] = Room.enemyRarities[difficulty, i];
            }
        }      
    }

    private void PlaceTiles()
    {
        //room object
        roomObject = new GameObject();
        roomObjects.Add(roomObject);
        roomObject.name = $"Room {currentIndex}";
        roomObject.transform.parent = transform;
        List<GameObject> hexObjects = new List<GameObject>();

        //Debug.Log($"{tilePoints.Count} tiles, {wallPoints.Count} walls, {possibleExits.Count} possible exits");

        //establish entrance, set new exit
        Vector2 entrancePoint = prevRoom.exitPoint;
        int exitIndex = Random.Range(0, possibleExits.Count);
        thisRoom.exitPoint = possibleExits[exitIndex];

        int randIndex;

        //extra walls
        if (currentIndex % 2 == 0 && currentIndex != 0)
        {
            int wallCount = Random.Range(0, maxWalls + 1);
            for (int i = 0; i < wallCount; i++)
            {
                if (tilePoints.Count == 0)
                    break;
                randIndex = Random.Range(0, tilePoints.Count);
                inWallPoints.Add(tilePoints[randIndex]);
                tilePoints.RemoveAt(randIndex);
            }
        }

        //platofrm on the exit, nothing on the entrance(which was the previous room's exit)
        foreach (Vector2 point in wallPoints)
        {
            if (point == thisRoom.exitPoint)
            {
                hexObjects.Add(Instantiate(tiles[1], point, Quaternion.identity, roomObject.transform));
            }

            else if (point != entrancePoint)
                hexObjects.Add(Instantiate(outerWall, point, Quaternion.identity, roomObject.transform));
        }

        //maybe no inwalls
        /*foreach (Vector2 point in inWallPoints)
            Instantiate(innerWall, point, Quaternion.identity, roomObject.transform);*/

        //tiles[0] empty, tiles[1] platform, factor in web Ratio

        foreach (Vector2 point in tilePoints)
        {
            float tileChooser = Random.Range(0f, 1f);
            if (tileChooser <= webRatio)
                Instantiate(tiles[0], point, Quaternion.identity, roomObject.transform);
            else
                hexObjects.Add(Instantiate(tiles[1], point, Quaternion.identity, roomObject.transform));
        }

        //create and rotate entrance Mark
        GameObject ent = Instantiate(entranceMark, entrancePoint, Quaternion.identity, roomObject.transform);
        for(int i=0; i<3; i++)
            ent.transform.GetChild(0).GetChild(i).GetComponent<Entry>().roomNumber = currentIndex;
        for (int i = 3; i < 6; i++)
            ent.transform.GetChild(0).GetChild(i).GetComponent<Entry>().roomNumber = currentIndex-1;

        switch(prevRoom.exitDir)
        {
            case Room.TravelDirection.U:
                ent.transform.GetChild(0).Rotate(Vector3.back, -60);
                break;

            case Room.TravelDirection.UR:
                break;

            case Room.TravelDirection.DR:
                ent.transform.GetChild(0).Rotate(Vector3.back, 60);
                break;

            case Room.TravelDirection.D:
                ent.transform.GetChild(0).Rotate(Vector3.back, 120);
                break;

            case Room.TravelDirection.DL:
                ent.transform.GetChild(0).Rotate(Vector3.back, 180);
                break;

            case Room.TravelDirection.UL:
                ent.transform.GetChild(0).Rotate(Vector3.back, -60);
                break;
        }


        //we need to redo the ChooseSprite method because the objects spawned at different times
        foreach (GameObject go in hexObjects)
        {
            HexTile hexScript = go.transform.GetComponent<HexTile>();
            if(hexScript != null)
                hexScript.ChooseSprite(hexScript.type);
        }
    }

    private void AddEnemies()
    {       

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

        //where to put enemies
        for (int i = 0; i < enemyCount; i++)
        {
            if (tilePoints.Count == 0)
                break;
            int randIndex = Random.Range(0, tilePoints.Count);
            enemyPoints.Add(tilePoints[randIndex]);
            tilePoints.RemoveAt(randIndex);
        }

        //which enemy to place
        foreach (Vector2 point in enemyPoints)
        {
            float enemyRoll = Random.Range(0f, 1f);
            int enemyIndex = 0;
            for(int i=0; i<enemyRates.Length; i++)
            {
                enemyRoll -= enemyRates[i];
                if (enemyRoll <= 0)
                {
                    enemyIndex = i;
                    break;
                }                  
            }
            Instantiate(enemies[enemyIndex], point, Quaternion.identity, roomObject.transform);
        }
    }
}
