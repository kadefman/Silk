using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHex : MonoBehaviour
{

    public enum Type { Wood, Web}
    public Type type;
    public Color[] colors;

    // Start is called before the first frame update
    void Awake()
    {
        ChangeType(type);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeType(Type t)
    {
        transform.GetComponent<SpriteRenderer>().color = colors[(int)t];
    }
}
