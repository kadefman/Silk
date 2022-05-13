using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{ 
    public GameObject tongueHitBox;   
    public Animator anim;

    //public float turnTime;
    private Vector2 tongueOrigin;
    public float tongueOutTime;
    public float tongueInTime;   
    public float tongueDistance;

    
    private bool attacking;
    private bool turning;
    private float inverseTurnTime;
    private float inverseOutTime;
    private float inverseInTime;    

    void Start()
    {
        attacking = false;
        turning = false;
        inverseOutTime = 1 / tongueOutTime;
        inverseInTime = 1 / tongueInTime;
        tongueOrigin = tongueHitBox.transform.position;
    }

    void Update()
    {
        if (attacking || turning)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            anim.SetTrigger("tongueAttack");

        //left
        if (Input.GetKeyDown(KeyCode.Alpha2))
            anim.SetTrigger("tongueWhip");

        //right
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FlipTongue();
            anim.SetTrigger("tongueWhip");
            Invoke("FlipTongue", .08f);
        }
            

    }

    void FlipTongue()
    {
        Debug.Log("flipTongue");
        transform.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
        //transform.GetChild(3).GetComponent<SpriteRenderer>().flipX = true;
    }

    //this should be gradual but it's 3am lol
    private void Turn()
    {
        int clockWiseFlip = Random.Range(0, 2);
        bool clockwise = clockWiseFlip == 0;

        transform.Rotate(Vector3.back, clockwise ? 30f : -30f);
        tongueOrigin = tongueHitBox.transform.position;


        turning = false;
    }

}
