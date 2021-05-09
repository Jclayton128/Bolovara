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
        if (!hasAuthority) { return; }  // This check isn't necessary because a computer must have authority to call a command.

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
            GameObject bullet = Instantiate(projectilePrefab, transform.position + (transform.up * offset), transform.rotation) as GameObject;
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * weaponSpeed;
            //NetworkServer.Spawn(bullet);
            //NetworkServer.Destroy(shell); //TODO find a way to destroy these objects via NetworkServer.Destroy after the lifetime is met
            Destroy(bullet, weaponLifetime);
            timeSinceLastAttack = timeBetweenAttacks;

            RpcDisplaySimAttackOnClients();
        }
    }

    [ClientRpc]
    private void RpcDisplaySimAttackOnClients()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position + (transform.up * offset), transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * weaponSpeed;
        //turn off spawned bullet's collider since the client's instance is simulated and not true
        Destroy(bullet, weaponLifetime);

    }

}
