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
    public float flyPrepareTime;
    public float flyTime;
    public float enemyGapTime;    
    public float flySpawnOffset;
    public float bossDeathTime;

    private Transform body;
    private Transform tongue;
    private Transform tongueSprite;
    private Transform tongueHitbox;
    private int turnAngle;
    private float coolTime;
    public bool canAttack;

    void Start()
    {
        GameManager.instance.frog = this;
        body = transform.GetChild(0);
        tongue = transform.GetChild(0).GetChild(0);
        tongueSprite = transform.GetChild(0).GetChild(0).GetChild(0);
        tongueHitbox = transform.GetChild(0).GetChild(0).GetChild(1);
        canAttack = false;
    }

    void Update()
    {
        coolTime = Random.Range(2f, 4f);

        if (!canAttack)
            return;

        int attackChoice = Random.Range(0, 4);

        if (attackChoice == 0)
            StabAttack();

        else if (attackChoice == 1)
            LeftWhip();

        else if (attackChoice == 2)
            RightWhip();

        else if (attackChoice == 3)
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
        while (enemyIndex < enemiesToSpawn.Count)
        {
            Instantiate(enemiesToSpawn[enemyIndex], transform.position + flySpawnOffset * Vector3.down, Quaternion.identity);
            enemyIndex++;
            //Debug.Log(enemyIndex);
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
        FindObjectOfType<AudioManager>().PlayForest();
        GameManager.instance.panels.Win();
        GameManager.instance.playerScript.godMode = true;
        GameManager.instance.playerScript.canMove = false;
        GameManager.instance.playerScript.canShoot = false;
        GameManager.instance.canReturn = true;
    }
}

