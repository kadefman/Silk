using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<Room> rooms;
    public GameObject[] tiles;
    public GameObject[] enemies;
    public GameObject[] items;
    public GameObject wall;
    public GameObject exitMark;
    public GameObject entranceMark;
    public GameObject genMark;
    public GameObject player;
    public GameObject cam;
    //public GameObject startRoom;

    public int roomCount;
    public int minHallSize;
    public int maxHallSize;
    public int minRoomSize;
    public int maxRoomSize;  
    public int minEnemies;
    public int maxEnemies;
    public int minWalls;
    public int maxWalls;
    public int minItems;
    public int maxItems;
    public float webRatio;

    
    private Room thisRoom;
    private Room prevRoom;
    private GameObject roomObject;
    private List<GameObject> roomObjects;

    private List<Vector2> possibleExits;
    private List<Room.TravelDirection> possibleExitDirections;
    
    private List<Vector2> tilePoints;
    private List<Vector2> wallPoints;
    private List<Vector2> enemyPoints;
    private List<Vector2> itemPoints;
    
    private Vector2 playerPos = Vector2.zero;
    private Vector2 rayOffset = new Vector2(0f, 0.1f);

    private int currentIndex = 0;

    private float xOffset = .96f;
    private float yOffset = 1.12f;

    

    void Start()
    {
        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        GenerateLevel();
        cam.transform.parent = player.transform;
        player.transform.position = playerPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ClearLevel();
            GenerateLevel();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {           
            GenerateLevel(true);            
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

    void GenerateLevel(bool step = false)
    {
        if (!step)
        {
            Debug.Log("Generate All");
            currentIndex = 0;
            while(currentIndex < roomCount)
            {
                Debug.Log("Generating room " + currentIndex + "----------");
                CreateRoom();
                currentIndex++;
            }          
        }

        else
        {
            Debug.Log("Generating room " + currentIndex + "----------");
            CreateRoom();
            currentIndex++;
        }
                    
    }

    //where do I do the startRoom thing? Address this in Room script too
    void CreateRoom(bool start = false)
    {
        PrepareRoom();
        ChooseExits(currentIndex);

        int height = Random.Range(minRoomSize, maxRoomSize);
        int length = Random.Range(minRoomSize, maxRoomSize);
        Vector2 genPoint = ChooseGenOffset(height, length);

        BuildShape(height, length, genPoint);

        //index was brought back 2, try again
        if (CollisionTest())
            return;

        PlaceTiles();
        FillRoom();
    }

    private void PrepareRoom()
    {
         //take some stuff from chooseExits?
        tilePoints = new List<Vector2>();
        wallPoints = new List<Vector2>();
        enemyPoints = new List<Vector2>();
        itemPoints = new List<Vector2>();
        possibleExits = new List<Vector2>();
        possibleExitDirections = new List<Room.TravelDirection>();

    }

    private void ChooseExits(int roomIndex)
    {
        //start room, will use prefab
        if (roomIndex == 0)
        {
            thisRoom = new Room(Room.Shape.Hexagon);
            //not sure how much info I'll need here going forward
            thisRoom.exitDir = Room.TravelDirection.D;
            thisRoom.exitWall = Room.WallDirection.DR;           
        }

        else
        {
            //set up entrances
            prevRoom = rooms[currentIndex - 1];                      
            int newEntranceNum = ((int)prevRoom.exitDir + 3) % 6;
            Room.TravelDirection newEntrance = (Room.TravelDirection)newEntranceNum;
            int newWallNum = ((int)prevRoom.exitWall + 3) % 6;
            Room.WallDirection enterWall = (Room.WallDirection)newWallNum;
            Debug.Log("coming from wallDir" + (int)enterWall);

            //pick exits:
            //hall, auto exit direction
            if (roomIndex % 2 == 1)
            {
                thisRoom = new Room(Room.Shape.Hall, newEntrance, enterWall);
                /*int exitInt = (Random.Range(5, 7) + (int)prevRoom.exitDir)%6;
                thisRoom.exitDir = (Room.TravelDirection)exitInt;*/
                thisRoom.exitDir = prevRoom.exitDir;
                thisRoom.exitWall = prevRoom.exitWall;                
            }

            //hex, 1 of 5 possible exit directions, 1 of 5 exit walls
            else
            {
                thisRoom = new Room(Room.Shape.Hexagon, newEntrance, enterWall);
                int rand = Random.Range(0, 6);
                int exitInt = rand;
                if (exitInt == newEntranceNum)
                    exitInt = newEntranceNum == 5 ? 0 : newEntranceNum + 1;
                thisRoom.exitDir = (Room.TravelDirection)exitInt;

                //choose wall
                rand = Random.Range(0, 2);
                int exitWallInt = (exitInt + rand)%6;
                if (exitWallInt == newWallNum)
                {
                    rand = (rand + 1) % 2;
                    exitWallInt = (exitInt + rand) % 6;
                }
                    
                thisRoom.exitWall = (Room.WallDirection)exitWallInt;
            }
            thisRoom.entrancePoint = prevRoom.exitPoint;          
        }
        rooms.Add(thisRoom);
    }

    private Vector2 ChooseGenOffset(int height, int length)
    {
        if (currentIndex == 0)
            return Vector2.zero;

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
                Debug.Log("Hex randOffset " + randOffset);
                Debug.Log("coming from wallDir" + (int)thisRoom.entranceWall);

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

    private bool CollisionTest()
    {
        List<Vector2> spotsToCheck = new List<Vector2>(tilePoints);
        spotsToCheck.AddRange(wallPoints);

        foreach(Vector2 point in spotsToCheck)
        {
            RaycastHit2D hit = Physics2D.Linecast(point, point + rayOffset, LayerMask.GetMask("Tall"));

            //go back 2 rooms, try again
            if (hit.transform != null)
            {
                Debug.Log("COLLISION! Can't make room " + currentIndex);
                Debug.Log(rooms.Count + " " + roomObjects.Count);

                Destroy(roomObjects[roomObjects.Count - 1]);
                Destroy(roomObjects[roomObjects.Count - 2]);
                roomObjects.RemoveRange(roomObjects.Count - 2,2);
                rooms.RemoveRange(rooms.Count - 3, 3);
                currentIndex -= 3;

                Debug.Log(rooms.Count + " " + roomObjects.Count);
                return true;
            }
        }
        return false;
    }

    private void PlaceTiles()
    {
        //room object
        roomObject = new GameObject();
        roomObjects.Add(roomObject);
        roomObject.name = $"Room {currentIndex}";
        roomObject.transform.parent = transform;

        //Debug.Log($"{tilePoints.Count} tiles, {wallPoints.Count} walls, {possibleExits.Count} possible exits");

        //establish entrance, set new exit
        Vector2 entrancePoint = currentIndex == 0 ? Vector2.zero : prevRoom.exitPoint;
        int exitIndex = Random.Range(0, possibleExits.Count);
        thisRoom.exitPoint = possibleExits[exitIndex];

        int randIndex;

        //extra walls
        if (currentIndex % 2 == 0 && currentIndex != 0)
        {
            int wallCount = Random.Range(minWalls, maxWalls + 1);
            for (int i = 0; i < wallCount; i++)
            {
                if (tilePoints.Count == 0)
                    break;
                randIndex = Random.Range(0, tilePoints.Count);
                wallPoints.Add(tilePoints[randIndex]);
                tilePoints.RemoveAt(randIndex);
            }
        }

        foreach (Vector2 point in wallPoints)
        {
            if (point == thisRoom.exitPoint)
            {
                Instantiate(exitMark, point, Quaternion.identity, roomObject.transform);
                Instantiate(tiles[1], point, Quaternion.identity, roomObject.transform);
            }

            else if (point != entrancePoint)
                Instantiate(wall, point, Quaternion.identity, roomObject.transform);
        }

        //tiles[0] empty, tiles[1] platform, will need to rework with more plat assets
        foreach (Vector2 point in tilePoints)
        {
            float tileChooser = Random.Range(0f, 1f);
            if (currentIndex == 0)
                tileChooser = 1f;
            if (tileChooser <= webRatio)
                Instantiate(tiles[0], point, Quaternion.identity, roomObject.transform);
            else
                Instantiate(tiles[1], point, Quaternion.identity, roomObject.transform);
        }

        if (currentIndex == 0)
        {
            Instantiate(wall, entrancePoint, Quaternion.identity, roomObject.transform);
            randIndex = Random.Range(0, tilePoints.Count);
            playerPos = tilePoints[randIndex];
            Instantiate(tiles[1], tilePoints[randIndex], Quaternion.identity, roomObject.transform);
            tilePoints.RemoveAt(randIndex);
        }

        else
            Instantiate(entranceMark, entrancePoint, Quaternion.identity, roomObject.transform);
    }

    private void FillRoom()
    {       
        //Debug.Log(tilePoints.Count + " tiles remaining");

        //Fill room with enemies and items

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        int itemCount = Random.Range(minItems, maxItems + 1);
        int randIndex;

        for (int i = 0; i < enemyCount; i++)
        {
            if (tilePoints.Count == 0)
                break;
            randIndex = Random.Range(0, tilePoints.Count);
            /*Debug.Log(tilePoints.Count + " " + randIndex);
            Debug.Log(enemyPoints.Count);*/
            enemyPoints.Add(tilePoints[randIndex]);
            tilePoints.RemoveAt(randIndex);
        }

        for (int i = 0; i < itemCount; i++)
        {
            if (tilePoints.Count == 0)
                break;
            randIndex = Random.Range(0, tilePoints.Count);
            //Debug.Log(tilePoints.Count + " " + randIndex);
            itemPoints.Add(tilePoints[randIndex]);
            tilePoints.RemoveAt(randIndex);
        }

        foreach (Vector2 point in itemPoints)
        {
            randIndex = Random.Range(0, items.Length);
            Instantiate(items[randIndex], point, Quaternion.identity, roomObject.transform);
        }

        foreach (Vector2 point in enemyPoints)
        {
            randIndex = Random.Range(0, enemies.Length);
            Instantiate(enemies[randIndex], point, Quaternion.identity, roomObject.transform);
        }
    }
}
