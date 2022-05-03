using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public float speed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        GameManager.instance.spinning = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.SpinWeb();
        }
    }

    private void FixedUpdate()
    {
        if(!GameManager.instance.spinning)
        {
            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }
        
    }
}
