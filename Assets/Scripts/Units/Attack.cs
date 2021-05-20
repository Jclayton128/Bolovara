using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Attack : NetworkBehaviour
{
    //Attack class is a base class that all unit's attack logic is handled at 

    //init
    protected ControlSource cs;
    [SerializeField] protected GameObject projectilePrefab = null;

    //param
    public float projectileSpeed;
    public float projectileLifetime;



    protected virtual void Start()
    {
        cs = GetComponentInParent<ControlSource>();
    }

    public abstract void CmdRequestAttackCommence();

    public abstract void AttackRelease();
    public virtual float GetAttackRange()
    {
        float range = projectileLifetime * projectileSpeed;
        return range;
    }

    protected abstract void ExecuteAttackOnServer();


}
