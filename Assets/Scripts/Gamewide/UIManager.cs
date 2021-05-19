using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class UIManager : NetworkBehaviour
{
    //init

    public ClientInstance playerAtThisComputer;
    public NetworkConnection playerConnection;
    [SerializeField] Slider healthBar = null;
    [SerializeField] GameObject shiftKnob = null;
    [SerializeField] Transform shiftPos_1 = null;
    [SerializeField] Transform shiftPos_2 = null;
    [SerializeField] Transform shiftPos_3 = null;
    [SerializeField] TextMeshProUGUI houseCounter = null;
    [SerializeField] TextMeshProUGUI moneyCounter = null;
    [SerializeField] Image flag = null;
    [SerializeField] Slider energyBar = null;
    [SerializeField] Image weaponIcon = null;
    [SerializeField] Slider cityCaptureSlider = null;
    [SerializeField] TextMeshProUGUI cityNameTMP = null;
    [SerializeField] RadarScreen radarScreen = null;
    [SerializeField] TextMeshProUGUI followMeText = null;
    [SerializeField] Image compassBackground = null;
    [SerializeField] Image compassNeedle = null;
    [SerializeField] TurretPanelDriver turretPanel = null;



    // Update is called once per frame
    void Update()
    {

    }

    public void SetLocalPlayerForUI(ClientInstance ci)
    {
        playerAtThisComputer = ci;
    }

    public ClientInstance GetLocalPlayerForChecking()
    {
        return playerAtThisComputer;
    }

    public Slider GetHealthBar(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return healthBar;
        }
        else
        {
            Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }
    public GameObject GetShiftKnob(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return shiftKnob;
        }
        else
        {
            Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }
    public void GetShiftPositions(ClientInstance askingCI, out Transform shift1, out Transform shift2, out Transform shift3)
    {
        if (askingCI == playerAtThisComputer)
        {
            shift1 = shiftPos_1;
            shift2 = shiftPos_2;
            shift3 = shiftPos_3;
        }
        else
        {
            Debug.Log("Asking GO is not the local player! No UI for you!");
            shift1 = null;
            shift2 = null;
            shift3 = null;
        }
    }

    public TextMeshProUGUI GetHouseCounter(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return houseCounter;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }

    public TextMeshProUGUI GetMoneyCounter(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return moneyCounter;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }

    public Image GetFlagUIElement(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return flag;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }

    public Slider GetEnergyBar(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return energyBar;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }
    public Image GetWeaponIcon(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return weaponIcon;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }

    public Slider GetCityCaptureSlider(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return cityCaptureSlider;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }

    public TextMeshProUGUI GetCityNameTMP(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return cityNameTMP;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }
    public RadarScreen GetRadarScreen(ClientInstance askingCI)
    {
        if (askingCI == playerAtThisComputer)
        {
            return radarScreen;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }

    public TextMeshProUGUI GetFollowMeText(ClientInstance askingCI)
    {

        if (askingCI == playerAtThisComputer)
        {
            return followMeText;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            return null;
        }
    }

    public void GetCompassComponents(ClientInstance askingCI, out Image compBG, out Image compNeedle)
    {

        if (askingCI == playerAtThisComputer)
        {
            compBG = compassBackground;
            compNeedle = compassNeedle;
        }
        else
        {
            //Debug.Log("Asking GO is not the local player! No UI for you!");
            compBG = null;
            compNeedle = null;
        }
    }

    public TurretPanelDriver GetTPD(ClientInstance askingCI)
    {

        if (askingCI == playerAtThisComputer)
        {
            return turretPanel;
        }
        else
        {
            return null;
        }
    }

}
