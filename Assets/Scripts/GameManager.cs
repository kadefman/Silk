using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance = null;
    [HideInInspector] public Transform sensor;
    [HideInInspector] public Transform webTile;
    [HideInInspector] public bool spinning;

    public GameObject platform;
    public TextMeshProUGUI silkText;
    public TextMeshProUGUI timerText;     
    public int silkStart;
   
    private int silkCount = 0;

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
        AddSilk(silkStart);
        timerText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            sensor.GetComponent<Renderer>().enabled = true;
    }

    public void AddSilk(int i)
    {
        silkCount += i;
        silkText.text = $"Silk: {silkCount}";
    }

    public void SpinWeb()
    {
        if (!spinning && silkCount >= 30 && webTile != null)
            StartCoroutine(Countdown());
        else
            Debug.Log("Could not spin web");
    } 

    private IEnumerator Countdown()
    {
        Transform spot = webTile;
        spinning = true;
        timerText.gameObject.SetActive(true);
        float timer = 2;

        while(timer>0)
        {
            timerText.text = $"Spinning web...{timer}";
            AddSilk(-10);
            timer--;            
            yield return new WaitForSeconds(1);
        }

        AddSilk(-10);
        timerText.gameObject.SetActive(false);
        CreateWeb(spot);
        spinning = false;
        sensor.GetComponent<Renderer>().enabled = false;
        sensor = null;
    }

    private void CreateWeb(Transform spot)
    {
        Vector2 position = spot.transform.position;
        Destroy(spot.gameObject);

        GameObject web = Instantiate(platform, position, Quaternion.identity, transform);
        web.GetComponent<GroundHex>().ChangeType(GroundHex.Type.Web);
    }

}
