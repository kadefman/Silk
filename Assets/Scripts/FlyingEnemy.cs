using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public enum Type {Fly, Bee};
    public Type type;

    public Transform sprite;
    public GameObject FxDiePrefab;
    /*public AudioSource audioSource;
    public AudioClip damageSFX;
    public AudioClip deathSFX;*/

    public bool dropSilkInPlace;
    public float speed;
    public float dropRate;
    public int damage;
    public int health;
    public int silkReward;

    private Collider2D coll;
    private Rigidbody2D rb;
    private Vector3 position;

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        coll = transform.GetComponent<Collider2D>();
        Vector2 randVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForce(speed * randVector.normalized);
    }
    void Update()
    {
        if(sprite != null) {
            var lookAtPoint = sprite.position+ new Vector3(rb.velocity.x, rb.velocity.y, 0);
            lookAtPoint.z = sprite.position.z;
            float angle = 0;

            Vector3 relative = transform.InverseTransformPoint(lookAtPoint);
            angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            transform.Rotate(0, 0, -angle);
        }
        
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
            /*audioSource.pitch = Random.Range(.95f, 1.1f);
            audioSource.PlayOneShot(damageSFX, .8f);*/
        }       
    }
    IEnumerator Die()
    {
        //game
        Destroy(transform.GetChild(0).gameObject);

        GameManager.instance.enemyCount--;
        if (GameManager.instance.enemyCount == 0)
            GameManager.instance.OpenDoor(false);

        if (dropSilkInPlace)
        {
            float dropRoll = Random.Range(0f, 1f);
            if(dropRoll<=dropRate)
                Instantiate(GameManager.RandomObject(GameManager.instance.items), transform.position, Quaternion.identity);
        }
        else
            GameManager.instance.AddSilk(silkReward);
        

        //audio
        /*audioSource.pitch = Random.Range(.95f, 1.1f);
        audioSource.PlayOneShot(deathSFX, .8f);*/

        //anim
        rb.velocity = Vector3.zero;
        Animator animator = GetComponentInChildren<Animator>();
        position = gameObject.transform.position;
        FindObjectOfType<AudioManager>().PlaySpatial("Enemy Death", position);
        animator.Play("Base Layer.Die");
        Instantiate(FxDiePrefab, transform);

        yield return new WaitForSeconds(2);

        Destroy(gameObject);
        
    }
}
