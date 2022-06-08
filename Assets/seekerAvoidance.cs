using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seekerAvoidance : MonoBehaviour
{
    GameObject[] seeker;
    public float spaceBetween = .3f;
    public float speed = .2f;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GameObject.FindGameObjectsWithTag("Seeker");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (GameObject go in seeker)
        {
            if (go != gameObject)
            {
                float distance = Vector3.Distance(go.transform.position, this.transform.position);
                if (distance <= spaceBetween)
                {
                    Vector3 direction = transform.position - go.transform.position;
                    if (direction.magnitude >= .5f)
                        transform.Translate(2 * direction * Time.deltaTime);
                    else
                        transform.Translate(direction * speed * Time.deltaTime);
                }
              
            }
        }
    }
}
