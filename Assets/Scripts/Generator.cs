using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject tile;
    public GameObject platform;
    public GameObject wall;
    public GameObject player;
    public GameObject cam;
    public bool followSpider;

    public int width;
    public int height;
    public int startX;
    public int startY;

    public float xOffset = .96f;
    public float yOffset = .56f;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        Vector2 playerPos = Vector2.zero;

        for (int x = 0; x <= width + 1; x++)
        {
            for (int y = 0; y <= height + 1; y++)
            {
                GameObject hex;

                if (x == 0 || y == 0 || x == width + 1 || y == height + 1)
                    hex = wall;

                else
                {
                    if (x == startX & y == startY)
                        hex = platform;
                    else
                        hex = tile;
                }

                GameObject go;

                if (x % 2 == 0)
                    go = Instantiate(hex, new Vector2(x * xOffset, 2 * y * yOffset), Quaternion.identity, transform);
                else
                    go = Instantiate(hex, new Vector2(x * xOffset, 2 * y * yOffset - yOffset), Quaternion.identity, transform);
               
                if (hex == platform)
                    playerPos = new Vector2(go.transform.position.x, go.transform.position.y);
            }
        }

        if (followSpider)
            cam.transform.parent = player.transform;
        else
            cam.transform.Translate(new Vector2(xOffset * width / 2, yOffset * height));

        player.transform.position = playerPos;
    }
}
