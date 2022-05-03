using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance = null;
    [HideInInspector] public Player playerScript;
    [HideInInspector] public Transform sensor;
    [HideInInspector] public Transform webTile;

    public GameObject web;
    public GameObject webCounter;
    public TextMeshProUGUI silkText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI spinText;
    
    public bool saveWebProgress;
    public float spinTime;
    public int spinCost;

    //this will be changed
    public int enemyCount;
    public GameObject door;
    public GameObject emp;

    private float inverseSpinTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        spinText.gameObject.SetActive(false);
        inverseSpinTime = 1f / spinTime;
    }

    private void Update()
    {
        if (enemyCount == 0 && door != null)
        {
            Vector2 doorPos = door.transform.position;
            Destroy(door);
            Instantiate(emp, doorPos, Quaternion.identity);
            door = null;
        }           
    }

    public IEnumerator SpinCountdown(int startSilk)
    {
        playerScript.spinning = true;
        spinText.gameObject.SetActive(true);
        spinText.text = $"Spinning web...";

        float silkSpent = 0f;
        int silkUnitsSpent = 0;
        bool cancelled = false;

        Transform tile = webTile;
        GameObject counter;

        if (tile.childCount == 0)
        {
            counter = Instantiate(webCounter, tile.transform.position, Quaternion.identity, tile);
            counter.GetComponent<TextMeshPro>().text = spinCost.ToString();
        }
            
        else
        {
            counter = tile.GetChild(0).gameObject;
            silkUnitsSpent = spinCost - int.Parse(counter.GetComponent<TextMeshPro>().text);
        }
                           
        while (silkUnitsSpent<spinCost)
        {
            if (!playerScript.spinning || playerScript.silkCount <=0)
            {
                if (!saveWebProgress)
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

        spinText.gameObject.SetActive(false);

        if (!cancelled)
        {
            CreateWeb(tile);
            playerScript.spinning = false;
            sensor.GetComponent<Renderer>().enabled = false;
            sensor = null;
        }             
    }

    private void CreateWeb(Transform spot)
    {
        Vector2 position = spot.transform.position;
        Destroy(spot.gameObject);
        Instantiate(web, position, Quaternion.identity, transform);
    }

}
