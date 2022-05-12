using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeHex : MonoBehaviour
{
    public enum Upgrade { MaxHealth, Damage, SpinCost }
   
    public GameObject itemObject;
    public GameObject costText;
    public Upgrade upgradeType;
    public int[] prices;

    private bool canPurchase;

    private void Start()
    {
        DisplayText();
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

        int upgradeCount = UpgradeCount();

        if (upgradeCount < 3 && prices[upgradeCount] <= GameManager.instance.currency)
        {
            canPurchase = true;
            Debug.Log("Can Purchase");
        }
            

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            canPurchase = false;
            Debug.Log("Can't purchase");
        }
            
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
        GameManager.instance.merchant.Animate();
        transform.GetComponent<Collider2D>().enabled = false;
        transform.GetComponent<Collider2D>().enabled = true;
    }
}
