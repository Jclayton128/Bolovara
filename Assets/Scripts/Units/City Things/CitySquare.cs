using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
[RequireComponent (typeof(IFF))]

public class CitySquare : NetworkBehaviour
{
    //init
    IFF iff;
    SpriteRenderer sr;
    [SerializeField] GameObject housePrefab = null;
    [SerializeField] GameObject turretPrefab = null;
    AllegianceManager am;

    //param
    public float CityRadius { get; protected set; } = 4f;
    int numberOfHousesToSpawn = 1;
    int numberOfTurretsToSpawn = 1;
    float timeToCapture = 2f;
    public float baseTimeBetweenHouseCreations;
    float randomTimeBetweenHouseSpawn = 2f;
    int maxCitySize = 10;

    //hood
    [SyncVar]
    string cityName;

    SyncList<Building> hic = new SyncList<Building>();
    SyncList<Building> tic = new SyncList<Building>();
    [SyncVar]
    float timeSpentCapturing = 0;
    float timeSinceLastHouseCreation = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();
        iff = GetComponent<IFF>();
        am = FindObjectOfType<AllegianceManager>();
        sr = GetComponent<SpriteRenderer>();
        SelectCityName();
        iff.SetIFFAllegiance(IFF.feralIFF);
        SpawnHousesWithinCity(numberOfHousesToSpawn);
        SpawnTurretsWithinCity(numberOfTurretsToSpawn);
        timeSinceLastHouseCreation = UnityEngine.Random.Range(0, randomTimeBetweenHouseSpawn);
        //ConvertHousesToTurrets();

