using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Basic : Attack
{
    //param
    float timeBetweenAttacks = 0.3f;
    float weaponSpeed = 10f;
    float weaponLifetime = .75f;
    float weaponDamage = 1f;
    float offset = .5f;
    float energyCost = 20f;

    //hood
    float timeSinceLastAttack = 0;

    protected override void Start()
    {
        base.Start();
    }

    [Command]
    public override void CmdRequestAttackCommence()
    {
        Debug.Log("request attack");
        //Client-side Shot validation here, if any.
        if (!hasAuthority) { return; }
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
        //Server-side validation of shot. Time between shots? Enough energy?
        if (timeSinceLastAttack < 0)
        {
            Debug.Log("execute attack on server");
            GameObject bullet = Instantiate(projectilePrefab, transform.position + (transform.up * offset), transform.rotation) as GameObject;
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * weaponSpeed;
            NetworkServer.Spawn(bullet);
            //NetworkServer.Destroy(shell); //TODO find a way to destroy these objects via NetworkServer.Destroy after the lifetime is met
            //Destroy(shell, weaponLifetime);
            timeSinceLastAttack = timeBetweenAttacks;
        }
    }

}
