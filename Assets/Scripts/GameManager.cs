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
    public TextMeshProUGUI currencyText;
    public GameObject healthBar;
    public GameObject[] items;  
    public int[] damagePermValues;
    public int[] silkPermValues;
    public int healthUpgrades;
    public int damageUpgrades;
    public int silkUpgrades;
    public int currency;

    [HideInInspector] public static GameManager instance = null;
    [HideInInspector] public List<Room> rooms;
    [HideInInspector] public Room currentRoom;
    [HideInInspector] public Player playerScript;
    [HideInInspector] public Transform sensor;
    [HideInInspector] public Transform webTile;
    [HideInInspector] public Merchant merchant;
    [HideInInspector] public int enemyCount;
    [HideInInspector] public int roomIndex;
    [HideInInspector] public bool generating;
    [HideInInspector] public int spinCost;
    [HideInInspector] public int baseDamage = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currency = 0;
    }

    public void SetRoom(int index)
    {
        if (rooms != null && rooms.Count != 0)
        {
            Debug.Log($"Entering Room {index}");
            Debug.Log(rooms.Count + " rooms total");
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
        playerScript.canMove = false;

        float silkSpent = 0f;
        float inverseSpinTime = 1f / playerScript.spinTime;
        int silkUnitsSpent = 0;
        bool cancelled = false;

        Transform tile = webTile;
        GameObject counter;

        if (tile.childCount == 0)
        {
            counter = Instantiate(webCounter, tile.transform.position, Quaternion.identity, tile);
            counter.GetComponent<TextMeshPro>().text = spinCost.ToString();
            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(true, false, tile));
        }
            
        else
        {
            counter = tile.GetChild(0).gameObject;
            silkUnitsSpent = spinCost - int.Parse(counter.GetComponent<TextMeshPro>().text);
            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(true, true, tile));
        }
                           
        while (silkUnitsSpent< spinCost)
        {
            if (!playerScript.spinning || playerScript.silkCount <=0)
            {
                if (!playerScript.saveWebProgress)
                    Destroy(counter);

                cancelled = true;
                break;
            }

            silkSpent += spinCost * inverseSpinTime * Time.deltaTime;
            if (silkSpent > 1f)
            {
                silkSpent -= 1f;
                silkUnitsSpent += 1;
                counter.GetComponent<TextMeshPro>().text = (spinCost - silkUnitsSpent).ToString();
                playerScript.AddSilk(-1);
            }
            
            yield return null;
        }

        //knockback?
        if(cancelled)
        {
            Vector2 knockBackDir = playerScript.transform.position - tile.transform.position;
            float knockBackFloat = .15f;
            playerScript.transform.position = (Vector2)playerScript.transform.position + knockBackDir * knockBackFloat;
            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(false, false, tile));

            //player sacrifices health for silk
            if(playerScript.silkCount <=0)
            {
                playerScript.SacrificeHealth();
            }
        }

        else
        {
            CreateWeb(tile);
            playerScript.spinning = false;           

            // Player Anim
            playerScript.StartCoroutine(playerScript.SpinSnap(false, true, tile));
        }                
    }

    public void CreateWeb(Transform spot)
    {
        Vector2 position = spot.transform.position;
        Destroy(spot.gameObject);
        Instantiate(web, position, Quaternion.identity, transform);
        FindObjectOfType<AudioManager>().Play("Web complete");
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

    public void SetHealth(int health)
    {
        playerScript.maxHealth = 6 + 2 * healthUpgrades;
        for(int i=1; i<=6; i++)
        {
            Transform healthBubble = healthBar.transform.GetChild(i);
            foreach(Transform child in healthBubble)
                child.gameObject.SetActive(false);

            if (health >= 2 * i)
                healthBubble.GetChild(0).gameObject.SetActive(true);
            else if (health == 2 * i - 1)
                healthBubble.GetChild(1).gameObject.SetActive(true);
            else if (playerScript.maxHealth >= 2*i)
                healthBubble.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void AddCurrency(int i)
    {
        currency += i;
        currencyText.text = currency.ToString();
    }

    public static GameObject RandomObject(GameObject[] objects, float prob1 = 0f)
    {
        if(prob1==0f)
        {
            float itemRoll = Random.Range(0f, 1f);
            if (itemRoll <= prob1)
                return objects[0];
            else
                return objects[1];
        }

        else
        {
            int itemRoll = Random.Range(0, objects.Length);
            return objects[itemRoll];
        }       
    }

    public void ResetUpgrades()
    {
        healthUpgrades = 0;
        playerScript.healthCount = 1;
        playerScript.AddHealth(5);

        damageUpgrades = 0;
        baseDamage = damagePermValues[damageUpgrades];

        silkUpgrades = 0;
        spinCost = silkPermValues[silkUpgrades];
    }
}
