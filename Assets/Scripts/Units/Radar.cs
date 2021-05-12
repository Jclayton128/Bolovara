using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : NetworkBehaviour
{
    //init
    [SerializeField] RadarScreen rs;
    [SerializeField] UnitTracker ut;
    [SerializeField] ClientInstance playerAtThisComputer;
    [SerializeField] UIManager uim;
    SyncDictionary<int, float> sectorIntensities = new SyncDictionary<int, float>();
    [SerializeField] List<GameObject> targets = new List<GameObject>(); 

    //param
    [SerializeField] float timeBetweenScans;  //0.3f
    [SerializeField] float radarAccuracy; //15  //how far off can the direction-of-arrival be, in degrees.
    [SerializeField] float radarRange;  //30
    [SerializeField] float signalFudge;  //0.05

    //hood
    float timeSinceLastScan = 0;

    #region initial setup
    public override void OnStartClient()
    { 
        if (hasAuthority)
        {
            playerAtThisComputer = ClientInstance.ReturnClientInstance();
            uim = FindObjectOfType<UIManager>();
            rs = uim.GetRadarScreen(playerAtThisComputer);
        }
    }   

    public override void OnStartServer()
    {
        base.OnStartServer();
        ut = FindObjectOfType<UnitTracker>();
        PopulateSectorIntensitieswithZero();
    }
    private void PopulateSectorIntensitieswithZero()
    {
        for (int i = 0; i < 8; i++)
        {
            sectorIntensities.Add(i, 0);
        }
    }
    #endregion

    void Update()
    {
        if (isServer)
        {
            timeSinceLastScan += Time.deltaTime;
            if (timeSinceLastScan >= timeBetweenScans)
            {
                ResetSectorIntensityToZero();
                GetTargets();
                IncreaseIntensityFromNoiseInEachSector();
                ClampIntensityLevelFloorToSelfNoiseInEachSector();
                TargetPushSectorIntensityToRadarScreen();
                timeSinceLastScan = 0;
            }
        }

    }

    private void ResetSectorIntensityToZero()
    {
        for (int i = 0; i < 8; i++)
        {
            sectorIntensities[i] = 0;
        }
    }

    [Server]
    private void GetTargets()
    {
        targets = ut.FindTargetsWithinSearchRange(transform.gameObject, radarRange);  //TODO look for defense turrets too
    }
    private void IncreaseIntensityFromNoiseInEachSector()
    {
        foreach (GameObject target in targets)
        {
            int sector = DetermineSector(target);
            //Debug.Log("angleFromNorth: " + signedAngFromNorth + " goes into approxSector: " + approxSector + " rounds to: " + sector);

            float signalIntensity = DetermineSignalIntensity(target);

            //TODO: Get the current target's noise and add it instead of the arbitrary value;
            sectorIntensities[sector] = sectorIntensities[sector] + signalIntensity;
        }

    }

    private int DetermineSector(GameObject target)
    {
        Vector3 dir = target.transform.position - transform.position;
        float signedAngFromNorth = Vector3.SignedAngle(dir, Vector3.up, Vector3.forward) - 22.5f;
        if (signedAngFromNorth < 0)
        {
            signedAngFromNorth += 360;
        }

        signedAngFromNorth = InjectRandomSignalSpread(signedAngFromNorth);

        float approxSector = (signedAngFromNorth / 45);
        int sector = Mathf.RoundToInt(approxSector);

        if (sector >= 8)
        {
            sector = 0;
        }
        if (sector < 0)
        {
            sector = 7;
        }

        return sector;
    }

    private float InjectRandomSignalSpread(float signedAngFromNorth)
    {
        float randomSpread = UnityEngine.Random.Range(-radarAccuracy, radarAccuracy);
        signedAngFromNorth += randomSpread;
        return signedAngFromNorth;
    }

    private float DetermineSignalIntensity(GameObject target)
    {
        float dist = (target.transform.position - transform.position).magnitude;
        float dist_normalized = dist / radarRange;
        float targetNoiseLevel = target.GetComponentInChildren<StealthHider>().gameObject.GetComponent<CircleCollider2D>().radius;
        //Debug.Log($"{target} is making {targetNoiseLevel} noise");
        float intensity = targetNoiseLevel / (dist_normalized) * signalFudge;
        //Debug.Log("intensity: " + intensity);
        return intensity;

    }

    private void ClampIntensityLevelFloorToSelfNoiseInEachSector()
    {
        float rawNoiseLevel = GetComponentInChildren<StealthHider>().gameObject.GetComponent<CircleCollider2D>().radius;
        float selfNoiseLevel = (rawNoiseLevel - .5f) / 4f;
        for (int i = 0; i < 8; i++)
        {
            sectorIntensities[i] = Mathf.Clamp(sectorIntensities[i], selfNoiseLevel, 1);
        }

    }

    [TargetRpc]
    private void TargetPushSectorIntensityToRadarScreen()
    {
        for (int i = 0; i < sectorIntensities.Count; i++)
        {
            rs.AssignCurrentIntensityToEachSector(i, sectorIntensities[i]);
        }
    }

}
