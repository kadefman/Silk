using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject web;
    public GameObject webCounter;
    public GameObject platform;
    public TextMeshProUGUI silkText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI spinText;

    [HideInInspector] public static GameManager instance = null;
    public List<Room> rooms;
    public Room currentRoom;
    [HideInInspector] public Player playerScript;
    [HideInInspector] public Transform sensor;
    [HideInInspector] public Transform webTile;
    [HideInInspector] public int enemyCount;
    [HideInInspector] public int roomIndex;
      
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        spinText.gameObject.SetActive(false);
    }

    public void SetRoom(int index)
    {
        if (rooms != null && rooms.Count != 0)
        {
            Debug.Log($"Entering Room {index}");
            Debug.Log(rooms.Count);
            roomIndex = index;
            currentRoom = rooms[roomIndex];
            enemyCount = currentRoom.enemyCount;
        }

        else
            roomIndex = -1;
    }

    public IEnumerator SpinCountdown(int startSilk)
    {
        playerScript.spinning = true;
        spinText.gameObject.SetActive(true);
        spinText.text = $"Spinning web...";

        float silkSpent = 0f;
        float inverseSpinTime = 1f / playerScript.spinTime;
        int silkUnitsSpent = 0;
        bool cancelled = false;

        Transform tile = webTile;
        GameObject counter;

        if (tile.childCount == 0)
        {
            counter = Instantiate(webCounter, tile.transform.position, Quaternion.identity, tile);
            counter.GetComponent<TextMeshPro>().text = playerScript.spinCost.ToString();
            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(true, false, tile));
        }
            
        else
        {
            counter = tile.GetChild(0).gameObject;
            silkUnitsSpent = playerScript.spinCost - int.Parse(counter.GetComponent<TextMeshPro>().text);
            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(true, true, tile));
        }
                           
        while (silkUnitsSpent< playerScript.spinCost)
        {
            if (!playerScript.spinning || playerScript.silkCount <=0)
            {
                if (!playerScript.saveWebProgress)
                    Destroy(counter);

                cancelled = true;
                break;
            }

            silkSpent += playerScript.spinCost * inverseSpinTime * Time.deltaTime;
            if (silkSpent > 1f)
            {
                silkSpent -= 1f;
                silkUnitsSpent += 1;
                counter.GetComponent<TextMeshPro>().text = (playerScript.spinCost - silkUnitsSpent).ToString();
                playerScript.AddSilk(-1);
            }
            
            yield return null;
        }

        spinText.gameObject.SetActive(false);

        //knockback?
        if(cancelled)
        {
            Vector2 knockBackDir = playerScript.transform.position - tile.transform.position;
            float knockBackFloat = .15f;
            playerScript.transform.position = (Vector2)playerScript.transform.position + knockBackDir * knockBackFloat;
            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(false, false, tile));
        }

        else
        {
            CreateWeb(tile);
            playerScript.spinning = false;
            // sensor.GetComponent<Renderer>().enabled = false;
            // sensor = null;

            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(false, true, tile));
        }  
        
        
    }

    private void CreateWeb(Transform spot)
    {
        Vector2 position = spot.transform.position;
        Destroy(spot.gameObject);
        Instantiate(web, position, Quaternion.identity, transform);
    }

    public void OpenDoor(bool key)
    {
        Transform door;
        if (key)
            door = currentRoom.keyDoor;
        else
            door = currentRoom.enemyDoor;

        if (door != null)
        {
            Debug.Log("There's a door");
            Vector2 doorPos = door.transform.position;
            door.DetachChildren();
            Destroy(door.gameObject);
            Instantiate(platform, doorPos, Quaternion.identity);
        }
        else
            Debug.Log("No door");
    }

    //this is a duplicate method, only exists so that enemies can reward silk directly 
    //(which may not permanently be the case anyway)
    public void AddSilk(int i)
    {
        playerScript.AddSilk(i);
    }
}
