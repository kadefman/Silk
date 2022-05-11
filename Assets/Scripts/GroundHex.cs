using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHex : MonoBehaviour
{

    public enum Type { Ground, Web}
    public Type type;
    public Sprite[] webSprites;
    public Sprite[] branchSprites;
    public Sprite[] extendSprites;
    public Sprite[] treeTopSprites;
    [HideInInspector] public bool isBranch;
    public Room.TravelDirection neighborDir;

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
        ChooseSprite(type);              
    }

    private void ChooseSprite(Type t)
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

                    if (groundAttachDirection == -1)
                    {
                        randRotation = Random.Range(0, 6); 
                        OrientTile(treeTopSprites, (Room.TravelDirection)randRotation);
                    }
                        
                    else
                        OrientTile(extendSprites, (Room.TravelDirection)groundAttachDirection);
                }               
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
        Debug.Log(sprites.Length + "sprites in the array");
        frontSprite.sprite = sprites[0];
        backSprite.sprite = sprites[1];
        //Debug.Log(transform.position + " is facing dir " + (int)dir);
        switch(dir)
        {
            case Room.TravelDirection.U:
                //rotate ccw
                transform.Rotate(Vector3.back * -60f);
                break;

            case Room.TravelDirection.UR:
                //do nothing
                break;

            case Room.TravelDirection.DR:
                //rotate cw
                transform.Rotate(Vector3.back * 60f);
                break;

            case Room.TravelDirection.D:
                //rotate 2cw
                transform.Rotate(Vector3.back * 120f);
                break;

            case Room.TravelDirection.DL:
                //rotate cw, flipx
                transform.Rotate(Vector3.forward * 60f);
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
}
