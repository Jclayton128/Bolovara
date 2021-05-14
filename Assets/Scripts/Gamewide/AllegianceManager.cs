using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AllegianceManager : MonoBehaviour
{
    //Init
    SceneLoader sl;

    [SerializeField] Sprite[] flagSource = null;
    SortedList<int, FactionLeader> factionLeaders = new SortedList<int, FactionLeader>();
    [SerializeField] int factionsInPlay = 0;

    //hood
    int playerAllegiance; // remove shim


    private void Awake()
    {
        int amCount = FindObjectsOfType<AllegianceManager>().Length;
        if (amCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {

        sl = FindObjectOfType<SceneLoader>();
    }

    public int GetNumberOfFactionsIncludingFeral()
    {
        return flagSource.Length;
    }
    public int GetPlayerIFF()
    {
        return playerAllegiance;
    }

    public void SetPlayerIFF(int newIff)
    {
        playerAllegiance = newIff;
    }
    public void AddFactionLeaderToList(int allegiance, FactionLeader fl) //call this when the Arena scene is loaded.
    {
        if (factionLeaders.ContainsKey(allegiance))
        {
            Debug.Log("faction " + allegiance + " already has a leader!");
        }
        else
        {
            Debug.Log($"added {fl.transform.gameObject} for faction " + allegiance + ".");
            factionLeaders.Add(allegiance, fl);
            factionsInPlay++;
        }
    }

    public void RemoveFactionLeaderFromList(int flAllegiance)
    {
        factionLeaders.Remove(flAllegiance);
        factionsInPlay--;
    }

    public void ReplaceFactionLeaderOnList(int allegiance, FactionLeader newFL)
    {
        if (factionLeaders.ContainsKey(allegiance))
        {
            factionLeaders.Remove(allegiance);
            AddFactionLeaderToList(allegiance, newFL);
        }
    }

    public FactionLeader GetFactionLeader(int iffAllegiance)
    {
        //Debug.Log($"I was asked for the faction leader for {iffAllegiance}");
        if (!factionLeaders.ContainsKey(iffAllegiance) || factionLeaders[iffAllegiance] == null)
        {
            Debug.Log($"A faction leader doesn't exist for {iffAllegiance}. Returning to Feral");
            return factionLeaders[IFF.feralIFF];
        }
        else
        {
            FactionLeader fl = factionLeaders[iffAllegiance];
            return fl;
        }

    }

    public Sprite GetFlagOfAllegiance(int key)
    {
        return flagSource[key];
    }



}
