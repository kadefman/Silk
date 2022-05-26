using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public enum Mode { enemies, powerups, items};

    public Mode mode;

    public List<GameObject> enemies;
    public List<GameObject> powerups;
    public List<GameObject> items;

    private List<GameObject> currentList;

    private void Start()
    {
        mode = Mode.enemies;
        currentList = enemies;
        Debug.Log("Spawn enemies, 1-7");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetMode();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnObject(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnObject(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawnObject(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawnObject(3);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SpawnObject(4);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SpawnObject(5);

        if (Input.GetKeyDown(KeyCode.Alpha7))
            SpawnObject(6);
    }

    private void SetMode()
    {
        switch(mode)
        {
            case Mode.enemies:
                Debug.Log("Spawn powerups, 1-6");
                mode = Mode.powerups;
                currentList = powerups;
                break;

            case Mode.powerups:
                Debug.Log("Spawn items, 1-6");
                mode = Mode.items;
                currentList = items;
                break;

            case Mode.items:
                Debug.Log("Spawn enemies, 1-7");
                mode = Mode.enemies;
                currentList = enemies;
                break;
        }
    }

    private void SpawnObject(int i)
    {
        if (currentList.Count <= i)
            return;

        Instantiate(currentList[i], Vector2.zero, Quaternion.identity);
    }
}
