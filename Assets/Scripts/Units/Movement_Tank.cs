using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Movement_Tank : Movement
{
    //init

    //param
    public float terrainMod_field;
    public float terrainMod_road;
    public float terrainMod_hills;
    public float terrainMod_forest;
    public float terrainMod_water;

    //hood
    public Vector3 commandedVector = new Vector3();
    float maxAngleOffBoresightToDrive = 10f;
    float angleOffCommandedVector;
    public float terrainMod;
    public float moveSpeed_Current_Terrain;

    protected override void Start()
    {
        base.Start();
        if (cs.IsRunningOnClient)
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Debug.DrawLine(transform.position, commandedVector.normalized + transform.position, Color.blue);
        UpdateCurrentMoveSpeedWithSpeedSetting();
        UpdateCurrentMoveSpeedWithTerrainModifier();
        UpdateCommandedVectorAndAngleOffIt();
    }

    private void UpdateCurrentMoveSpeedWithTerrainModifier()
    {
        int terrain = cs.currentTerrainType;
        if (terrain == 4)
        {
            terrainMod = terrainMod_road;
            moveSpeed_Current_Terrain = moveSpeed_current * terrainMod;
            return;
        }
        if (terrain == 5)
        {
            terrainMod = terrainMod_hills;
            moveSpeed_Current_Terrain = moveSpeed_current * terrainMod;
            return;

        }
        if (terrain == 6)
        {
            terrainMod = terrainMod_forest;
            moveSpeed_Current_Terrain = moveSpeed_current * terrainMod;
            return;
        }
        if (terrain == 7)
        { 
            terrainMod = terrainMod_water;
            moveSpeed_Current_Terrain = moveSpeed_current * terrainMod;
            return;
        }
        else
        {
            terrainMod = terrainMod_field;
            moveSpeed_Current_Terrain = moveSpeed_current * terrainMod;
            return;
        }

    }


    private void FixedUpdate()
    {
        RotateToCommandedVector();
        DriveAlongCommandedVector();
    }



    private void DriveAlongCommandedVector()
    {
        if (!isCommandedToMove || Mathf.Abs(angleOffCommandedVector) > maxAngleOffBoresightToDrive)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 3);
            return;
        }
        if (isCommandedToMove)
        {
            if (Mathf.Abs(angleOffCommandedVector) < maxAngleOffBoresightToDrive * 4)
            {
                rb.velocity = commandedVector * (moveSpeed_Current_Terrain / 2);
            }
            if (Mathf.Abs(angleOffCommandedVector) < maxAngleOffBoresightToDrive)
            {
                rb.velocity = commandedVector * moveSpeed_Current_Terrain;
            }
        }
    }

    private void UpdateCommandedVectorAndAngleOffIt()
    {
        commandedVector.x = cs.HorizComponent;
        commandedVector.y = cs.VertComponent;
        if (commandedVector.magnitude > 1)
        {
            commandedVector.Normalize();
        }
        angleOffCommandedVector = Vector3.SignedAngle(transform.up, commandedVector, Vector3.forward);
    }

    private void RotateToCommandedVector()
    {
        if (!isCommandedToMove)
        {
            rb.angularVelocity = 0;
            return;
        }
        if (angleOffCommandedVector > -0.1f)
        {
            rb.angularVelocity = turnSpeed_normal;
        }
        if (angleOffCommandedVector < 0.1f)
        {
            rb.angularVelocity = -turnSpeed_normal;
        }
    }
}
