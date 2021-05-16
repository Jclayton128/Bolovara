using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System;

public class AvatarClientUIDriver : NetworkBehaviour
{
    Slider cityCaptureSlider;
    TextMeshProUGUI cityNameTMP;
     Image compassBG;
    Image compassNeedle;
    CityManager cm;
    

    //param
    float maxCityDistance = 30f;
    Color iffAllied = Color.green;
    Color iffEnemy = Color.red;
    Color iffFeral = Color.blue;
    Color iffCapturing = Color.yellow;


    //hood
    [SyncVar (hook = nameof(UpdateUI))]
    float time;
    [SyncVar]
    float maxTime;

    float compassDeg;
    string closestCity;
    Color compassBGColor;

    CitySquare prevCS;
    CitySquare nearestCS;
    float distToNearestCS;
    float initialScale;

    int myIFFAllegiance; // OK to capture this because it won't change for the player.

    public override void OnStartClient()
    {
        base.OnStartClient();
        ClientInstance ci = ClientInstance.ReturnClientInstance();
        UIManager uim = FindObjectOfType<UIManager>();
        cityCaptureSlider = uim.GetCityCaptureSlider(ci);
        cityNameTMP = uim.GetCityNameTMP(ci);  //TODO implement city names.
        uim.GetCompassComponents(ci, out compassBG, out compassNeedle);
        cm = FindObjectOfType<CityManager>();
        initialScale = compassNeedle.transform.localScale.x;
        myIFFAllegiance = GetComponent<IFF>().GetIFFAllegiance();
    }

    public void UpdateTimes(float newTime, float newMaxTime)
    {
        time = newTime;
        maxTime = newMaxTime;
    }

    private void UpdateUI(float placeholder1, float placeholder2)
    {
        cityCaptureSlider.maxValue = maxTime;
        cityCaptureSlider.value = time;
    }

    private void Update()
    {
        if (isClient)
        {
            prevCS = nearestCS;
            nearestCS = cm.FindNearestCitySquare(transform);

            OrientCompassTowardsNearestCity();
            ScaleCompassWithDistanceToNearestCity();
            ChangeCompassBGWithIFFStatusOfNearestCity();

            if (nearestCS == prevCS)
            {
                return;
            }
            UpdateNameBarWithClosestCity();
        }
    }

    private void OrientCompassTowardsNearestCity()
    {
        float ang = cm.FindAngleToCitySquare(transform, nearestCS);
        Quaternion rot = Quaternion.Euler(0, 0, ang);
        compassNeedle.transform.rotation = rot;
    }

    private void ScaleCompassWithDistanceToNearestCity()
    {
        float dist = (nearestCS.transform.position - transform.position).magnitude;
        float factor = 1 - (dist / maxCityDistance);
        factor = Mathf.Clamp(factor, .3f, 1.0f);
        compassNeedle.transform.localScale = Vector3.one * factor * initialScale;
    }

    private void ChangeCompassBGWithIFFStatusOfNearestCity()
    {
        int nearestIFF = cm.FindCityIFF(nearestCS);
        if (nearestCS.GetTimeSpentCapturing() > 0.1f)
        {
            compassBG.color = iffCapturing;
            return;
        }
        if (nearestIFF == IFF.feralIFF)
        {
            compassBG.color = iffFeral;
            return;
        }
        if (nearestIFF == myIFFAllegiance)
        {
            compassBG.color = iffAllied;
            return;
        }
        if (nearestIFF != myIFFAllegiance)
        {
            compassBG.color = iffEnemy;
            return;
        }
    }


    private void UpdateNameBarWithClosestCity()
    {
        cityNameTMP.text = nearestCS.GetCityName();
    }

}
