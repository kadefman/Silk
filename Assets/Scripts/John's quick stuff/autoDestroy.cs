using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoDestroy : MonoBehaviour
{
    public float duration;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;   
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > duration)
        {
            Destroy(gameObject);
        }
    }
}
