using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;

public class MoneyHolder : NetworkBehaviour
{
    //init
    public TextMeshProUGUI moneyBar;


    //param

    //hood
    [SyncVar(hook = nameof(UpdateUI))]
    int money = 5;
    void Start()
    {
        if (isClient && hasAuthority)
        {
            ClientInstance ci = ClientInstance.ReturnClientInstance();
            moneyBar = FindObjectOfType<UIManager>().GetMoneyCounter(ci);
        }
    }

    public int GetMoneyAmount()
    {
        return money;
    }

    public void ModifyMoney(int amount)
    {
        if (isServer)
        {
            money += amount;
        }
    }

    private void UpdateUI(int placeholder1, int placeholder2)
    {
        if (!moneyBar) { return; }
        moneyBar.text = "$ " + money.ToString();
    }

    public bool CheckForSufficientFunds(int amount)
    {
        if (amount > money)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
