using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{ 
    public GameObject tongueHitBox;
    //public float turnTime;
    public float tongueOutTime;
    public float tongueInTime;   
    public float tongueDistance;

    private Vector2 tongueOrigin;
    private bool attacking;
    private bool turning;
    private float inverseTurnTime;
    private float inverseOutTime;
    private float inverseInTime;
    
    

    void Start()
    {
        attacking = false;
        turning = false;
        inverseOutTime = 1 / tongueOutTime;
        inverseInTime = 1 / tongueInTime;
        tongueOrigin = tongueHitBox.transform.position;
    }

    void Update()
    {
        if (attacking || turning)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Turn();

        else if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(TongueOut());

    }

    //this should be gradual but it's 3am lol
    private void Turn()
    {
        int clockWiseFlip = Random.Range(0, 2);
        bool clockwise = clockWiseFlip == 0;

        transform.Rotate(Vector3.back, clockwise ? 60f : -60f);
        tongueOrigin = tongueHitBox.transform.position;


        turning = false;
    }

    private IEnumerator TongueOut()
    {
        Debug.Log("Tongue Out");
        attacking = true;
        Vector2 startPoint = tongueHitBox.transform.position;
        Vector2 endPoint = startPoint + -(Vector2)transform.up * tongueDistance;
        float sqrRemainingDistance = (startPoint - endPoint).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(tongueHitBox.transform.position, endPoint, inverseOutTime * Time.deltaTime);
            tongueHitBox.transform.position = newPosition;
            sqrRemainingDistance = ((Vector2)newPosition - endPoint).sqrMagnitude;
            Debug.Log(sqrRemainingDistance);
            yield return null;
        }

        tongueHitBox.transform.position = endPoint;
        StartCoroutine(TongueIn());
    }

    private IEnumerator TongueIn()
    {
        Debug.Log("Tongue In");
        attacking = true;
        Vector2 startPoint = tongueHitBox.transform.position;
        Vector2 endPoint = tongueOrigin;
        float sqrRemainingDistance = (startPoint - endPoint).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(tongueHitBox.transform.position, endPoint, inverseInTime * Time.deltaTime);
            tongueHitBox.transform.position = newPosition;
            sqrRemainingDistance = ((Vector2)newPosition - endPoint).sqrMagnitude;
            yield return null;
        }

        tongueHitBox.transform.position = tongueOrigin;
        attacking = false;
    }
}
