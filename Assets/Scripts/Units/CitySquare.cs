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
    int numberOfHousesToSpawn = 6;
    int numberOfTurretsToSpawn = 1;

    //hood
    public string cityName { get; protected set; }
    public List<Building> housesInCity = new List<Building>();
    public List<Building> turretsInCity = new List<Building>();

    void Start()
    {
        iff = GetComponent<IFF>();
        am = FindObjectOfType<AllegianceManager>();
        sr = GetComponent<SpriteRenderer>();
        SelectCityName();
        SpawnHousesWithinCity(numberOfHousesToSpawn);
        ConvertHousesToTurrets();
        SetAllegianceForBuildingsInCity(iff.GetIFFAllegiance());

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
            do
            {
                Vector3 gridSnappedPos = Vector3.zero;
                Vector2 pos = UnityEngine.Random.insideUnitCircle * CityRadius;
                Vector3 pos3 = pos;
                gridSnappedPos = new Vector3(Mathf.Round(pos.x / gridUnit), Mathf.Round(pos.y / gridUnit), 0);

                //if (Mathf.Abs(gridSnappedPos.x) < cityMinDistFromSquare)
                //{
                //    float sign = Mathf.Sign(gridSnappedPos.x);
                //    gridSnappedPos.x = cityMinDistFromSquare * sign;
                //}
                //if (Mathf.Abs(gridSnappedPos.y) < cityMinDistFromSquare)
                //{
                //    float sign = Mathf.Sign(gridSnappedPos.y);
                //    gridSnappedPos.y = cityMinDistFromSquare * sign;
                //}

                Vector3 halfStep = (new Vector3(1, 1, 0)) * gridUnit / 2f;
                actualPos = transform.position + gridSnappedPos + halfStep;
                //Debug.Log($"generated pos: {pos}, which is dist {(transform.position - pos3).magnitude} and {gridSnappedPos} is gsp.  ActualPos is {actualPos}. Distance is {(transform.position - actualPos).magnitude}");

            }
            while (!(IsTestLocationValid_NavMesh(actualPos) & IsTestLocationValid_Physics(actualPos)));

            GameObject newHouse = Instantiate(housePrefab, actualPos, housePrefab.transform.rotation) as GameObject;
            Building house = newHouse.GetComponent<Building>();
            housesInCity.Add(house);
            house.am = am;
            house.InitializeBuilding();
            house.SetOwningCity(this);
        }
    }

    private void ConvertHousesToTurrets()
    {
        for (int i = 0; i < numberOfTurretsToSpawn; i++)
        {
            if (housesInCity.Count == 0) { return; }
            int random = UnityEngine.Random.Range(0, housesInCity.Count);
            GameObject houseToReplace = housesInCity[random].gameObject;
            GameObject newTurret = Instantiate(turretPrefab, houseToReplace.transform.position, turretPrefab.transform.rotation) as GameObject;
            Building turret = newTurret.GetComponent<Building>();
            turret.am = am;
            turret.SetOwningCity(this);
            turretsInCity.Add(turret);
            housesInCity.Remove(houseToReplace.GetComponent<Building>());
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
        //Debug.Log($"{housesInCity.Count} houses should be changing iff state to {iff.GetIFFAllegiance()}");
        //Debug.Log($"{turretsInCity.Count} turrets should be changing iff state to {iff.GetIFFAllegiance()}");
        foreach (Building house in housesInCity)
        {
            house.SetHouseIFFAllegiance(newIFF);
            house.UpdateCurrentOwner();
        }
        foreach (Building turret in turretsInCity)
        {
            turret.SetHouseIFFAllegiance(newIFF);
            turret.UpdateCurrentOwner();
        }
    }
    #endregion  



    public void RemoveBuildingFromList(Building deadThing)
    {
        housesInCity.Remove(deadThing);
        turretsInCity.Remove(deadThing);
    }
}
