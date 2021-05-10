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
    public float rotationSpeed;


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
        float factor = Mathf.Clamp(angToAimDir/10, -1, 1);
        if (Mathf.Abs(angToAimDir) <= .1f)
        {
            return;
        }
        if (angToAimDir > 0.1)
        {
            turret.transform.Rotate(Vector3.forward, rotationSpeed * factor * Time.deltaTime);
        }
        if (angToAimDir < -.1)
        {
            turret.transform.Rotate(Vector3.forward, rotationSpeed * factor * Time.deltaTime);
        }
    }


}
