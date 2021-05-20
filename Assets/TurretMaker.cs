using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class TurretMaker : NetworkBehaviour
{
    UIManager uim;
    TurretPanelDriver tpd;
    [SerializeField] GameObject[] turretOptionPrefabs = null;
    [SerializeField] GameObject dummyPrefab = null;
    GameObject dummySelected;
    GameObject nearestHouse;
    SpriteRenderer dummySR;
    [SerializeField] int[] turretOptionCosts = null;
    int myIFF;
    MoneyHolder mh;
    CaptureTool avatarCT;

    //param
    float snapRange = 1.0f;
    Color invalidColor = new Color(1, 0.3f, 0.3f, 0.5f);
    Color validColor = new Color(1, 1, 1, 0.5f);


    //hood
    Vector3 mousePos = Vector3.zero;
    int currentSelectionIndex = 0;
    void Start()
    {

        if (hasAuthority)
        {
            uim = FindObjectOfType<UIManager>();
            ClientInstance ci = ClientInstance.ReturnClientInstance();
            tpd = uim.GetTPD(ci);
            tpd.SetTurretMaker(this);
            SetOptionSpritesOnTPD();
            SetOptionCostsOnTPD();

        }
        myIFF = GetComponent<FactionLeader>().GetMasterIFFAllegiance();
        mh = GetComponent<MoneyHolder>();
    }

    private void SetOptionSpritesOnTPD()
    {
        List<Sprite> spriteSource = new List<Sprite>();

        foreach (GameObject go in turretOptionPrefabs)
        {
            Sprite newSprite = go.GetComponent<SpriteRenderer>().sprite;
            spriteSource.Add(newSprite);
        }

        tpd.SetOptionImages(spriteSource);
    }
    private void SetOptionCostsOnTPD()
    {
        tpd.SetOptionCosts(turretOptionCosts);
    }

    public void PrepareOption(int i)
    {
        ClearCurrentSelection();
        currentSelectionIndex = i;

        if (mh.CheckForSufficientFunds(turretOptionCosts[i]) == false)
        {
            Debug.Log("You can't afford this!");
            //TODO play a negative sound.
        }
        else
        {

            GameObject dummy = Instantiate(dummyPrefab, mousePos, Quaternion.identity) as GameObject;
            Sprite dummySprite = turretOptionPrefabs[i].GetComponent<SpriteRenderer>().sprite;
            dummySelected = dummy;
            dummySR = dummy.GetComponent<SpriteRenderer>();
            dummySR.sprite = dummySprite;
            dummySR.color = new Color(1, 0.3f, 0.3f, 0.5f);
        }
    }

    public void ClearCurrentSelection()
    {
        if (dummySelected)
        {
            dummySR = null;
            Destroy(dummySelected);
            dummySelected = null;
        }
        currentSelectionIndex = 0;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        UpdateTurretSelectionBasedOnMouse();
    }

    public void HandleClick()
    {

        if (nearestHouse)
        {
            CmdRequestTurretUpgrade(currentSelectionIndex, nearestHouse.transform.position, myIFF, nearestHouse);
        }

    }

    private void UpdateTurretSelectionBasedOnMouse()
    {
        if (hasAuthority && dummySelected)
        {

            nearestHouse = CheckForNearbyAlliedHouse(mousePos);

            if (nearestHouse)
            {
                dummySelected.transform.position = nearestHouse.transform.position;
                dummySR.color = validColor;
            }
            else
            {
                dummySelected.transform.position = mousePos;
                dummySR.color = invalidColor;
            }
        }
    }

    [Command]
    private void CmdRequestTurretUpgrade(int index, Vector3 position, int iff, GameObject sacrificialHouse)
    {
        int cost = turretOptionCosts[index];
        if (mh.CheckForSufficientFunds(cost) == false) { return; }  //Server check for sufficient funds
        if (sacrificialHouse.GetComponent<IFF>().GetIFFAllegiance() != iff) { return; } // Server check that the house is allied
        mh.ModifyMoney(-1 * turretOptionCosts[currentSelectionIndex]);

        GameObject newTurret = Instantiate(turretOptionPrefabs[index], position, Quaternion.identity) as GameObject;
        newTurret.GetComponent<IFF>().SetIFFAllegiance(iff);

        ClearCurrentSelection();
        RpcUpgradeHouseIntoTurret(index, position, iff);

        sacrificialHouse.GetComponent<Building>().FindCurrentOwner();
        sacrificialHouse.GetComponent<Building>().owner.GetComponent<HouseHolder>().DecrementHouseCount();

        //nearestHouse.GetComponent<Building>().DyingActions();  //The server doesn't have a working allegiance manager, I guess? This line makes the server sad.
        NetworkServer.Destroy(sacrificialHouse);
        nearestHouse = null;
        ClearCurrentSelection();
    }

    [ClientRpc]
    public void RpcUpgradeHouseIntoTurret(int turretType, Vector3 pos, int newIFF)
    {
        GameObject newTurret = Instantiate(turretOptionPrefabs[turretType], pos, Quaternion.identity) as GameObject;
        newTurret.GetComponent<IFF>().SetIFFAllegiance(newIFF);
    }


    private GameObject CheckForNearbyAlliedHouse(Vector3 mousePos)
    {
        GameObject nearHouse = Finder.FindNearestGameObjectWithTag(mousePos, "House", snapRange);
        if (!nearHouse) { return null; }
        int houseIFF = nearHouse.GetComponent<IFF>().GetIFFAllegiance();


        if (nearHouse && houseIFF == myIFF)
        {
            return nearHouse;
        }
        else
        {
            return null;
        }
    }


}
