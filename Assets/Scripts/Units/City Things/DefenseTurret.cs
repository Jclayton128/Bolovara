using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class DefenseTurret : NetworkBehaviour
{
    //init
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] AudioClip[] firingSounds = null;
    UnitTracker ut;
    [SerializeField] List<GameObject> targets = new List<GameObject>();
    IFF ownIFF;
    StealthHider sh;
    [SerializeField] StealthSeeker ss;

    //param
    public float timeBetweenShots; //.25f
    public float weaponSpeed; //20
    public float weaponLifetime; //.45f
    public float weaponDamage;
    float bulletOffset = 0.1f;  //.4 does it

    //hood
    float timeSinceLastShot = 0;
    float attackRange;
    AudioClip selectedFiringSound;
    public GameObject target;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        ut = FindObjectOfType<UnitTracker>();
        ownIFF = GetComponent<IFF>();
        attackRange = weaponLifetime * weaponSpeed;
        sh = GetComponentInChildren<StealthHider>();
        ss = GetComponentInChildren<StealthSeeker>();
        ss.OnSeekerDetection += EvaluateDetectedObject;
        ss.OnSeekerLostContact += HandleLostContact;
        ut.AddUnitToTargetableList(gameObject);
        ownIFF.OnChangeIFF += CheckForAlliesAsTargetsAfterIFFChange;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        selectedFiringSound = SelectSoundFromArray(firingSounds);
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            PrioritizeTargets();
            FireWeaponAtTarget();
        }
    }

    private void EvaluateDetectedObject(GameObject go)
    {
        Debug.Log("Evaluating detected object " + go.name);
        IFF goIFF;
        if (go.TryGetComponent<IFF>(out goIFF) && go.GetComponent<ControlSource>() != null)
        {
            int targetIFF = goIFF.GetIFFAllegiance();
            if (targetIFF != ownIFF.GetIFFAllegiance())
            {
                targets.Add(go);
            }
        }
    }

    private void HandleLostContact(GameObject go)
    {
        Debug.Log("lost contact on " + go.name);
        targets.Remove(go);
    }

    private void CheckForAlliesAsTargetsAfterIFFChange(int newIFF)
    {
        Debug.Log("check is firing!");
        targets.RemoveAll(CheckForEnemy);
        ss.ResetDetector();   
    }

    private bool CheckForEnemy(GameObject go)
    {
        if (go.GetComponent<IFF>().GetIFFAllegiance() != ownIFF.GetIFFAllegiance())
        {
            Debug.Log("Found an enemy after reviewing target list");
            return false;
        }
        else
        {
            Debug.Log("Found an ally after reviewing target list");
            return true;
        }
    }

    private void PrioritizeTargets()
    {
        if (targets.Count > 0)
        {
            target = targets[0]; // TODO: maybe have some prioritization in the turret's target if more than one target
        }
        else
        {
            target = null;
        }
    }

    private void FireWeaponAtTarget()
    {
        timeSinceLastShot -= Time.deltaTime;
        if (!target) { return; }
        Vector3 dir = target.transform.position - transform.position;
        float diff = dir.magnitude;
        //Debug.Log("distance: " + diff + ". AtkRng: " + attackRange);
        if (diff <= attackRange && timeSinceLastShot <= 0)
        {
            if (isClient && firingSounds.Length > 0)
            {
                AudioSource.PlayClipAtPoint(selectedFiringSound, transform.position);
                selectedFiringSound = SelectSoundFromArray(firingSounds);
            }

            float zAng = Vector3.SignedAngle(Vector2.up, dir, Vector3.forward);
            Quaternion rot = Quaternion.Euler(0, 0, zAng);
            GameObject bullet = Instantiate(weaponPrefab, transform.position + (dir*bulletOffset),rot) as GameObject;
            bullet.GetComponent<Rigidbody2D>().velocity = weaponSpeed * bullet.transform.up;

            DamageDealer dd = bullet.GetComponent<DamageDealer>();

            sh.SpikeLoudnessDueToAttack();
            dd.SetDamage(weaponDamage);
            dd.SetAttackSource(gameObject);
            RpcDisplaySimAttackOnClients(transform.position + (dir * bulletOffset), rot);
 

            Destroy(bullet, weaponLifetime);
            timeSinceLastShot = timeBetweenShots;
        }
    }

    [ClientRpc]
    private void RpcDisplaySimAttackOnClients(Vector3 pos, Quaternion rot)
    {
        GameObject bullet = Instantiate(weaponPrefab, pos, rot) as GameObject;
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * weaponSpeed;
        DamageDealer dd = bullet.GetComponent<DamageDealer>();
        dd.Simulated = true;
        dd.SetDamage(0);
        dd.SetAttackSource(gameObject);
        Destroy(bullet, weaponLifetime);
    }

    private AudioClip SelectSoundFromArray(AudioClip[] audioArray)
    {
        if (audioArray.Length == 0)
        {
            return null;
        }
        int random = UnityEngine.Random.Range(0, audioArray.Length);
        return audioArray[random];
    }

    private void OnDestroy()
    {
        if (isServer)
        {
            ut.RemoveUnitFromTargetableList(gameObject);
            ownIFF.OnChangeIFF -= CheckForAlliesAsTargetsAfterIFFChange;
        }

    }

}


