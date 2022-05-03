using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public int silkCount;
    [HideInInspector] public int healthCount;
    [HideInInspector] public bool spinning;
    
    public float speed;
    public int silkStart;   
    public int healthStart;   

    private Rigidbody2D rb;
    
    void Start()
    {
        GameManager.instance.playerScript = this;
        rb = transform.GetComponent<Rigidbody2D>();
        spinning = false;

        AddSilk(silkStart);
        silkCount = silkStart;
        AddHealth(healthStart);
        healthCount = healthStart;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!spinning)
                SpinWeb();
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            spinning = false;
        }

       /* if(Input.GetKeyDown(KeyCode.Z))
        {
            Shoot();
        }*/
    }

    private void FixedUpdate()
    {
        if (!spinning)
        {
            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }
    }

    public void AddSilk(int i)
    {
        silkCount += i;
        GameManager.instance.silkText.text = $"Silk: {silkCount}";
    }

    public void AddHealth(int i)
    {
        healthCount += i;
        GameManager.instance.healthText.text = $"Health: {healthCount}";
    }

    public void SpinWeb()
    {
        if(GameManager.instance.webTile != null)
            GameManager.instance.StartCoroutine("SpinCountdown", silkStart);
        else
            Debug.Log("Could not spin web");
    }

    
}
