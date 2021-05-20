using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer), typeof(IFF))]
public class Building : NetworkBehaviour
{
    //init
    public AllegianceManager am;
    SpriteRenderer sr;
    [SerializeField] Sprite[] possibleHouseSprites = null;
    CitySquare cs;
    public IFF iff;


    //param
    public bool isHouse = true;
    float timeBetweenMoneyDrops = 5f;
    int amountOfMoneyOnEachDrop = 1;

    //hood
    [SerializeField] GameObject owner;
    float timeSinceLastMoneyDrop;


    public override void OnStartServer()
    {
        InitializeBuilding();
    }
    public void InitializeBuilding()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isHouse)
        {
            ChooseHouseImage();
        }
       

        //UpdateCurrentOwner();
        timeSinceLastMoneyDrop = UnityEngine.Random.Range(0, timeBetweenMoneyDrops);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        sr = GetComponent<SpriteRenderer>();
        if (isHouse)
        {
            ChooseHouseImage();
        }
    }

    private void ChooseHouseImage()
    {
        int rand = UnityEngine.Random.Range(0, possibleHouseSprites.Length);
        sr.sprite = possibleHouseSprites[rand];
    }

    void Update()
    {
        if (isServer)
        {
            GenerateMoneyForOwner();
        }
    }

    public CitySquare GetOwningCitySquare()
    {
        return cs;
    }

    private void GenerateMoneyForOwner()
    {
        timeSinceLastMoneyDrop -= Time.deltaTime;
        if (timeSinceLastMoneyDrop <= 0)
        {
            FindCurrentOwner();
            owner.GetComponent<MoneyHolder>().ModifyMoney(amountOfMoneyOnEachDrop);
            timeSinceLastMoneyDrop = timeBetweenMoneyDrops;
        }
    }

    public void FindCurrentOwner()
    {
        owner = am.GetFactionLeader(iff.GetIFFAllegiance()).gameObject;
    }
    public void SetOwningCity(CitySquare citysq)
    {
        cs = citysq;
    }
    public void SetHouseIFFAllegiance(int newIFF)
    {
        iff.SetIFFAllegiance(newIFF);

        if (!GetComponent<DefenseTurret>())
        {
            if (owner) //don't decrement if there isn't a previous owner to decrement from
            {
                owner.GetComponent<HouseHolder>().DecrementHouseCount();  //owner reference should still be the old owner
            }
            owner = am.GetFactionLeader(newIFF).gameObject; //now owner reference becomes the new owner.
            owner.GetComponent<HouseHolder>().IncrementHouseCount();
        }
    }

    public void DyingActions()
    {
        if (isClient) { return; }
        if (!cs) { return; }

        if (!GetComponent<DefenseTurret>())
        {
            FindCurrentOwner();
            if (owner)
            {
                owner.GetComponent<HouseHolder>().DecrementHouseCount();
            }
        }
        cs.RemoveBuildingFromLists(this);
    }

  }
