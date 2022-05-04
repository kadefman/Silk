using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private float rayStart = .2f;
    private float rayEnd = .3f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public bool IsFacingEmpty()
    {
        Vector2 startPoint = transform.position + rayStart * transform.right;
        Vector2 endPoint = transform.position + rayEnd * transform.right;
        RaycastHit2D hit = Physics2D.Linecast(startPoint, endPoint, LayerMask.GetMask("EmptyTile"));
        //Debug.DrawLine(startPoint, endPoint, Color.red, 5f);

        if (hit.transform != null && hit.transform.CompareTag("EmptyTile"))
        {
            GameManager.instance.webTile = hit.transform;
            return true;
        }
            
        else
            return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (IsFacingEmpty())
            {
                GameManager.instance.sensor = transform;
                transform.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.GetComponent<Renderer>().enabled = false;
            if (GameManager.instance.sensor == transform)
            {
                GameManager.instance.sensor = null;
                GameManager.instance.webTile = null;
            }
                
        }
    }
}
