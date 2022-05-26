using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{

    public enum Type { Ground, Web, Wall}
    public Type type;
    public Room.TravelDirection neighborDir;
    public Sprite[] webSprites;
    public Sprite[] branchSprites;
    public Sprite[] extendSprites;
    public Sprite[] treeTopSprites;


    [HideInInspector] public bool isBranch;
    
    private Vector2 rayOffset = new Vector2(0f, 0.1f);
    private SpriteRenderer frontSprite;
    private SpriteRenderer backSprite;

    void Awake()
    {
        if (type == Type.Ground)
        {
            frontSprite = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            backSprite = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        }

        //the generator will call this script on all relevant objects when it's done generating
        if(GameManager.instance == null || !GameManager.instance.generating)
            ChooseSprite(type);              
    }
    private void OnEnable()
    {
        ChooseSprite(type);
    }

    public void ChooseSprite(Type t)
    {
        int spriteIndex;
        int randRotation;
        switch(t)
        {
            case Type.Web:
                spriteIndex = Random.Range(0, webSprites.Length);
                transform.GetComponent<SpriteRenderer>().sprite = webSprites[spriteIndex];
                break;

            case Type.Ground:             
                List<Vector2> aroundTiles = GetSurroundings(transform.position);

                //attach to walls
                int wallAttachDirection = -1;
                randRotation = Random.Range(0, 6);
                int j = randRotation;

                for (int i=0; i<6; i++)
                {                   
                    Vector2 point = aroundTiles[j];
                    RaycastHit2D hit = Physics2D.Linecast(point, point + rayOffset, LayerMask.GetMask("Tall"));

                    if (hit.transform != null)
                        wallAttachDirection = j;

                    j++;
                    if (j == 6)
                        j = 0;
                }

                if (wallAttachDirection != -1)
                    OrientTile(branchSprites, (Room.TravelDirection)wallAttachDirection);

                //attach to ground
                else
                {
                    float randTreeTop = Random.Range(0f , 1f);
                    int groundAttachDirection = -1;
                    randRotation = Random.Range(0, 6);
                    j = randRotation;

                    for (int i = 0; i < 6; i++)
                    {                    
                        Vector2 point = aroundTiles[j];
                        RaycastHit2D hit = Physics2D.Linecast(point, point + rayOffset, LayerMask.GetMask("Ground"));

                        if (hit.transform != null)
                        {
                            groundAttachDirection = j;
                        }

                        j++;
                        if (j == 6)
                            j = 0;
                    }

                    if (groundAttachDirection == -1 || randTreeTop <= .3f)
                    {
                        randRotation = Random.Range(0, 6); 
                        OrientTile(treeTopSprites, (Room.TravelDirection)randRotation);
                    }
                        
                    else
                        OrientTile(extendSprites, (Room.TravelDirection)groundAttachDirection);
                }               
                break;

            case Type.Wall:
                aroundTiles = GetSurroundings(transform.position);
                int attachDir = 0;
                int openEdges = 0;
                for(int i=0; i<6; i++)
                {
                    Vector2 point = aroundTiles[i];
                    RaycastHit2D hit = Physics2D.Linecast(point, point + rayOffset, LayerMask.GetMask("Ground"));

                    if(hit.transform != null)
                    {
                        openEdges++;
                        if(openEdges<3)
                            attachDir = i;                       
                    }

                    else
                    {
                        hit = Physics2D.Linecast(point, point + rayOffset, LayerMask.GetMask("EmptyTile"));

                        if (hit.transform != null)
                        {
                            openEdges++;
                            if (openEdges < 3)
                                attachDir = i;
                        }
                    }
                }

                OrientTree(openEdges, (Room.TravelDirection)attachDir);

                break;
        }
    }

    private List<Vector2> GetSurroundings(Vector2 middle)
    {
        List<Vector2> points = new List<Vector2>();
        float x = Generator.xOffset;
        float y = Generator.yOffset;
        //travelD order: U clockwise

        points.Add(y * Vector2.up);
        points.Add(new Vector2(x, .5f * y));
        points.Add(new Vector2(x, -.5f * y));
        points.Add(y * Vector2.down);
        points.Add(new Vector2(-x, -.5f * y));
        points.Add(new Vector2(-x, .5f * y));

        for(int i=0; i<6; i++)
        {
            points[i] += middle;
        }

        return points;
    }

    private void OrientTile(Sprite[] sprites, Room.TravelDirection dir)
    {
        neighborDir = dir;
        frontSprite.sprite = sprites[0];
        backSprite.sprite = sprites[1];
        //Debug.Log(transform.position + " is facing dir " + (int)dir);
        switch(dir)
        {
            case Room.TravelDirection.U:
                //rotate ccw
                frontSprite.transform.Rotate(Vector3.back * -60f);
                backSprite.transform.Rotate(Vector3.back * -60f);
                break;

            case Room.TravelDirection.UR:
                //do nothing
                break;

            case Room.TravelDirection.DR:
                //rotate cw
                frontSprite.transform.Rotate(Vector3.back * 60f);
                backSprite.transform.Rotate(Vector3.back * 60f);
                break;

            case Room.TravelDirection.D:
                //rotate 2cw
                frontSprite.transform.Rotate(Vector3.back * 120f);
                backSprite.transform.Rotate(Vector3.back * 120f);
                break;

            case Room.TravelDirection.DL:
                //rotate cw, flipx
                frontSprite.transform.Rotate(Vector3.forward * 60f);
                backSprite.transform.Rotate(Vector3.forward * 60f);
                frontSprite.flipX = true;
                backSprite.flipX = true;

                break;

            case Room.TravelDirection.UL:
                //flipx
                frontSprite.flipX = true;
                backSprite.flipX = true;
                break;
        }
    }

    private void OrientTree(int edges, Room.TravelDirection dir)
    {
        WallTileArt treeScript = transform.GetComponent<WallTileArt>();
        treeScript.nbOfFreeCorners = edges;
        treeScript.seed = Random.Range(0, 100);
        List<Transform> artSprites = new List<Transform>();
        for(int i=1; i<5; i++)
            artSprites.Add(transform.GetChild(i));

        foreach(Transform t in artSprites)
        {
            //default is down
            switch (dir)
            {
                case Room.TravelDirection.D:
                    //do nothing
                    break;

                case Room.TravelDirection.DL:
                    //clockwise
                    t.Rotate(Vector3.back * 60f);
                    break;

                case Room.TravelDirection.UL:
                    //2 clockwise, so on
                    t.Rotate(Vector3.back * 120f);
                    break;

                case Room.TravelDirection.U:
                    t.Rotate(Vector3.back * 180f);
                    break;

                case Room.TravelDirection.UR:
                    t.Rotate(Vector3.forward * 120f);
                    break;

                case Room.TravelDirection.DR:
                    t.Rotate(Vector3.forward * 60f);
                    break;
            }
        }       
    }
}
