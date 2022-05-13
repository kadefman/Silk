using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScne : MonoBehaviour
{
    public float timeBeforeChange;
    public int indexToGo;
    private float t;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > timeBeforeChange)
        {
            SceneManager.LoadScene(indexToGo);
        }
    }

}
