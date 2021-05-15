using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class CaptureTool : MonoBehaviour
{
    //init
    [SerializeField] ControlSource cs = null;
    [SerializeField] IFF iff = null;
    AvatarClientUIDriver avcuid;

    public CitySquare cityToCapture;

    //param


    //hood
    float timeSpentCapturing;

    void Start()
    {
        avcuid = GetComponentInParent<AvatarClientUIDriver>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cs.IsRunningOnServer)
        {
            int collisionIFF = collision.GetComponent<IFF>().GetIFFAllegiance();
            int ownIFF = iff.GetIFFAllegiance();
            if ( collisionIFF == ownIFF ) { return; }
            cityToCapture = collision.GetComponent<CitySquare>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (cs.IsRunningOnServer)
        {
            if (!cityToCapture) { return; }
            cityToCapture.BuildCaptureTime(Time.deltaTime);
            PushUpdateToUIDriver(cityToCapture);

            if (cityToCapture.GetTimeSpentCapturing() >= cityToCapture.GetTimeRequiredToCapture())
            {
                int newAllegiance = iff.GetIFFAllegiance();
                cityToCapture.GetComponent<IFF>().SetIFFAllegiance(newAllegiance);
                cityToCapture.SetAllegianceForBuildingsInCity(newAllegiance);
                cityToCapture.ResetCaptureStatus();
                cityToCapture = null;
                avcuid.UpdateTimes(0, 0);
            }
        }

    }

    private void PushUpdateToUIDriver(bool isCapturingACity)
    {
        if (!avcuid) { return; }
        if (isCapturingACity)
        {
            avcuid.UpdateTimes(cityToCapture.GetTimeSpentCapturing(), cityToCapture.GetTimeRequiredToCapture());

        }
        if (!isCapturingACity)
        {
            avcuid.UpdateTimes(0, 0);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!cityToCapture) { return; }
        cityToCapture = null;
        PushUpdateToUIDriver(cityToCapture);
    }



}
