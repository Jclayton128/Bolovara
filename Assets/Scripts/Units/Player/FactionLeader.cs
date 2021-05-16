using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//[RequireComponent(typeof(MoneyHolder), typeof(IFF), typeof(HouseHolder))]

public class FactionLeader : NetworkBehaviour
{
    //init
    AllegianceManager am;
    [SyncVar]
    int masterIFFAllegiance;


    void Start()
    {
        am = FindObjectOfType<AllegianceManager>();
        am.AddFactionLeaderToList(masterIFFAllegiance, this);
    }

    public void SetMasterIFFAllegiance(int alleg)
    {
        masterIFFAllegiance = alleg;
    }

    public int GetMasterIFFAllegiance()
    {
        return masterIFFAllegiance;
    }

}
