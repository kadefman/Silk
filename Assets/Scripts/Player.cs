using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum ControlScheme { ArrowAuto, ArrowManual, Mouse }

    public GameObject bullet;
    public ControlScheme controls;
    public float speedStart;
    public int silkStart;   
    public int healthStart;
    public int spinCost;
    public int shotCost;
    public float spinTime;
    public bool saveWebProgress;

    public Animator animator;
    public Animator animator2;
    public GameObject FxDiePrefab;

    [HideInInspector] public int silkCount;
    [HideInInspector] public int healthCount;
    [HideInInspector] public float speed;
    [HideInInspector] public bool spinning;
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool godMode;

    private Rigidbody2D rb;
    private Vector2 curDirection;
    private float bulletOffset = .08f;
    private bool setup;

    SpriteRenderer sp;
    private Vector3 positionBeforeSpinSnap;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }

    void Start()
    {
        setup = true;

        GameManager.instance.playerScript = this;

        rb = transform.GetComponent<Rigidbody2D>();
        spinning = false;
        godMode = false;
        canMove = true;
        curDirection = Vector2.down;

        speed = speedStart;
        AddSilk(silkStart);
        silkCount = silkStart;
        AddHealth(healthStart);
        healthCount = healthStart;
        setup = false;
        sp = gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        //Debug.Log(sp);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!spinning && canMove)
                SpinWeb();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            spinning = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
            ToggleGodMode();

        //3 variations of shooting controls
        if(!spinning && silkCount>0 && canMove)
        {
            if (controls == ControlScheme.Mouse)
            {               
                if (Input.GetMouseButtonDown(0))
                {                  
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = 10;
                    Vector2 dir = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
                    Shoot(dir);
                }                   
            }

            else if (controls == ControlScheme.ArrowAuto)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                    Shoot(curDirection);
            }

            else if (controls == ControlScheme.ArrowManual)
            {
                if (Input.GetKeyDown(KeyCode.T))
                    Shoot(Vector2.up);

                else if (Input.GetKeyDown(KeyCode.H))
                    Shoot(Vector2.right);

                else if (Input.GetKeyDown(KeyCode.G))
                    Shoot(Vector2.down);

                else if (Input.GetKeyDown(KeyCode.F))
                    Shoot(Vector2.left);
            }
        }              
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
            }

            else
            {
                animator.SetBool("isMoving", true);

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
            speed = speedStart;
        }
    } 

    public void AddSilk(int i)
    {
        silkCount += i;
        GameManager.instance.silkText.text = $"Silk: {silkCount}";
        if(!setup && i>0)
            StartCoroutine(ShowChange(false));
    }

    public void AddHealth(int i)
    {
        if (godMode && i < 0)
            return;

        healthCount += i;
        GameManager.instance.healthText.text = $"Health: {healthCount}";

        //colored circle thingy
        /*if(!setup && i != 0)
            StartCoroutine(ShowChange(true, i>0));*/

        if (i < 0)
        {
            animator.SetTrigger("Hit");
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
        Instantiate(bullet, (Vector2)transform.position + dir*bulletOffset, Quaternion.LookRotation(Vector3.back, dir));
        AddSilk(-shotCost);

        animator2.SetTrigger("Shoot");
    }

    private IEnumerator ShowChange(bool health, bool positive = true)
    {
        if (health)
            transform.GetComponent<SpriteRenderer>().color = positive ? Color.green : Color.red;
        else
            transform.GetComponent<SpriteRenderer>().color = Color.blue;

        float timer = .2f;
        while (timer >= float.Epsilon)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        transform.GetComponent<SpriteRenderer>().color = Color.black;
    }
    private void Die()
    {
        // Do stuff when dying
        canMove = false;
        animator.Play("Base Layer.Die");
        Instantiate(FxDiePrefab, transform);
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
        }

    }
}
