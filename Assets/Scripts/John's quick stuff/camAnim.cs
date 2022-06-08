using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camAnim : MonoBehaviour
{
    public Animator anim;
    public Vector3 position;
    public GameObject latch;
    public GameObject player;
    // Start is called before the first frame update
    
    public void smoothMove()
    {
        //position = transform.position;
        //gameObject.transform.parent = null;
        //gameObject.transform.position = position;
        //Debug.Log(position);

        anim.SetTrigger("move");
        StartCoroutine(enterancewait());
        
    }

    IEnumerator enterancewait()
    {
        latch = GameObject.FindGameObjectWithTag("Latch");
        player = GameObject.FindGameObjectWithTag("Player");
        latch.transform.position = player.transform.position;
        yield return new WaitForSeconds(.2f);
        gameObject.transform.parent = latch.transform;


    }
}
