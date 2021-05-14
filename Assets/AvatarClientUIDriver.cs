using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class AvatarClientUIDriver : NetworkBehaviour
{
    public Slider cityCaptureSlider;
    public TextMeshProUGUI cityNameTMP;

    [SyncVar (hook = nameof(UpdateUI))]
    public float time;
    [SyncVar]
    public float maxTime; 

    public override void OnStartClient()
    {
        base.OnStartClient();
        ClientInstance ci = ClientInstance.ReturnClientInstance();
        UIManager uim = FindObjectOfType<UIManager>();
        cityCaptureSlider = uim.GetCityCaptureSlider(ci);
        cityNameTMP = uim.GetCityNameTMP(ci);  //TODO implement city names.
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

}
