using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class randomLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SortingGroup>().sortingOrder = Random.Range(-100, 0);
    }

 
}
