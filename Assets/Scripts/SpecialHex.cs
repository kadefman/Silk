﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpecialHex : MonoBehaviour
{
    public enum Upgrade { MaxHealth, Damage, SpinCost, Help, Reset, Info}
   
    public GameObject itemObject;
    public GameObject costText;
    public Upgrade upgradeType;
    public int[] prices;

    private bool canPurchase;

    private void Start()
    {
        if(upgradeType != Upgrade.Help && upgradeType != Upgrade.Reset && upgradeType != Upgrade.Info)
        {
            //Debug.Log((int)upgradeType);
            DisplayText();
        }
            
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canPurchase)
            GiveUpgrade();
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.transform.CompareTag("Player"))
            return;

        if (upgradeType == Upgrade.Help)
        {
            GameManager.instance.panels.Tutorial();
            return;
        }

        if (upgradeType == Upgrade.Reset)
        {
            GameManager.instance.panels.ResetConfirm();
            GameManager.instance.canReset = true;
            return;
        }

        if(upgradeType == Upgrade.Info)
        {
            GameManager.instance.panels.Info();
            return;
        }

        if (!collision.transform.CompareTag("Player"))
            return;

        int upgradeCount = UpgradeCount();
        Color c;

        if(upgradeCount == 3)
        {
            c = Color.grey;
            GameManager.instance.panels.Shop(soldOut: true);
        }

        if (upgradeCount < 3 && prices[upgradeCount] <= GameManager.instance.currency)
        {
            canPurchase = true;
            c = Color.green;
            GameManager.instance.panels.Shop((int)upgradeType);
        }

        else
        {
            c = Color.grey;
            GameManager.instance.panels.Shop(noFunds: true);
        }
            

        //sensors
        for (int i = 0; i < 6; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {       
        if (!collision.transform.CompareTag("Player"))
            return;

        GameManager.instance.panels.HidePanels();

        if (upgradeType == Upgrade.Reset || upgradeType == Upgrade.Help || upgradeType == Upgrade.Info)
        {
            GameManager.instance.canReset = false;
            return;
        }

        //exiting shop tile
        canPurchase = false;
        for (int i = 0; i < 6; i++)
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;       
    }

    private int UpgradeCount()
    {
        switch (upgradeType)
        {
            case Upgrade.MaxHealth:
                return GameManager.instance.healthUpgrades;

            case Upgrade.Damage:
                return GameManager.instance.damageUpgrades;

            case Upgrade.SpinCost:
                return GameManager.instance.silkUpgrades;

            default:
                return 0;
        }
    }

    private void DisplayText()
    {
        int upgradeCount = UpgradeCount();

        if (upgradeCount < 3)
        {
            costText.gameObject.SetActive(true);
            costText.transform.GetComponent<TextMeshPro>().text = prices[upgradeCount].ToString();
        }

        else
            costText.transform.GetComponent<TextMeshPro>().text = "SOLD OUT";
    }

    private void GiveUpgrade()
    {
        int upgradeCount = UpgradeCount();
        GameManager.instance.AddCurrency(-prices[upgradeCount]);

        switch (upgradeType)
        {
            case Upgrade.MaxHealth:
                GameManager.instance.healthUpgrades++;
                GameManager.instance.playerScript.AddHealth(2);
                break;

            case Upgrade.Damage:
                GameManager.instance.damageUpgrades++;
                GameManager.instance.baseDamage = GameManager.instance.damagePermValues[GameManager.instance.damageUpgrades];
                break;

            case Upgrade.SpinCost:
                GameManager.instance.silkUpgrades++;
                GameManager.instance.spinCost = GameManager.instance.silkPermValues[GameManager.instance.silkUpgrades];
                break;
        }

        DisplayText();
        GameManager.instance.panels.Shop(bought: true);
        //purchase audio
        FindObjectOfType<AudioManager>().Play("Buy");

        for (int i = 0; i < 6; i++)
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
    }
}
