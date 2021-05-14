using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IFF : MonoBehaviour
{
    //init
    [SerializeField] SpriteRenderer flagSR = null;
    AllegianceManager am;

    //param
    [SerializeField] int iffAllegiance;
    public static readonly int feralIFF = 0;

    private void Start()
    {
        am = FindObjectOfType<AllegianceManager>();
        SetFlag();
    }


    public void SetIFFAllegiance(int value)
    {
        iffAllegiance = value;
        SetFlag();
    }

    public int GetIFFAllegiance()
    {
        return iffAllegiance;
    }
    private void SetFlag()
    {
        if (!flagSR) { return; }
        Debug.Log(gameObject + "is still the prob because of " + am);
        flagSR.sprite = am.GetFlagOfAllegiance(iffAllegiance);
    }

    public int GetFeralIFF()
    {
        return feralIFF;
    }
}
