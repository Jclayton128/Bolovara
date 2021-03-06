using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Basic : Attack
{
    //init
    Transform weaponSource;
    StealthHider sh;

    //param
    float timeBetweenAttacks = 0.3f;
    float weaponSpeed = 5f;
    float weaponLifetime = .75f;
    float weaponDamage = 1f;
    float offset = .45f;
    float energyCost = 20f;

    //hood
    float timeSinceLastAttack = 0;

    protected override void Start()
    {
        base.Start();
        weaponSource = GetComponentInChildren<WeaponSource>().transform;
        sh = GetComponentInChildren<StealthHider>();


    }

    [Command]
    public override void CmdRequestAttackCommence()
    {
        Debug.Log("request attack");
        //if (!hasAuthority) { return; }  // This check isn't necessary because a computer must have authority to call a command.

        // Client-side Shot validation here, if any. 

        ExecuteAttackOnServer();
        //ExecuteAttackOnClient();  //This should be a target RPC to set the simulated attack on the client's screen, but without any colliders on bullets.
    }

    public override void AttackRelease()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        timeSinceLastAttack -= Time.deltaTime;
    }

    [Server]
    protected override void ExecuteAttackOnServer()
    {
        // Server-side validation of shot. Sufficient time between shots? Enough energy? Perhaps take in client's believed position and allow it
        // to be the source of the shot as long as its within a certain range of server-true position.
        if (timeSinceLastAttack < 0)
        {
            Debug.Log("execute attack on server");
            GameObject bullet = Instantiate(projectilePrefab, weaponSource.position + (weaponSource.up * offset), weaponSource.rotation) as GameObject;
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * weaponSpeed;
            DamageDealer dd = bullet.GetComponent<DamageDealer>();
            dd.Simulated = false;
            dd.SetDamage(weaponDamage);
            Destroy(bullet, weaponLifetime);
            timeSinceLastAttack = timeBetweenAttacks;

            sh.SpikeLoudnessDueToAttack();
            

            RpcDisplaySimAttackOnClients();
        }
    }

    [ClientRpc]
    private void RpcDisplaySimAttackOnClients()
    {
        GameObject bullet = Instantiate(projectilePrefab, weaponSource.position + (weaponSource.up * offset), weaponSource.rotation) as GameObject;
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * weaponSpeed;
        DamageDealer dd = bullet.GetComponent<DamageDealer>();
        dd.Simulated = true;
        Destroy(bullet, weaponLifetime);
    }

}
