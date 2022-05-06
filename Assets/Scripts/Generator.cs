using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject[] enemies;
    public GameObject[] items;
    public GameObject wall;
    public GameObject player;
    public GameObject cam;

    //the following public variables are only for testing single rooms
    public Room.Shape shape;
    /*public int width;
    public int height;
    public int startX;
    public int startY;*/

    private float xOffset = .96f;
    private float yOffset = 1.12f;

    private Vector2 playerPos = Vector2.zero;

    void Start()
    {
        GenerateLevel();
        cam.transform.parent = player.transform;
        player.transform.position = playerPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            foreach(Transform child in transform)
                Destroy(child.gameObject);

            GenerateLevel();
        }           
    }

    void GenerateLevel()
    {
        Room.Shape singleShape;

        if (shape == Room.Shape.Other)
        {            
            int rand = Random.Range(0,3);
            singleShape = (Room.Shape)rand;
        }
        else
            singleShape = shape;

        Room singleRoom = new Room(singleShape);
        GenerateRoom(singleRoom, Vector2.zero);
    }

    void GenerateRoom(Room room, Vector2 genPoint)
    {
        List<Vector2> tilePoints = new List<Vector2>();
        List<Vector2> wallPoints = new List<Vector2>();

        switch(room.shape)
        {
            case Room.Shape.Rect:
                int length = Random.Range(2, 6);
                int height = Random.Range(2, 6);
                bool upDown = false;
                int rand = Random.Range(0, 2);
                upDown = rand == 0;

                Debug.Log("Rectangle type " + (upDown ? "upDown, " : "downUp, ") + length + "by " + height);

                for (int x = 0; x <= length+1; x++)
                {
                    for(int y=0; y <= height+1; y++)
                    {
                        float pointX = genPoint.x + xOffset * x;
                        float pointY;
                        if (x % 2 == 1)
                            pointY = genPoint.y + (yOffset * (y + (upDown ? -.5f : .5f)));
                        else
                            pointY = genPoint.y + yOffset * y;

                        Debug.Log($"{x}, {y}, {pointX}, {pointY}");
                        Vector2 thisPoint = new Vector2(pointX, pointY);

                        if (x == 0 || x == length+1 || y == 0 || y == height+1)
                            wallPoints.Add(thisPoint);

                        else
                            tilePoints.Add(thisPoint);
                    }
                }
                break;

            case Room.Shape.Circle:
                int rad = Random.Range(1, 6);
                
                break;

            case Room.Shape.Hall:
                break;

            default:
                Debug.Log("This shape has not been implemented");
                return;
        }

        foreach (Vector2 point in wallPoints)   
            Instantiate(wall, point, Quaternion.identity, transform);

        foreach(Vector2 point in tilePoints)
        {
            int rand = Random.Range(0, 2);
            Instantiate(tiles[rand], point, Quaternion.identity,transform);
        }
            
        //choose locations for player, enemies, pickups
    }
}
