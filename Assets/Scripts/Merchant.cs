using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.merchant = this;
    }

    public void Animate()
    {
        Debug.Log("Thanks for your money!");
    }
}
