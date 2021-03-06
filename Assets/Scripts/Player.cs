using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GameObject[] bullets;
    public float defaultSpeed;
    public int silkStart;
    public int maxHealth;
    //public int spinCost;
    public int shotCost;
    public float shotCoolTime;
    public float spinTime;
    public float invincibilityTime;
    public bool saveWebProgress;
       
    public AudioSource audioSource; //for spinning
    public AudioSource walkingSource; //for walking
    public AudioClip[] walkingClips;
    public Animator animator;
    public Animator animator2;
    public GameObject FxDiePrefab;

    [HideInInspector] public int silkCount;
    [HideInInspector] public int healthCount;
    [HideInInspector] public float speed;
    [HideInInspector] public bool spinning;
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool godMode;
    [HideInInspector] public bool invincible;

    public bool piercing;
    public bool tripleShot;
    public bool bigBullets;
    public bool longRange;
    
    private Rigidbody2D rb;
    private Vector2 curDirection;
    private float bulletOffset = .08f;
    

    private Collider2D playerCollider;

    SpriteRenderer sp;
    private Vector3 positionBeforeSpinSnap;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }

    void Start()
    {
        GameManager.instance.playerScript = this;
        GameManager.instance.ResetPowerups();
        GameManager.instance.runCount++;

        rb = transform.GetComponent<Rigidbody2D>();
        spinning = false;
        godMode = false;
        canMove = true;
        canShoot = true;
        invincible = false;
        curDirection = Vector2.down;

        
        sp = gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<CapsuleCollider2D>();

        //Debug.Log(sp);

        speed = defaultSpeed;

        AddSilk(silkStart);
        silkCount = silkStart;

        AddHealth(maxHealth);
        healthCount = maxHealth;

        //Invoke("TriggerUI", .1f);            
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canMove && (silkCount > 0 || godMode))
                SpinWeb();
            if (spinning)
                audioSource.Play();
        }
       
        if (Input.GetKeyUp(KeyCode.Space))
        {
            spinning = false;
            audioSource.Stop();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (canMove && canShoot)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10;
                Vector2 dir = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
                Shoot(dir);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(GameManager.instance.canReset)
            {
                GameManager.instance.ResetUpgrades();
                GameManager.instance.panels.HidePanels();
                GameManager.instance.panels.PostReset();
                GameManager.instance.canReset = false;
            }

            if(GameManager.instance.canReturn)
            {
                GameManager.instance.canReturn = false;
                GameManager.instance.playerScript.godMode = false;
                GameManager.instance.playerScript.canMove = true;
                GameManager.instance.playerScript.canShoot = true;
                GameManager.instance.panels.HidePanels();
                SceneManager.LoadScene(1);
            }
        }
       
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            ToggleGodMode();

        if (Input.GetKeyDown(KeyCode.RightBracket))
            GameManager.instance.AddCurrency(10);                
    }

    private void FixedUpdate()
    {
        if (canMove)
        {           
            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

            if (movement == Vector2.zero)
            {
                animator.SetBool("isMoving", false);
                walkingSource.Stop();
            }

            else
            {
                animator.SetBool("isMoving", true);
                //walkingSource.clip = walkingClips[Random.Range(0, walkingClips.Length)];
                if (!walkingSource.isPlaying)
                {
                    walkingSource.clip = walkingClips[Random.Range(0, walkingClips.Length)];
                    walkingSource.Play();
                }
                

                if (movement.y == 1)
                {
                    curDirection = Vector2.up;
                    animator.SetFloat("horizontalMovement", 0f);
                    animator.SetFloat("verticalMovement", 1f);
                }

                else if (movement.x == 1)
                {
                    curDirection = Vector2.right;
                    sp.flipX = true;
                    animator.SetFloat("horizontalMovement", -1f);
                }
                else if (movement.x == -1)
                {
                    curDirection = Vector2.left;
                    sp.flipX = false;
                    animator.SetFloat("horizontalMovement", 1f);
                }

                else if (movement.y == -1)
                {
                    curDirection = Vector2.down;
                    sp.flipX = false;
                    animator.SetFloat("verticalMovement", -1f);
                    animator.SetFloat("horizontalMovement", 0f);
                }
            }
        }
    }

    private void ToggleGodMode()
    {
        Debug.Log("wheeeee!");
        if(!godMode)
        {
            godMode = true;
            speed = 6;
            shotCost = 0;
        }

        else
        {
            godMode = false;
            speed = defaultSpeed;
        }
    }

    public void AddSilk(int i)
    {
        //Debug.Log("Adding silk");
        silkCount += i;
        GameManager.instance.silkText.text = silkCount.ToString();

        if (silkCount < 20)
        {
            GameManager.instance.silkText.color = Color.red;
            GameManager.instance.canvas.transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(true);
            GameManager.instance.canvas.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
        }

        else
        {
            GameManager.instance.silkText.color = Color.white;
            GameManager.instance.canvas.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
            GameManager.instance.canvas.transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);
        }

    }


    public void AddHealth(int i)
    {
        if (godMode && i < 0)
            return;

        healthCount += i;

        //maxHealth is changed here if necessary
        GameManager.instance.SetHealth();
      
        if (i < 0)
        {
            animator.SetTrigger("Hit");
            FindObjectOfType<AudioManager>().Play("Player hurt");
        }

        if (healthCount <= 0)
        {
            Die();
        }
    }

    public void SpinWeb()
    {
        if(GameManager.instance.webTile != null)
        {
            if (godMode)
                GameManager.instance.CreateWeb(GameManager.instance.webTile);
            else
                GameManager.instance.StartCoroutine("SpinCountdown", silkStart);
        }           
    }

    public void Shoot(Vector2 dir)
    {
        GameObject bulletPrefab = piercing ? bullets[1] : bullets[0];
        int bulletCount = tripleShot ? 3 : 1;
        Vector2[] shotDirections = new Vector2[] { dir, Quaternion.AngleAxis(20, Vector3.forward) * dir, 
            Quaternion.AngleAxis(-20, Vector3.forward) * dir };

        for (int i = 0; i < bulletCount; i++)
        {
            Vector2 bulletDirection = shotDirections[i];
            GameObject newBullet = Instantiate(bulletPrefab, (Vector2)transform.position + bulletDirection * bulletOffset, 
                Quaternion.LookRotation(Vector3.back, bulletDirection));

            Bullet bulletScript = newBullet.transform.GetComponent<Bullet>();
            bulletScript.damage = GameManager.instance.baseDamage;

            if (bigBullets)
            {
                newBullet.transform.localScale = new Vector3(2, 2, 1);
                bulletScript.damage *= 2;
            }
                
            if (longRange)
                bulletScript.longRange = true;
        }

        AddSilk(-shotCost);
        StartCoroutine(Cooldown(shotCoolTime));
        FindObjectOfType<AudioManager>().Play("Web shot 1");
        animator2.SetTrigger("Shoot");

        if(silkCount<=0)
        {
            SacrificeHealth();
        }
    }

    public void SacrificeHealth()
    {
        silkCount = 0;
        AddSilk(20);
        AddHealth(-1);
    }

    private void Die()
    {
        // Do stuff when dying
        FindObjectOfType<AudioManager>().Play("Player Death");
        if (GameManager.instance.roomIndex == GameManager.instance.bossRoomIndex)
        {
            FindObjectOfType<AudioManager>().PlayForest();
        }
        walkingSource.Stop();
        
        canMove = false;
        animator.Play("Base Layer.Die");
        Instantiate(FxDiePrefab, transform);
        StartCoroutine(scenewait());
        playerCollider.enabled = !playerCollider.enabled;

        foreach(GameObject web in GameManager.instance.webs)
            Destroy(web);
            
        GameManager.instance.webs.Clear();
    }

    public IEnumerator Cooldown (float time)
    {
        canShoot = false;
        float timer = time;

        while(timer>=float.Epsilon)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        canShoot = true;
    }
    public IEnumerator SpinSnap( bool getIn, bool skip, Transform tile)
    {
        /*
         * (bool) getIn: If true the function makes the player snap inside the web
         * (bool) skip: If get In, if true skip the cross dash animation, if not get in skip the teleportation
         */
        SpriteRenderer sp = GetComponentInChildren<SpriteRenderer>();
        Collider2D tileCollider = tile.gameObject.GetComponent<Collider2D>();

        if (!(!getIn && skip))
        {
            // Hide Player
            Color transparent = sp.color;
            transparent.a = 0;
            sp.color = transparent;

            // Tile Collider disable
            tileCollider.enabled = false;

            float duration = 0.1f;
            Vector3 targetPosition = Vector3.zero;
            if (getIn)
            {
                targetPosition = tile.transform.position;
                positionBeforeSpinSnap = transform.position;
            }
            else
                targetPosition = positionBeforeSpinSnap;


            // Lerp Position 
            float time = 0;
            Vector3 startPosition = transform.position;
            while (time < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;

            // Show Player
            transparent.a = 1;
            sp.color = transparent;
        }
        // Trigger Anims
        if (getIn) { 
            if (skip)
            {
                animator.Play("Base Layer.SpinBegin");
            }
            else
            {
                animator.Play("Base Layer.SpinDashes");
            }
        }
        else
        {
            if (!skip)
            {
                animator.Play("Base Layer.Movement");
                tileCollider.enabled = true;
            }
            else
            {
                animator.Play("Base Layer.Movement");
                tileCollider.enabled = true;
            }
            canMove = true;
        }
    }

    IEnumerator scenewait()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(1);
    }

    public void Invincible()
    {
        //Debug.Log("invincible");
        invincible = true;
        Invoke("NotInvincible", invincibilityTime);
    }

    public void NotInvincible()
    {
        invincible = false;
    }
}
