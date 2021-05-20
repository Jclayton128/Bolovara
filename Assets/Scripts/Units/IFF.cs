using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IFF : NetworkBehaviour
{
    //init
    [SerializeField] SpriteRenderer flagSR = null;
    AllegianceManager am;

    //param
    [SyncVar (hook = nameof(SetFlag))]
    [SerializeField] int iffAllegiance;
    public static readonly int feralIFF = 0;

    public Action<int> OnChangeIFF;
    private void Start()
    {
        am = FindObjectOfType<AllegianceManager>();
        SetFlag(0,0);
    }


    public void SetIFFAllegiance(int value)
    {
        iffAllegiance = value;
        SetFlag(0,0);
        OnChangeIFF?.Invoke(value);
    }

    public int GetIFFAllegiance()
    {
        return iffAllegiance;
    }
    private void SetFlag(int placeholder1, int placeholder2)
    {
        if (!flagSR) { return; }
        if (!am)
        {
            am = FindObjectOfType<AllegianceManager>();
        }
        flagSR.sprite = am.GetFlagOfAllegiance(iffAllegiance);
    }

    public int GetFeralIFF()
    {
        return feralIFF;
    }
}
