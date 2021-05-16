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
    List<GameObject> targets = new List<GameObject>();
    IFF ownIFF;
    StealthHider sh;

    //param
    public float timeBetweenShots; //.25f
    public float weaponSpeed; //20
    public float weaponLifetime; //.45f
    public float weaponDamage;
    public float searchRange = 10f;
    float bulletOffset = 0.1f;
    float timeBetweenScans = 0.2f;

    //hood
    float timeSinceLastShot = 0;
    float attackRange;
    AudioClip selectedFiringSound;
    public GameObject target;
    float timeSinceLastScan = 0;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        ut = FindObjectOfType<UnitTracker>();
        ownIFF = GetComponentInChildren<IFF>();
        attackRange = weaponLifetime * weaponSpeed;
        sh = GetComponentInChildren<StealthHider>();
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
            ScanForTarget();
            FireWeaponAtTarget();
        }
    }

    private void ScanForTarget()
    {
        timeSinceLastScan -= Time.deltaTime;
        if (timeSinceLastScan <= 0)
        {
            target = ut.FindClosestTargetWithinSearchRange(gameObject, searchRange, ownIFF.GetIFFAllegiance());
            timeSinceLastScan = timeBetweenScans;
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

            if (isServer)
            {
                sh.SpikeLoudnessDueToAttack();
                DamageDealer dd = bullet.GetComponent<DamageDealer>();
                dd.SetDamage(weaponDamage);
                dd.SetAttackSource(gameObject);
            }
            NetworkServer.Spawn(bullet);

            Destroy(bullet, weaponLifetime);
            timeSinceLastShot = timeBetweenShots;
        }
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


}


