using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{
    
    public Animator anim;
    public List<GameObject> enemiesToSpawn;

    public int health;
    public float stabTime;
    public float tongueWhipTime;    
    public float flyTime;
    public float enemyGapTime;
    public float coolTime;
    public float flySpawnOffset;

    private Transform body;
    private Transform tongue;
    private Transform tongueSprite;
    private Transform tongueHitbox;
    private int turnAngle;
    private bool canAttack;

    void Start()
    {
        body = transform.GetChild(0);
        tongue = transform.GetChild(0).GetChild(0);
        tongueSprite = transform.GetChild(0).GetChild(0).GetChild(0);
        tongueHitbox = transform.GetChild(0).GetChild(0).GetChild(1);
        HideHitbox();
        canAttack = true;
    }

    void Update()
    {
        if (!canAttack)
            return;

        //center stab
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StabAttack();

        //left whip
        if (Input.GetKeyDown(KeyCode.Alpha2))
            LeftWhip();

        //right whip
        if (Input.GetKeyDown(KeyCode.Alpha3))
            RightWhip();
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //FlipBody();
            anim.SetTrigger("tongueWhipRight");
            //Invoke("FlipBody", tongueWhipTime);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
            FlyAttack();
    }

    void StabAttack()
    {
        tongueHitbox.gameObject.SetActive(true);
        canAttack = false;        
        turnAngle = Random.Range(-15, 15);
        TurnTongue();
        turnAngle *= -1;
        anim.SetTrigger("tongueAttack");
        Invoke("TurnTongue", stabTime);
        Invoke("HideHitbox", stabTime);
        Invoke("EndCoolTime", coolTime);
    }

    void LeftWhip()
    {
        tongueHitbox.gameObject.SetActive(true);
        canAttack = false;
        anim.SetTrigger("tongueWhip");
        Invoke("HideHitbox", tongueWhipTime);
        Invoke("EndCoolTime", coolTime); 
    }
    void RightWhip()
    {
        canAttack = false;
        FlipBody();
        anim.SetTrigger("tongueWhip");
        Invoke("FlipBody", tongueWhipTime);
        Invoke("HideHitbox", tongueWhipTime);
        Invoke("EndCoolTime", coolTime);
    }

    void FlyAttack()
    {
        tongueHitbox.gameObject.SetActive(true);
        canAttack = false;
        anim.SetTrigger("flyAttack");
        StartCoroutine(SpawnFlies());
        Invoke("EndCoolTime", coolTime);
    }

    void FlipBody()
    {
        body.GetComponent<SpriteRenderer>().flipX = !body.GetComponent<SpriteRenderer>().flipX;
        //tongue.GetComponent<SpriteRenderer>().flipX = !tongue.GetComponent<SpriteRenderer>().flipX;
    }

    void TurnTongue()
    {
        tongue.transform.Rotate(Vector3.back, turnAngle);
    }

    IEnumerator SpawnFlies()
    {
        int enemyIndex = 0;
        while(enemyIndex < enemiesToSpawn.Count)
        {
            Instantiate(enemiesToSpawn[enemyIndex], transform.position + flySpawnOffset*Vector3.down, Quaternion.identity);
            enemyIndex++;
            Debug.Log(enemyIndex);
            yield return new WaitForSeconds(enemyGapTime);
        }
    }

    void HideHitbox()
    {
        Debug.Log(tongueHitbox.gameObject.name);
        tongueHitbox.gameObject.SetActive(false);
        tongueHitbox.GetComponent<SpriteRenderer>().enabled = false;
    }

    void EndCoolTime()
    {
        canAttack = true;
        Debug.Log("can attack");
    }

    public void Die()
    {
        anim.SetTrigger("death");
    }
}
