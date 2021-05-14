using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[RequireComponent(typeof(MoneyHolder), typeof(IFF), typeof(HouseHolder))]

public class FactionLeader : MonoBehaviour
{
    //init
    AllegianceManager am;
    [SerializeField] int masterIFFAllegiance = 5;


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
