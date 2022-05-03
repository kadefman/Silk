using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHex : MonoBehaviour
{

    public enum Type { Wood, Web}
    public Type type;
    public Color[] colors;

    void Awake()
    {
        //ChangeType(type);        
    }

    void Update()
    {
        
    }

    private void ChangeType(Type t)
    {
        //transform.GetComponent<SpriteRenderer>().color = colors[(int)t];
    }
}
