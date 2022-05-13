using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{
    public GameObject tongueHitBox;
    public Animator anim;

    public float stabTime;
    public float tongueWhipTime;    
    public float flyTime;
    public float tongueShift;

    private Transform body;
    private Transform tongue;
    private bool canAttack;

    void Start()
    {
        body = transform.GetChild(0);
        tongue = transform.GetChild(0).GetChild(0).GetChild(0);
        canAttack = true;
    }

    void Update()
    {
        if (!canAttack)
            return;

        //center stab
        if (Input.GetKeyDown(KeyCode.Alpha1))
            anim.SetTrigger("tongueAttack");

        //left stab
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TurnTongueLeft();
            anim.SetTrigger("tongueAttack");
            Invoke("TurnTongueRight", stabTime);
        }

        //right stab
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TurnTongueRight();
            anim.SetTrigger("tongueAttack");
            Invoke("TurnTongueLeft", stabTime);
        }


        //left whip
        if (Input.GetKeyDown(KeyCode.Alpha4))
            anim.SetTrigger("tongueWhip");

        //right whip
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            FlipTongue();
            anim.SetTrigger("tongueWhip");
            Invoke("FlipTongue", tongueWhipTime);
        }


        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            FlipTongue();
            anim.SetTrigger("flyAttack");
            Invoke("FlipTongue", tongueWhipTime);
        }
    }

    void FlipTongue()
    {
        body.GetComponent<SpriteRenderer>().flipX = !body.GetComponent<SpriteRenderer>().flipX;
        tongue.GetComponent<SpriteRenderer>().flipX = !tongue.GetComponent<SpriteRenderer>().flipX;
    }

    void TurnTongueLeft()
    {
        tongue.transform.Rotate(Vector3.back, 30);
        tongue.transform.position += Vector3.left * tongueShift;
    }

    void TurnTongueRight()
    {
        tongue.transform.Rotate(Vector3.back, -30);
        tongue.transform.position += Vector3.right * tongueShift;
    }

    void EndCoolTime()
    {

    }
}
