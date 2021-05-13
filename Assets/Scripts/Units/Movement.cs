
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Movement class is the base class that all unit movements derive from;

    //init
    protected ControlSource cs;
    protected Rigidbody2D rb;

    //param
    public float moveSpeed_normal;
    public float turnSpeed_normal; //deg/sec

    //hood
    protected float moveSpeed_current;
    protected float turnSpeed_current;
    protected bool isCommandedToMove = false;

    protected virtual void Start()
    {
        cs = GetComponent<ControlSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!cs)
            cs = GetComponent<ControlSource>();
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
        if (cs.IsRunningOnServer)
        {
            CheckForCommandedMovement();
        }
    }
    protected virtual void CheckForCommandedMovement()
    {
        if (Mathf.Abs(cs.HorizComponent) > Mathf.Epsilon || Mathf.Abs(cs.VertComponent) > Mathf.Epsilon) //if move commands are non-zero;
        {
            isCommandedToMove = true;
        }
        if (Mathf.Abs(cs.HorizComponent) == 0f && Mathf.Abs(cs.VertComponent) == 0f) //if move commands are non-zero;
        {
            isCommandedToMove = false;
        }
    }

    protected virtual void UpdateCurrentMoveSpeedWithSpeedSetting()
    {
        //TODO: Get and use terrain speed modifier
        float gearModifier = (cs.SpeedSetting) / 2f;
        if (cs.SpeedSetting >= 0)
        {
            moveSpeed_current = moveSpeed_normal * gearModifier;
        }
        if (cs.SpeedSetting < 0)
        {
            moveSpeed_current = 0;
        }

        //Debug.Log("CS speed setting: " + cs.speedSetting +  ". gear mod: " + gearModifier + " . MSC: " + moveSpeed_current);

    }
}
