using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]

public class StealthHider : MonoBehaviour

{
    //init
    [SerializeField] Material mat = null;
    SpriteRenderer[] srs;
    Rigidbody2D rb;
    [SerializeField] CircleCollider2D hiderColl;
    ControlSource cs;
    [SerializeField] GameObject sensorGhostPrefab = null;
    //public IFF iff;
    //AllegianceManager am;


    //param
    public float hiderRadius_Base;
    public float hiderGrowthRate; //per second;
    public float hiderShrinkRate; //per second;
    public float attackSoundSpike; // This auto-forces the stealth hider to this size;
    public bool isBuilding = false;

    float playerUnitFadeAmount = 0.5f;


    //hood
    public float hiderRadius_Modified;
    public float hiderRadius_TerrainModifier = 1;
    bool isAvatarOfLocalPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
        srs = transform.root.GetComponentsInChildren<SpriteRenderer>();
        rb = transform.root.GetComponentInChildren<Rigidbody2D>();
        hiderColl = GetComponent<CircleCollider2D>();
        hiderColl.radius = hiderRadius_Base;
        cs = transform.root.GetComponentInChildren<ControlSource>();
        //iff = transform.root.GetComponentInChildren<IFF>();
        //am = FindObjectOfType<AllegianceManager>();
        if (!isBuilding)
        {
            isAvatarOfLocalPlayer = cs.CheckIfAvatarOfLocalPlayer();
        }

        if (!isAvatarOfLocalPlayer)
        {
            FadeOrTurnInvisible();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHiderRadiusInputBasedOnSpeedAndTerrain();
    }



    private void UpdateHiderRadiusInputBasedOnSpeedAndTerrain()
    {
        if (!isBuilding)
        {
            float vel = rb.velocity.magnitude;
            hiderRadius_TerrainModifier = GetTerrainModifier();
            hiderRadius_Modified = hiderRadius_Base * vel * hiderRadius_TerrainModifier;
        }
        else
        {
            hiderRadius_Modified = 0;
        }
        AdjustHiderRadius();
    }

    private float GetTerrainModifier()
    {
        int terrain = cs.currentTerrainType;

        if (terrain == 4) //road
        {
            hiderRadius_TerrainModifier = 1.25f;
            //Debug.Log("hiding in road");
            return hiderRadius_TerrainModifier;
        }
        if (terrain == 5)  //hills
        {
            hiderRadius_TerrainModifier = .5f;
            //Debug.Log("hiding in hills");
            return hiderRadius_TerrainModifier;
        }
        if (terrain == 6) //forest
        {
            hiderRadius_TerrainModifier = .75f;
            //Debug.Log("hiding in forest");
            return hiderRadius_TerrainModifier;
        }
        if (terrain == 7) //water
        {
            hiderRadius_TerrainModifier = 1.5f;
            //Debug.Log("hiding in water");
            return hiderRadius_TerrainModifier;
        }
        else
        {
            hiderRadius_TerrainModifier = 1;
            //Debug.Log("hiding in field");
            return hiderRadius_TerrainModifier;
        }
    }

    public void SpikeLoudnessDueToAttack()
    {
        Debug.Log("spike loudness due to attack");
        hiderColl.radius = attackSoundSpike;
    }
    private void AdjustHiderRadius()
    {
        //Debug.Log($"coll mod {hiderRadius_Modified} and hider rad {hiderColl.radius} at speed {rb.velocity.magnitude}");
        if (hiderRadius_Modified > hiderColl.radius)
        {
            //Debug.Log("hider radius needs to grow");
            hiderColl.radius += hiderGrowthRate * Time.deltaTime;
        }
        if (hiderRadius_Modified < hiderColl.radius)
        {
            //Debug.Log("hider radius needs to shrink");
            hiderColl.radius -= hiderShrinkRate * Time.deltaTime;
        }
        if (!isBuilding)
        {
            hiderColl.radius = Mathf.Clamp(hiderColl.radius, hiderRadius_Base / 4, hiderRadius_Base * attackSoundSpike * hiderRadius_TerrainModifier);
        }
        if (isBuilding)
        {
            hiderColl.radius = Mathf.Clamp(hiderColl.radius, 0, attackSoundSpike * hiderRadius_Base);
        }

    }
    public void FadeOrTurnInvisible()
    {
        if (isAvatarOfLocalPlayer)
        {
            return;
        }
        if (false)//iff.GetIFFAllegiance() == am.GetPlayerIFF())
        {
            SlightlyFadeOutForPlayerUnit();
        }
        else
        {
            MakeObjectInvisible();
            //Or turn turrets back into houses?
        }
    }

    private void SlightlyFadeOutForPlayerUnit()
    {
        if (isBuilding) { return; }
        foreach (SpriteRenderer thisSR in srs)
        {
            Color curCol = thisSR.color;
            thisSR.color = new Color(curCol.r, curCol.g, curCol.b, playerUnitFadeAmount);
        }
    }


    private void MakeObjectInvisible()
    {
        if (isBuilding) { return; }
        if (transform.root.tag != "Player")
        {
            CreateSensorGhost();
            foreach (SpriteRenderer thisSR in srs)
            {
                thisSR.enabled = false;
            }
        }
    }

    private void CreateSensorGhost()
    {
        float z = transform.root.GetComponentInChildren<Rigidbody2D>().rotation;
        Quaternion currentRot = Quaternion.Euler(0, 0, z);
        GameObject sg = Instantiate(sensorGhostPrefab, transform.position, currentRot) as GameObject;
        SpriteRenderer sr = sg.GetComponent<SpriteRenderer>();
        sr.sprite = srs[0].sprite;
        sr.material = mat;
    }

    public void MakeObjectFullyVisible()
    {
        if (isBuilding) { return; }

        foreach (SpriteRenderer thisSR in srs)
        {
            thisSR.enabled = true;
            Color curCol = thisSR.color;
            thisSR.color = new Color(curCol.r, curCol.g, curCol.b, 1);
        }
    }

}
