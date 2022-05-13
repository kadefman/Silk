using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelHolder : MonoBehaviour
{
    public void Start()
    {
        GameManager.instance.panels = this;
    }

    public void Tutorial()
    {
        ShowPanel(0);
    }

    public void Info()
    {
        ShowPanel(1);
    }

    public void Shop(int upgrade, bool bought, bool soldOut, bool noFunds)
    {
        ShowPanel(2);
        TextMeshPro shopText = transform.GetChild(2).GetComponent<TextMeshPro>();
        if (bought)
            shopText.text = "Thank you!";

        else if (soldOut)
            shopText.text = "You are too powerful! You cannot buy anymore.";

        else if (noFunds)
            shopText.text = "You can't afford that yet. Sorry!";

        else
        {
            string upgradeName;
            if (upgrade == 0)
                upgradeName = "health";
            else
                upgradeName = upgrade == 1 ? "damage" : "silk";

            shopText.text = $"Upgrade {upgradeName}? \n Press space to confirm...";
        }           
    }

    public void ResetConfirm()
    {
        ShowPanel(3);
    }

    public void PostReset()
    {
        ShowPanel(4);
    }

    public void Win()
    {
        ShowPanel(5);
        TextMeshPro runText = transform.GetChild(5).GetChild(2).GetComponent<TextMeshPro>();
        runText.text = $"Congratulations! \n You completed the game in {GameManager.instance.runCount} runs. \n Press 'R' to start over...";
    }

    private void ShowPanel(int index)
    {
        transform.GetChild(index).gameObject.SetActive(true);
    }

    public void HidePanels()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
