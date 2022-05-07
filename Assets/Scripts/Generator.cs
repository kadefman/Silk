using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject[] enemies;
    public GameObject[] items;
    public GameObject wall;
    public GameObject exitMark;
    public GameObject entranceMark;
    public GameObject player;
    public GameObject cam;

    //the following public variables are only for testing single rooms
    //public Room.Shape shape;
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

    private Room.TravelDirection recentExit;
    private Room.WallDirection recentExitWall;
    private List<Room.TravelDirection> possibleExitDirections;
    private List<Vector2> possibleExits;
    private List<Vector2> tilePoints;
    private List<Vector2> wallPoints;
    private List<Vector2> enemyPoints;
    private List<Vector2> itemPoints;
    private Vector2 recentExitPoint;
    private Vector2 playerPos = Vector2.zero;

    private int currentIndex = 0;

    private float xOffset = .96f;
    private float yOffset = 1.12f;

    

    void Start()
    {
        GenerateLevel();
        cam.transform.parent = player.transform;
        player.transform.position = playerPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            currentIndex = 0;
            foreach(Transform child in transform)
                Destroy(child.gameObject);

            GenerateLevel();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {           
            GenerateLevel(true);            
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            currentIndex = 0;
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }
    }

    void GenerateLevel(bool step = false)
    {
        if (!step)
        {
            Debug.Log("Generate All");
            for (int i = 0; i < roomCount; i++)
            {
                Debug.Log("Generating room " + currentIndex + "----------");
                PrepareRoom(i);
                currentIndex++;
            }               
        }

        else
        {
            PrepareRoom(currentIndex);
            currentIndex++;
        }
                    
    }

    void PrepareRoom(int roomIndex)
    {
        /*Room.Shape thisShape;

            if (shape == Room.Shape.Random)
            {
                int rand = Random.Range(0, 4);
                thisShape = (Room.Shape)rand;
            }
            else
                thisShape = shape;*/

        //Room thisRoom = new Room(thisShape);


        Room thisRoom;
        Room.TravelDirection exit;
        Room.WallDirection exitWall;
        tilePoints = new List<Vector2>();
        wallPoints = new List<Vector2>();
        enemyPoints = new List<Vector2>();
        itemPoints = new List<Vector2>();
        possibleExits = new List<Vector2>();
        possibleExitDirections = new List<Room.TravelDirection>();

        //start room, will use prefab
        if (roomIndex == 0)
        {
            thisRoom = new Room(Room.Shape.Hexagon);
            //not sure how much info I'll need here going forward
            exit = Room.TravelDirection.D;
            exitWall = Room.WallDirection.DR;           
        }

        else
        {
            int newEntranceNum = ((int)recentExit + 3) % 6;
            Room.TravelDirection newEntrance = (Room.TravelDirection)newEntranceNum;
            int newWallNum = ((int)recentExitWall + 3) % 6;
            Room.WallDirection enterWall = (Room.WallDirection)newWallNum;

            //hall, auto exit direction
            if (roomIndex % 2 == 1)
            {
                thisRoom = new Room(Room.Shape.Hall, newEntrance, enterWall);
                exit = recentExit;
                exitWall = recentExitWall;
            }

            //hex, 1 of 5 possible exit directions, 1 of 5 exit walls
            else
            {
                int rand;
                //choose direction
                thisRoom = new Room(Room.Shape.Hexagon, newEntrance, enterWall);
                rand = Random.Range(0, 6);
                int exitInt = rand;
                if (exitInt == newEntranceNum)
                    exitInt = newEntranceNum == 5 ? 0 : newEntranceNum + 1;
                exit = (Room.TravelDirection)exitInt;

                //choose wall
                rand = Random.Range(0, 2);
                int exitWallInt = (exitInt + rand)%6;
                if (exitWallInt == newWallNum)
                {
                    rand = (rand + 1) % 2;
                    exitWallInt = (exitInt + rand) % 6;
                }
                    
                exitWall = (Room.WallDirection)exitWallInt;

                /*Debug.Log("entrance dir " + newEntranceNum);
                Debug.Log("entrance Wall dir " + newWallNum);*/
            }
        }
          
        GenerateRoom(thisRoom, recentExitPoint, exit, exitWall);
        
    }

    void GenerateRoom(Room room, Vector2 entrancePoint, Room.TravelDirection exitDir, Room.WallDirection exitWallDir)
    {
        //Debug.Log("Room entrance at " + entrancePoint);
        GameObject roomObject = new GameObject();
        roomObject.name = $"Room {currentIndex}";
        roomObject.transform.parent = transform;
        int length = Random.Range(minRoomSize, maxRoomSize + 1);
        int height = Random.Range(minRoomSize, maxRoomSize+1);
        int orientation;

        //does this code belong all the way up here?
        Vector2 genOffset;        
        Vector2 genPoint = Vector2.zero;

        switch (room.shape)
        {
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

            case Room.Shape.Diamond:

                orientation = Random.Range(0, 3);
                string debugString;
                if (orientation == 0)
                    debugString = "upDown, ";
                else if (orientation == 1)
                    debugString = "downUp, ";
                else
                    debugString = "horizontal, ";

                Debug.Log("Diamond type " + debugString + length + " by " + height);

                //slanted walls all around
                if(orientation == 2)
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

            case Room.Shape.Hexagon:
                Debug.Log("Hexagon, side = " + length);
                if (length < 2)
                    length = 2;

                int randOffset = Random.Range(1, length);

                //choose genPoint from which the room is built
                switch (room.entranceWall)
                {
                    case Room.WallDirection.L:
                        Debug.Log("coming from L");
                        genOffset = new Vector2(0, -randOffset * yOffset);
                        room.entranceWall = Room.WallDirection.L;
                        break;

                    case Room.WallDirection.UL:
                        Debug.Log("coming from UL");
                        genOffset = new Vector2(-randOffset * xOffset, -yOffset*(length + .5f*randOffset));
                        room.entranceWall = Room.WallDirection.UL;
                        break;
                 
                    case Room.WallDirection.DL:
                        Debug.Log("coming from DL");
                        genOffset = new Vector2(-randOffset * xOffset, randOffset * .5f * yOffset);
                        room.entranceWall = Room.WallDirection.DL;
                        break;

                    case Room.WallDirection.R:
                        Debug.Log("coming from R");
                        genOffset = new Vector2(-2*length*xOffset, -randOffset * yOffset);
                        room.entranceWall = Room.WallDirection.R;
                        break;

                    case Room.WallDirection.UR:
                        Debug.Log("coming from UR");
                        genOffset = new Vector2(-xOffset * (randOffset + length), -yOffset * .5f * (randOffset + length));
                        room.entranceWall = Room.WallDirection.UL;
                        break;

                        
                    case Room.WallDirection.DR:
                        Debug.Log("coming from DR");
                        genOffset = new Vector2(-xOffset * (randOffset + length), yOffset *.5f * (randOffset - length));
                        room.entranceWall = Room.WallDirection.UL;
                        break;

                    default:
                        genOffset = Vector2.zero;
                        Debug.Log("You didn't pick a valid entrance direction");
                        break;

                }
                //Debug.Log("genOffset " + genOffset + ", randOffset " + randOffset);

                genPoint = entrancePoint + genOffset;

                //build room, choose potential exit points
                for (int x = 0; x <= 2 * length; x++)
                {
                    int lineHeight;
                    if (x <= length)
                        lineHeight = x + length + 1;
                    else
                        lineHeight = 3* length + 1 - x;

                    for(int y=0; y< lineHeight; y++)
                    {
                        float pointX = genPoint.x + xOffset * x;
                        float pointY;
                        if(x < length)
                            pointY = genPoint.y + (yOffset * (y - .5f * x));
                        else
                            pointY = genPoint.y + (yOffset * (1 + y - .5f * (2* length + 2 - x)));

                        Vector2 thisPoint = new Vector2(pointX, pointY);

                        //We'll handle corners another time
                        if(x==0)
                        {
                            wallPoints.Add(thisPoint);
                            if (exitWallDir == Room.WallDirection.L && y != 0 && y != lineHeight-1)
                                possibleExits.Add(thisPoint);
                        }

                        else if(x==2*length)
                        {
                            wallPoints.Add(thisPoint);
                            if (exitWallDir ==  Room.WallDirection.R && y != 0 && y != lineHeight-1)
                                possibleExits.Add(thisPoint);
                        }

                        else if(y==0)
                        {
                            wallPoints.Add(thisPoint);
                            if (exitWallDir == Room.WallDirection.DL && x < length && x != 0)
                                possibleExits.Add(thisPoint);

                            else if (exitWallDir == Room.WallDirection.DR && x > length && x != 2*length)
                                possibleExits.Add(thisPoint);
                        }

                        else if (y == lineHeight-1)
                        {
                            wallPoints.Add(thisPoint);
                            if (exitWallDir == Room.WallDirection.UL && x < length && x != 0)
                                possibleExits.Add(thisPoint);

                            else if (exitWallDir == Room.WallDirection.UR && x > length && x != 2*length)
                                possibleExits.Add(thisPoint);
                        }

                        else
                            tilePoints.Add(thisPoint);
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

                        if (x == 0 || x == length + 2 || y == 0 || y == lineHeight-1)
                            wallPoints.Add(thisPoint);

                        else
                            tilePoints.Add(thisPoint);
                    }
                }
                break;

            case Room.Shape.Hall:
                //auto direction
                genPoint = entrancePoint;              
                Debug.Log("Hall, length " + length);
                //Debug.Log("genPoint: " + genPoint);

                length = Random.Range(minHallSize, maxHallSize + 1);
                if (length < 3)
                    length = 3;

                for (int i=0; i<length; i++)
                {
                    Vector2 thisPoint;
                    switch(exitDir)
                    {
                        case Room.TravelDirection.UL:
                            thisPoint = new Vector2(genPoint.x + -xOffset * i, genPoint.y + .5f * yOffset * i);
                            wallPoints.Add(thisPoint + new Vector2(0, yOffset));
                            wallPoints.Add(thisPoint + new Vector2(0, -yOffset));                            
                            break;

                        case Room.TravelDirection.DL:
                            thisPoint = new Vector2(genPoint.x + -xOffset * i, genPoint.y - .5f * yOffset * i);
                            wallPoints.Add(thisPoint + new Vector2(0, yOffset));
                            wallPoints.Add(thisPoint + new Vector2(0, -yOffset));
                            break;

                        //walls are lower than tiles
                        case Room.TravelDirection.U:
                            thisPoint = new Vector2(genPoint.x, genPoint.y + yOffset * i);
                            wallPoints.Add(thisPoint + new Vector2(-xOffset, -.5f * yOffset));
                            wallPoints.Add(thisPoint + new Vector2(xOffset, -.5f * yOffset));
                            break;

                        case Room.TravelDirection.UR:
                            thisPoint = new Vector2(genPoint.x + xOffset * i, genPoint.y + .5f * yOffset * i);
                            wallPoints.Add(thisPoint + new Vector2(0, yOffset));
                            wallPoints.Add(thisPoint + new Vector2(0, -yOffset));
                            break;

                        case Room.TravelDirection.DR:
                            thisPoint = new Vector2(genPoint.x + xOffset * i, genPoint.y - .5f * yOffset * i);
                            wallPoints.Add(thisPoint + new Vector2(0, yOffset));
                            wallPoints.Add(thisPoint + new Vector2(0, -yOffset));
                            break;

                        //walls are higher than tiles
                        case Room.TravelDirection.D:
                            thisPoint = new Vector2(genPoint.x, genPoint.y - yOffset * i);
                            wallPoints.Add(thisPoint + new Vector2(-xOffset, .5f * yOffset));
                            wallPoints.Add(thisPoint + new Vector2(xOffset, .5f * yOffset));
                            break;

                        default:
                            thisPoint = Vector2.zero;
                            Debug.Log("You're missing an exit direction");
                            break;
                    }

                    if (i == length - 1)
                    {
                        possibleExits.Add(thisPoint);
                        wallPoints.Add(thisPoint);
                    }
                        
                    else
                        tilePoints.Add(thisPoint);                  
                }
                break;
        }

        /*Debug.Log("Exit dir: " + (int)exitDir);
        Debug.Log("Exit wall dir " + (int)exitWallDir);*/
        Debug.Log($"{tilePoints.Count} tiles, {wallPoints.Count} walls, {possibleExits.Count} possible exits");
        
        int exitIndex = Random.Range(0, possibleExits.Count);      
        recentExitPoint = possibleExits[exitIndex]; 
        recentExit = exitDir;
        recentExitWall = exitWallDir;

        //Debug.Log("Exit located at " + recentExitPoint);

        int randIndex;

        //Fill room with platforms and walls
       
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
            if (point == recentExitPoint)
                Instantiate(exitMark, point, Quaternion.identity, roomObject.transform);
            else if(point != entrancePoint)
                Instantiate(wall, point, Quaternion.identity, roomObject.transform);
        }
       
        //tiles[0] empty, tiles[1] platform, will need to rework with more plat assets
        foreach(Vector2 point in tilePoints)
        {
            float tileChooser = Random.Range(0f, 1f);
            if(tileChooser <= webRatio)
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
        {
            Instantiate(entranceMark, entrancePoint, Quaternion.identity, roomObject.transform);
            Instantiate(tiles[1], entrancePoint, Quaternion.identity, roomObject.transform);
        }
            


        Debug.Log(tilePoints.Count + " tiles remaining");

        //Fill room with enemies and items

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        int itemCount = Random.Range(minItems, maxItems + 1);      

        for (int i = 0; i < enemyCount; i++)
        {
            if (tilePoints.Count == 0)
                break;
            randIndex = Random.Range(0, tilePoints.Count);
            Debug.Log(tilePoints.Count + " " + randIndex);
            Debug.Log(enemyPoints.Count);
            enemyPoints.Add(tilePoints[randIndex]);
            tilePoints.RemoveAt(randIndex);
        }

        for (int i = 0; i < itemCount; i++)
        {
            if (tilePoints.Count == 0)
                break;
            randIndex = Random.Range(0, tilePoints.Count);
            Debug.Log(tilePoints.Count + " " + randIndex);           
            itemPoints.Add(tilePoints[randIndex]);
            tilePoints.RemoveAt(randIndex);
        }

        foreach(Vector2 point in itemPoints)
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
