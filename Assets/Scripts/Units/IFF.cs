﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IFF : MonoBehaviour
{
    //init
    [SerializeField] SpriteRenderer flagSR = null;
    AllegianceManager am;
    Image flagImage;

    //param
    [SerializeField] int iffAllegiance;
    public static readonly int feralIFF = 0;

    private void Start()
    {
        am = FindObjectOfType<AllegianceManager>();
        GetFlagUIElement();
        SetFlag();
    }

    private void GetFlagUIElement()
    {
        if (transform.root.tag == "Player")
        {
            //flagImage = FindObjectOfType<UIManager>().GetFlag(transform.root.gameObject);
            iffAllegiance = am.GetPlayerIFF();
            if (flagImage)
            {
                flagImage.sprite = am.GetFlagOfAllegiance(iffAllegiance);
            }
        }
    }

    public void SetIFFAllegiance(int value)
    {
        iffAllegiance = value;
        SetFlag();
        //Debug.Log(gameObject.name + " is now aligned with: " + iffAllegiance);
    }

    public int GetIFFAllegiance()
    {
        return iffAllegiance;
    }
    private void SetFlag()
    {
        if (!flagSR) { GetFlagUIElement(); }
        if (!flagSR) { return; }
        flagSR.sprite = am.GetFlagOfAllegiance(iffAllegiance);
    }

    public int GetFeralIFF()
    {
        return feralIFF;
    }
}