        //SetAllegianceForBuildingsInCity(IFF.feralIFF);
    }

    #region creation
    private void SelectCityName()
    {
        CityManager cm = FindObjectOfType<CityManager>();
        cityName = cm.GetRandomCityName();
    }

    private void SpawnHousesWithinCity(int numberOfHouses)
    {
        Grid grid = FindObjectOfType<Grid>();
        float gridUnit = grid.cellSize.x;

        for (int i = 0; i < numberOfHouses; i++)
        {
            Vector3 actualPos = Vector3.zero;
            int attempts = 0;
            do
            {
                Vector3 gridSnappedPos = Vector3.zero;
                Vector2 pos = UnityEngine.Random.insideUnitCircle * CityRadius;
                Vector3 pos3 = pos;
                gridSnappedPos = new Vector3(Mathf.Round(pos.x / gridUnit), Mathf.Round(pos.y / gridUnit), 0);
                Vector3 halfStep = (new Vector3(1, 1, 0)) * gridUnit / 2f;
                actualPos = transform.position + gridSnappedPos + halfStep;
                attempts++;
                if (attempts > 10)
                {
                    break;
                }
            }
            while (!(IsTestLocationValid_NavMesh(actualPos) & IsTestLocationValid_Physics(actualPos)));

            GameObject newHouse = Instantiate(housePrefab, actualPos, housePrefab.transform.rotation) as GameObject;
            Building house = newHouse.GetComponent<Building>();
            house.am = am;
            house.SetHouseIFFAllegiance(iff.GetIFFAllegiance());
            //house.InitializeBuilding();
            house.SetOwningCity(this);
            NetworkServer.Spawn(newHouse);
            hic.Add(house);
        }
    }

    private void SpawnTurretsWithinCity(int numberOfTurrets)
    {
        Grid grid = FindObjectOfType<Grid>();
        float gridUnit = grid.cellSize.x;
        for (int i = 0; i < numberOfTurrets; i++)
        {
            Vector3 actualPos = Vector3.zero;
            do
            {
                Vector3 gridSnappedPos = Vector3.zero;
                Vector2 pos = UnityEngine.Random.insideUnitCircle * CityRadius;
                Vector3 pos3 = pos;
                gridSnappedPos = new Vector3(Mathf.Round(pos.x / gridUnit), Mathf.Round(pos.y / gridUnit), 0);
                Vector3 halfStep = (new Vector3(1, 1, 0)) * gridUnit / 2f;
                actualPos = transform.position + gridSnappedPos + halfStep;

            }
            while (!(IsTestLocationValid_NavMesh(actualPos) & IsTestLocationValid_Physics(actualPos)));

            GameObject newTurret = Instantiate(turretPrefab, actualPos, housePrefab.transform.rotation) as GameObject;
            Building turret = newTurret.GetComponent<Building>();

            turret.am = am;
            turret.SetHouseIFFAllegiance(iff.GetIFFAllegiance());
            //house.InitializeBuilding();
            turret.SetOwningCity(this);
            NetworkServer.Spawn(newTurret);
            tic.Add(turret);
        }
    }

    private void ConvertHousesToTurrets()
    {
        for (int i = 0; i < numberOfTurretsToSpawn; i++)
        {
            if (hic.Count == 0) { return; }
            int random = UnityEngine.Random.Range(0, hic.Count);
            GameObject houseToReplace = hic[random].gameObject;
            GameObject newTurret = Instantiate(turretPrefab, houseToReplace.transform.position, turretPrefab.transform.rotation) as GameObject;
            Building turret = newTurret.GetComponent<Building>();
            turret.am = am;
            turret.SetOwningCity(this);
            NetworkServer.Spawn(newTurret);
            tic.Add(turret);
            hic.Remove(houseToReplace.GetComponent<Building>());
            NetworkServer.UnSpawn(houseToReplace);
            Destroy(houseToReplace);
        }
    }
    private bool IsTestLocationValid_Physics(Vector3 testPos)
    {
        Collider2D rchit = Physics2D.OverlapCircle(testPos, 0.3f, 1 << 10);
        if (rchit)
        {
            //Debug.Log($"invalid due to physics at {rchit.transform.position} on {rchit.transform.gameObject.name}");
            return false;
        }
        else
        {
            //Debug.Log("physics is good");
            return true;
        }
    }
    private bool IsTestLocationValid_NavMesh(Vector3 testPos)
    {
        NavMeshHit hit;
        NavMeshQueryFilter filter = new NavMeshQueryFilter();
        filter.areaMask = NavMesh.AllAreas;
        filter.agentTypeID = GameObject.FindGameObjectWithTag("NavMeshBuildings").GetComponent<NavMeshSurface2d>().agentTypeID;
        NavMesh.SamplePosition(testPos, out hit, 0.1f, filter);
        bool[] layersFound = LayerMaskExtensions.HasLayers(hit.mask);

        if (layersFound[0])
        {
            //Debug.Log($"0 is good at {testPos}");
            return true;
        }
        else
        {
            //Debug.Log($"Invalid at {testPos}");
            return false;
        }

    }

    public void SetAllegianceForBuildingsInCity(int newIFF)
    {
        Debug.Log($"{hic.Count} houses should be changing iff state to {iff.GetIFFAllegiance()}");
        //Debug.Log($"{turretsInCity.Count} turrets should be changing iff state to {iff.GetIFFAllegiance()}");
        foreach (Building house in hic)
        {
            house.SetHouseIFFAllegiance(newIFF);
            house.FindCurrentOwner();
        }
        foreach (Building turret in tic)
        {
            turret.SetHouseIFFAllegiance(newIFF);
            turret.FindCurrentOwner();
        }
    }


    #endregion

    #region capturing

    private void Update()
    {
        timeSpentCapturing -= (Time.deltaTime / 2f);  //Constant drain on capture time.
        timeSpentCapturing = Mathf.Clamp(timeSpentCapturing, 0, timeToCapture + 1);

        if (isServer)
        {
            timeSinceLastHouseCreation += Time.deltaTime;
            if (hic.Count < maxCitySize && timeSinceLastHouseCreation >= baseTimeBetweenHouseCreations * hic.Count)
            {
                SpawnHousesWithinCity(1);
                timeSinceLastHouseCreation = UnityEngine.Random.Range(0, randomTimeBetweenHouseSpawn); ;
            }
        }

    }

    public void BuildCaptureTime(float time)
    {
        timeSpentCapturing += time;
    }
    public void ResetCaptureStatus()
    {
        timeSpentCapturing = 0;
    }

    public float GetTimeSpentCapturing()
    {
        return timeSpentCapturing;
    }

    public float GetTimeRequiredToCapture()
    {
        return timeToCapture;
    }

    #endregion

    public string GetCityName()
    {
        return cityName;
    }
    public void RemoveBuildingFromList(Building deadThing)
    {
        hic.Remove(deadThing);
        tic.Remove(deadThing);
    }
}
