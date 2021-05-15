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
    int money = 0;
    void Start()
    {
        ClientInstance ci = ClientInstance.ReturnClientInstance();
        moneyBar = FindObjectOfType<UIManager>().GetMoneyCounter(ci);

    }

    public int GetMoneyAmount()
    {
        return money;
    }

    public void AddMoney(int amount)
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
}
