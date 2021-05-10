using UnityEngine;
using TMPro;
using System;
using Mirror;

public class PlayerInput : ControlSource
{
    //init
    Attack attack;

    //param

    //hood
    public bool LMBdown = false;
    public bool RMBdown = false;
    Vector3 mousePos = new Vector3(0, 0, 0);

    protected override void Start()
    {
        base.Start();
        attack = GetComponent<Attack>();

    }

 
    // Update is called once per frame
    protected override void Update()
    {
        if (hasAuthority)
        {
            base.Update();
            HandleKeyboardInput();
            HandleMouseInput();
        }       
    }

    private void HandleMouseInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LMBdown = true;
            attack.CmdRequestAttackCommence();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            LMBdown = false;
            //attack.AttackRelease();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RMBdown = true;
            //attack.RMBDown();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            RMBdown = false;
            //attack.RMBUp();
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        AimDir = (mousePos - transform.position).normalized;
    }

    private void HandleKeyboardInput()
    {
        HorizComponent = Input.GetAxis("Horizontal");
        VertComponent = Input.GetAxis("Vertical");
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void Scan()
    {
        //players have to do their own scanning...
    }

}
