using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum ControlScheme { ArrowAuto, ArrowManual, Mouse }

    public GameObject bullet;
    public ControlScheme controls;
    public float speed;
    public int silkStart;   
    public int healthStart;
    public int spinCost;
    public int shotCost;
    public float spinTime;
    public bool saveWebProgress;

    public Animator animator;

    [HideInInspector] public int silkCount;
    [HideInInspector] public int healthCount;
    [HideInInspector] public bool spinning;
    [HideInInspector] public bool godMode;

    private Rigidbody2D rb;
    private Vector2 curDirection;
    private float bulletOffset = .08f;
    private bool setup;

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
        curDirection = Vector2.right;

        AddSilk(silkStart);
        silkCount = silkStart;
        AddHealth(healthStart);
        healthCount = healthStart;
        setup = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!spinning)
                SpinWeb();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            spinning = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
            ToggleGodMode();

        //3 variations of shooting controls
        if(!spinning && silkCount>0)
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
        if (!spinning)
        {
            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            

            /*if (isIdle)
            {
                animator.SetBool("isMoving", false);
                Debug.Log("idle");
            }
            else
            {
                animator.SetBool("isMoving", true);
                Debug.Log("moving");
            }*/

            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
            if (movement.x == 1)
            {
                curDirection = Vector2.right;
                /*animator.SetBool("isMoving", true);
                animator.SetFloat("horizontalMovement", 1f);
                Debug.Log("right");*/
            }
            else if (movement.x == -1)
            {
                curDirection = Vector2.left;
                /*animator.SetBool("isMoving", true);
                animator.SetFloat("horizontalMovement", -1f);*/
            }
            else if (movement.y == 1)
            {
                curDirection = Vector2.up;
                /*animator.SetBool("isMoving", true);
                animator.SetFloat("horizontalMovement", 0f);
                animator.SetFloat("verticalMovement", 1f);*/
            }
            else if (movement.y == -1)
            {
                curDirection = Vector2.down;
                /*animator.SetFloat("verticalMovement", -1f);
                animator.SetFloat("horizontalMovement", 0f);
                animator.SetBool("isMoving", true);*/
            }

            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.localScale = new Vector3(-1.3f, 1.3f, 1);
                animator.SetBool("isMoving", true);
                animator.SetFloat("horizontalMovement", -1f);

            }
            
            if (Input.GetKey(KeyCode.W))
            {

                animator.SetBool("isMoving", true);
                animator.SetFloat("horizontalMovement", 0f);
                animator.SetFloat("verticalMovement", 1f);
            }

            if (Input.GetKey(KeyCode.S))
            {

                animator.SetFloat("verticalMovement", -1f);
                animator.SetFloat("horizontalMovement", 0f);
                animator.SetBool("isMoving", true);
            }

            if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1);
                animator.SetBool("isMoving", true);
                animator.SetFloat("horizontalMovement", 1f);
                Debug.Log("right");
            }

            bool isIdle = movement.x == 0 && movement.y == 0;

            if (isIdle)
            {
                animator.SetBool("isMoving", false);
                //animator.SetFloat("verticalMovement", 0f);
                //animator.SetFloat("horizontalMovement", 0f);

                //Debug.Log("not moving");
            }

            //animator.SetFloat("horizontalMovement", 1);
            
        }
    }

    private void ToggleGodMode()
    {
        Debug.Log("wheeeee!");
        if(!godMode)
        {
            godMode = true;
            speed = 6;
            spinCost = 0;
            shotCost = 0;
            spinTime = 0;
        }

        else
        {
            godMode = false;
            speed = 2;
            spinCost = 30;
            spinTime = 1.8f;
        }
    } 

    //do we need to differentiate setup?
    public void AddSilk(int i)
    {
        silkCount += i;
        GameManager.instance.silkText.text = $"Silk: {silkCount}";
        if(!setup && i>0)
            StartCoroutine(ShowChange(false));
    }

    public void AddHealth(int i)
    {
        healthCount += i;
        GameManager.instance.healthText.text = $"Health: {healthCount}";
        if(!setup && i != 0)
            StartCoroutine(ShowChange(true, i>0));
    }

    public void SpinWeb()
    {
        if(GameManager.instance.webTile != null)
            GameManager.instance.StartCoroutine("SpinCountdown", silkStart);
    }

    public void Shoot(Vector2 dir)
    {
        Instantiate(bullet, (Vector2)transform.position + dir*bulletOffset, Quaternion.LookRotation(Vector3.back, dir));
        AddSilk(-shotCost);
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
}
