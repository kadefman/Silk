using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    //each small array must add to 1
    //7 enemies: stillFly, movingFly, bee, angryBee, fastFly, bigBee, bigAngryBee
    //8 items: nothing, silk, health, currency, bigSilk, bigHealth, bigCurrency, powerup
    public static float[,] itemRarities = new float[7, 8]
    //stillFly
    {{.3f, .4f, .2f, .1f, 0, 0, 0, 0},
    //movingFly
    {.3f, .4f, .2f, .1f, 0, 0, 0, 0},
    //bee
    {.2f, .35f, .15f, .2f, 0, 0, 0, .1f},
    //angryBee
    {.2f, .2f, .1f, .2f, .1f, .1f, 0, .1f},
    //fastFly
    {.15f, .05f, .05f, .2f, .2f, .2f, 0, .15f},
    //bigBee
    {0, 0, 0, .4f, .2f, .2f, .1f, .15f},
    //bigAngryBee
    {0, 0, 0, .2f, .2f, .2f, .2f, .3f}};


    public Transform sprite;
    public GameObject FxDiePrefab;
    /*public AudioSource audioSource;
    public AudioClip damageSFX;
    public AudioClip deathSFX;*/

    public int enemyIndex;
    public float speed;   
    public int damage;
    public int health;

    private Collider2D coll;
    private Rigidbody2D rb;

    private int sfxNum;
    private string sfxString;

    //public Transform sprite;
    //public GameObject FxDiePrefab;
    private Vector3 position;

    public float seekSpeed;
    private Transform target;
    public bool isSeeker = false;
    public bool isStill = false;
    private float rotationModifier = 90;

    private float[] dropRates;
    private float xVelocity;
    private float yVelocity;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = transform.GetComponent<Rigidbody2D>();
        coll = transform.GetComponent<Collider2D>();
        Vector2 randVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForce(speed * randVector.normalized);

        dropRates = new float[8];
        for(int i=0; i<dropRates.Length; i++)
        {
            dropRates[i] = itemRarities[enemyIndex,i];
        }

    }
    void Update()
    {
        if (isStill)
            return;

        if(sprite != null) {
            if (isSeeker == false)
            {
                var lookAtPoint = sprite.position + new Vector3(rb.velocity.x, rb.velocity.y, 0);
                lookAtPoint.z = sprite.position.z;
                float angle = 0;

                Vector3 relative = transform.InverseTransformPoint(lookAtPoint);
                angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                transform.Rotate(0, 0, -angle);

                if (rb.velocity.magnitude < .5)
                {
                    Vector2 randVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    rb.AddForce((.3f * speed) * randVector.normalized);
              
                }
            }
            
            if (isSeeker)
            {
                if (rb.velocity.magnitude > seekSpeed)
                {
                    rb.velocity = new Vector2(.1f, .1f);
                    
                }
                target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                transform.position = Vector2.MoveTowards(transform.position, target.position, seekSpeed * Time.deltaTime);
                Vector3 vectorToTarget = target.transform.position - transform.position;
                float seekAngle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
                Quaternion q = Quaternion.AngleAxis(seekAngle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
            }
                


        }
        
        /*if (isSeeker)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, seekSpeed * Time.deltaTime);
            //transform.LookAt(target);
        }*/

    }

    public void GetHit(int i)
    {
        health -= i;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }

        else if(i>0)
        {
            //Debug.Log("Maybe sounds?");
            position = gameObject.transform.position;
            FindObjectOfType<AudioManager>().PlaySpatial("Enemy hit 1", position, 1f);

            /*audioSource.pitch = Random.Range(.95f, 1.1f);
            audioSource.PlayOneShot(damageSFX, .8f);*/
        }       
    }

    private void DropItem()
    {
        //which item to place
        float itemRoll = Random.Range(0f, 1f);
        int itemIndex = 0;

        Debug.Log("Killed enemy, rolled " + itemRoll);
        string arrString = "";

        for (int i = 0; i < dropRates.Length; i++)
        {
            itemRoll -= dropRates[i];
            arrString += dropRates[i] + " ";
            if (itemRoll <= 0)
            {
                itemIndex = i;
                break;
            }
                
        }

        Debug.Log(arrString + "we got index " + itemIndex);

        if(itemIndex < 7 && itemIndex > 0)
            Instantiate(GameManager.instance.items[itemIndex-1], transform.position, Quaternion.identity);
        
        //random powerup, uniform
        else if(itemIndex == 7)
        {
            int powerupIndex = Random.Range(0, GameManager.instance.powerupsRemaining.Count);
            GameObject powerup = GameManager.instance.powerupsRemaining[powerupIndex];
            Instantiate(powerup, transform.position, Quaternion.identity);
            GameManager.instance.powerupsRemaining.Remove(powerup);
        }
    }

    IEnumerator Die()
    {
        Destroy(transform.GetChild(0).gameObject);

        GameManager.instance.enemyCount--;
        if (GameManager.instance.enemyCount == 0)
            GameManager.instance.OpenDoor(false);

        DropItem();
           

        //audio
        /*audioSource.pitch = Random.Range(.95f, 1.1f);
        audioSource.PlayOneShot(deathSFX, .8f);*/

        //anim
        rb.velocity = Vector3.zero;
        Animator animator = GetComponentInChildren<Animator>();
        position = gameObject.transform.position;
        sfxNum = Random.Range(1, 4);
        sfxString = sfxNum.ToString();
        FindObjectOfType<AudioManager>().PlaySpatial("Enemy Death " + sfxString, position, .95f);
        animator.Play("Base Layer.Die");
        Instantiate(FxDiePrefab, transform);

        yield return new WaitForSeconds(2);

        Destroy(gameObject);
        
    }
}
