using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotater : MonoBehaviour
{
    //init
    [SerializeField] Transform turret = null;
    ControlSource cs;

    //param
    public float rotationSpeed = 360;


    //hood
    public float angToAimDir;
    

    void Start()
    {
        cs = GetComponent<ControlSource>();
    }

    // Update is called once per frame
    void Update()
    {
        angToAimDir = Vector3.SignedAngle(turret.transform.up, cs.AimDir, Vector3.forward);
    }

    private void FixedUpdate()
    {
        RotateTurretToMouseDir();
    }

    private void RotateTurretToMouseDir()
    {
        if (Mathf.Abs(angToAimDir) <= .1f)
        {
            return;
        }
        if (angToAimDir > 0.1)
        {
            turret.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        if (angToAimDir < -.1)
        {
            turret.transform.Rotate(Vector3.forward, -1 * rotationSpeed * Time.deltaTime);
        }
    }


}
