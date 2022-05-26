using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpecialHex : MonoBehaviour
{
    public enum Type { MaxHealth, Damage, SpinCost, Help, Reset, Info}
   
    public GameObject costText;
    public Type type;
    public int[] prices;

    private bool canPurchase;
    private bool isUpgrade;

    private void Start()
    {
        if (type != Type.Help && type != Type.Reset && type != Type.Info)
        {
            DisplayText();
            isUpgrade = true;
        }

        else
            isUpgrade = false;
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

        if(!isUpgrade)
        {
            if (type == Type.Help)
                GameManager.instance.panels.Tutorial();

            if (type == Type.Reset)
            {
                GameManager.instance.panels.ResetConfirm();
                GameManager.instance.canReset = true;
            }

            if (type == Type.Info)
                GameManager.instance.panels.Info();

            return;
        }
         
        int upgradeCount = UpgradeCount();
        Color c;

        if(upgradeCount == 3)
        {
            c = Color.grey;
            GameManager.instance.panels.Shop(soldOut: true);
        }

        else if (prices[upgradeCount] <= GameManager.instance.currency)
        {
            canPurchase = true;
            c = Color.green;
            GameManager.instance.panels.Shop((int)type);
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
        GameManager.instance.canReset = false;

        if(isUpgrade)
        {
            canPurchase = false;
            for (int i = 0; i < 6; i++)
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }        
    }

    private int UpgradeCount()
    {
        switch (type)
        {
            case Type.MaxHealth:
                return GameManager.instance.healthUpgrades;

            case Type.Damage:
                return GameManager.instance.damageUpgrades;

            case Type.SpinCost:
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

        switch (type)
        {
            case Type.MaxHealth:
                GameManager.instance.healthUpgrades++;
                GameManager.instance.playerScript.AddHealth(2);
                break;

            case Type.Damage:
                GameManager.instance.damageUpgrades++;
                GameManager.instance.baseDamage = GameManager.instance.damagePermValues[GameManager.instance.damageUpgrades];
                break;

            case Type.SpinCost:
                GameManager.instance.silkUpgrades++;
                GameManager.instance.spinCost = GameManager.instance.silkPermValues[GameManager.instance.silkUpgrades];
                break;
        }

        DisplayText();
        GameManager.instance.panels.Shop(bought: true);
        FindObjectOfType<AudioManager>().Play("Buy");

        for (int i = 0; i < 6; i++)
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
    }
}
