using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{
    
    public Animator anim;
    public List<GameObject> enemiesToSpawn;

    public int maxHealth;   
    public float stabTime;
    public float tongueWhipTime;
    public float flyPrepareTime;
    public float flyTime;
    public float enemyGapTime;
    public float coolTime;
    public float bossDeathTime;
    public float flySpawnOffset;

    public int health;

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
        //HideHitbox();
        canAttack = true;
        health = maxHealth;
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

        if (Input.GetKeyDown(KeyCode.Alpha4))
            FlyAttack();

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //FlipBody();
            anim.SetTrigger("tongueWhipRight");
            //Invoke("FlipBody", tongueWhipTime);
        }       
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
        //FlipBody();
        anim.SetTrigger("tongueWhipRight");
        //Invoke("FlipBody", tongueWhipTime);
        Invoke("HideHitbox", tongueWhipTime);
        Invoke("EndCoolTime", coolTime);
    }

    void FlyAttack()
    {
        tongueHitbox.gameObject.SetActive(true);
        canAttack = false;
        anim.SetTrigger("flyAttack");
        StartCoroutine(PrepareFlies());
        Invoke("EndCoolTime", coolTime);
    }

    void TurnTongue()
    {
        tongue.transform.Rotate(Vector3.back, turnAngle);
    }

    IEnumerator PrepareFlies()
    {
        yield return new WaitForSeconds(flyPrepareTime);
        StartCoroutine(SpawnFlies());
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

    public void HideHitbox()
    {
        tongueHitbox.gameObject.SetActive(false);
        //tongueHitbox.GetComponent<SpriteRenderer>().enabled = false;
    }

    void EndCoolTime()
    {
        canAttack = true;
        Debug.Log("can attack");
    }

    public IEnumerator Die()
    {
        anim.SetTrigger("death");
        yield return new WaitForSeconds(bossDeathTime);
        Destroy(gameObject);
        GameManager.instance.panels.Win();
    }
}
