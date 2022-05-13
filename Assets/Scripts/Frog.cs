using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{
    
    public Animator anim;

    public float stabTime;
    public float tongueWhipTime;    
    public float flyTime;
    public float stabDist;
    public float coolTime;

    private Transform body;
    private Transform tongue;
    private Transform tongueHitBox;
    private bool canAttack;

    void Start()
    {
        body = transform.GetChild(0);
        tongue = transform.GetChild(0).GetChild(0);
        tongueHitBox = transform.GetChild(0).GetChild(0).GetChild(1);
        canAttack = true;
    }

    void Update()
    {
        if (!canAttack)
            return;

        //center stab
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StabAttack();

        //left stab
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TurnTongueLeft();
            StabAttack();
            Invoke("TurnTongueRight", stabTime);
        }

        //right stab
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TurnTongueRight();
            StabAttack();
            Invoke("TurnTongueLeft", stabTime);
        }


        //left whip
        if (Input.GetKeyDown(KeyCode.Alpha4))
            anim.SetTrigger("tongueWhip");


        //right whip
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //FlipBody();
            anim.SetTrigger("tongueWhipRight");
            //Invoke("FlipBody", tongueWhipTime);
        }


        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            anim.SetTrigger("flyAttack");
        }
    }

    void StabAttack()
    {
        anim.SetTrigger("tongueAttack");
        //StartCoroutine(Stab());
        canAttack = false;
        Invoke("EndCoolTime", coolTime);
    }

    /*IEnumerator Stab(bool down = true)
    {
        float inverseTime = 2 / stabTime;
        Vector3 targetPosition = tongueHitBox.transform.position - stabDist * (down ? Vector3.down : Vector3.up);

        float sqrRemainingDistance = (tongueHitBox.transform.position - targetPosition).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(tongueHitBox.transform.position, targetPosition, inverseTime * Time.deltaTime);
            tongueHitBox.transform.position = newPosition;
            sqrRemainingDistance = (tongueHitBox.transform.position - targetPosition).sqrMagnitude;
            yield return null;
        }       

        StartCoroutine(Stab(false));
    }*/

    void FlipBody()
    {
        body.GetComponent<SpriteRenderer>().flipX = !body.GetComponent<SpriteRenderer>().flipX;
        //tongue.GetComponent<SpriteRenderer>().flipX = !tongue.GetComponent<SpriteRenderer>().flipX;
    }

    void TurnTongueLeft()
    {
        tongue.transform.Rotate(Vector3.back, 30);
    }

    void TurnTongueRight()
    {
        tongue.transform.Rotate(Vector3.back, -30);
    }

    void EndCoolTime()
    {
        canAttack = true;
        Debug.Log("can attack");
    }
}
