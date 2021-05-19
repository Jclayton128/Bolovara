using UnityEngine;
using TMPro;
using System;
using Mirror;
using UnityEngine.UI;

public class PlayerInput : ControlSource
{
    //init
    Attack attack;
    UIManager uim;
    GameObject shiftKnob;
    [SerializeField] Transform[] gearShiftPositions = null;
    TextMeshProUGUI followMeText;
    Image flagImage;
    AllegianceManager am;
    IFF iff;
    TurretPanelDriver tpd;
    TurretMaker tm;

    //param

    //hood
    public bool LMBdown = false;
    public bool RMBdown = false;
    Vector3 mousePos = new Vector3(0, 0, 0);
    ClientInstance playerAtThisComputer;

    float desHoriz;
    float desVert;
    int desSpeedSetting = 1;
    Vector3 desAimDir = Vector3.zero;

    protected override void Start()
    {
        base.Start();
        attack = GetComponent<Attack>();
        am = FindObjectOfType<AllegianceManager>();
        HookIntoLocalUISystems();
    }

    private void HookIntoLocalUISystems()
    {
        if (hasAuthority)
        {
            playerAtThisComputer = ClientInstance.ReturnClientInstance();
            uim = FindObjectOfType<UIManager>();
            shiftKnob = uim.GetShiftKnob(playerAtThisComputer);
            uim.GetShiftPositions(playerAtThisComputer, out gearShiftPositions[0], out gearShiftPositions[1], out gearShiftPositions[2]);
            tpd = uim.GetTPD(playerAtThisComputer);
            iff = GetComponent<IFF>();
            int myIFF = playerAtThisComputer.GetComponent<FactionLeader>().GetMasterIFFAllegiance();
            iff.SetIFFAllegiance(myIFF);
            flagImage = uim.GetFlagUIElement(playerAtThisComputer);
            flagImage.sprite = am.GetFlagOfAllegiance(myIFF);           
        }
    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (hasAuthority)
        {
            HandleKeyboardInput();
            HandleMouseInput();
            CmdSendServerSideDesiredInput(desHoriz, desVert, desSpeedSetting, desAimDir);
        }       
    }

    private void HandleMouseInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) )
        {
            LMBdown = true;
            if (attack.GetBlockedByUIStatus() == false)
            {
                attack.CmdRequestAttackCommence();
            }


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
        desAimDir = (mousePos - transform.position).normalized;
    }

    private void HandleKeyboardInput()
    {
        desHoriz = Input.GetAxis("Horizontal");
        desVert = Input.GetAxis("Vertical");
        HandleGearShifting();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tpd.TogglePanelState();
        }
    }
    private void HandleGearShifting()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            desSpeedSetting++;
            //TODO: Play an audioclip with gear shifting 'clunk'
            if (desSpeedSetting > gearShiftPositions.Length)
            {
                desSpeedSetting = 1;
            }
        }

        if (!shiftKnob) { return; }
        shiftKnob.transform.position = gearShiftPositions[desSpeedSetting - 1].transform.position;
    }

    [Command]
    private void CmdSendServerSideDesiredInput(float horiz, float vert, int speed, Vector3 aim)
    {
        HorizComponent = horiz;
        VertComponent = vert;
        SpeedSetting = speed;
        AimDir = aim;
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
