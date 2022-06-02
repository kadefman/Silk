using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTileArt : MonoBehaviour
{
    public Transform Walls;
    public Transform Underbranch;
    public Transform Branches;
    public Transform Top;

    private Transform Wall1;
    private Transform Wall2;
    private Transform Wall3;

    public int seed;
    public int nbOfFreeCorners; // the number of corner that are not against another wall but inside the room

    private int tmpSeed;
    private int tmpsNbOfFreeCorners;

    // Start is called before the first frame update
    void Start()
    {
        Wall1 = Walls.transform.Find("1Wall");
        Wall2 = Walls.transform.Find("2Walls");
        Wall3 = Walls.transform.Find("3Walls");

        SetArt();
    }

    // Update is called once per frame
    void Update()
    {
        if(seed!=tmpSeed || nbOfFreeCorners != tmpsNbOfFreeCorners)
        {
            SetArt();
        }
        tmpSeed = seed;
        tmpsNbOfFreeCorners = nbOfFreeCorners;
    }

    public void SetArt()
    {
        // --------------------
        // Clean Art
        // --------------------
        List<Transform> artContainers = new List<Transform> { Wall1, Wall2, Wall3, Underbranch, Branches, Top };
        foreach (Transform container in artContainers)
        {
            for (int i = 0; i < container.transform.childCount; i++)
            {
                container.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        

        // --------------------
        // Get Art
        // --------------------

        // 1 Wall
        GameObject wallArt = null;
        if (nbOfFreeCorners == 1)
        {
            int wallIndex = Mathf.FloorToInt(Random.value * Wall1.childCount);
            wallArt = Wall1.GetChild(wallIndex).gameObject;
        }
        else if (nbOfFreeCorners == 2)
        {
            int wallIndex = Mathf.FloorToInt(Random.value * Wall2.childCount);
            wallArt = Wall2.GetChild(wallIndex).gameObject;
        }
        else
        {
            int wallIndex = Mathf.FloorToInt(Random.value * Wall3.childCount);
            wallArt = Wall3.GetChild(wallIndex).gameObject;
        }

        // 2 Underbranch
        GameObject underbranchArt = null;
        int underbranchIndex = Mathf.FloorToInt(Random.value * Underbranch.childCount);
        underbranchArt = Underbranch.GetChild(underbranchIndex).gameObject;

        // 3 Branches
        GameObject branchArt = null;
        int branchIndex = Mathf.FloorToInt(Random.value * Branches.childCount);
        branchArt = Branches.GetChild(underbranchIndex).gameObject;

        // 4 Top
        GameObject topArt = null;
        float rand = Random.value;
        if (rand > 0.7f)
        {
            int randomChildIdx = Random.Range(0, Top.transform.childCount);
            topArt = Top.GetChild(randomChildIdx).gameObject;
        }

        // --------------------
        // Set Art
        // --------------------
        wallArt.SetActive(true);
        underbranchArt.SetActive(true);
        branchArt.SetActive(true);
        if(topArt !=null)
            topArt.SetActive(true);


        // --------------------
        // Rotate Art
        // --------------------
        // TODO: Rotate the art empties to align to the correct way the wall is facing....
    }
}
