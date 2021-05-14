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
        Debug.Log("trigger enter 0");
        if (cs.IsRunningOnServer)
        {
            Debug.Log("trigger enter 1");
            int collisionIFF = collision.GetComponent<IFF>().GetIFFAllegiance();
            Debug.Log("trigger enter 2");
            int ownIFF = iff.GetIFFAllegiance();
            Debug.Log("trigger enter 3");
            if ( collisionIFF == ownIFF ) { return; }
            Debug.Log("trigger enter 4");
            cityToCapture = collision.GetComponent<CitySquare>();
            Debug.Log("trigger enter 5");
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
