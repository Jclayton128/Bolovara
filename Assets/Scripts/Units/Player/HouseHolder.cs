using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class HouseHolder : NetworkBehaviour
{
    //init
    TextMeshProUGUI houseCounter;


    //param

    [SyncVar(hook = nameof(UpdateUI))]
    [SerializeField] int numberOfHouses = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (isClient && hasAuthority)
        {
            ClientInstance ci = ClientInstance.ReturnClientInstance();
            houseCounter = FindObjectOfType<UIManager>().GetHouseCounter(ci);
        }
    }
     
    public void DecrementHouseCount()
    {
        if (isServer)
        {
            numberOfHouses--;
        }
    }

    public void IncrementHouseCount()
    {
        if (isServer)
        {
            numberOfHouses++;
        }
    }

    private void UpdateUI(int placeholder1, int placeholder2)
    {
        if (!houseCounter) { return; }
        houseCounter.text = numberOfHouses.ToString();

    }


}
