using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasDo : MonoBehaviour
{
    public static canvasDo instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }

        GameManager.instance.canvas = gameObject;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
