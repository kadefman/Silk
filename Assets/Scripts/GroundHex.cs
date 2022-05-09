using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHex : MonoBehaviour
{

    public enum Type { Wood, Web}
    public Type type;
    public Sprite[] webSprites;
    public Sprite uglySprite;
    public bool useArt;
    //public Color[] colors;

    void Awake()
    {
        ChooseSprite(type);
        //ChangeType(type);        
    }

    void Update()
    {
        
    }

    private void ChooseSprite(Type t)
    {
        int spriteIndex;
        switch(t)
        {
            case Type.Web:
                if (useArt)
                {
                    spriteIndex = Random.Range(0, webSprites.Length);
                    transform.GetComponent<SpriteRenderer>().sprite = webSprites[spriteIndex];
                }
                else
                    transform.GetComponent<SpriteRenderer>().sprite = uglySprite;
                break;

            default:
                break;
        }
    }

    private void ChangeType(Type t)
    {
        //transform.GetComponent<SpriteRenderer>().color = colors[(int)t];
    }
}
