using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Health : NetworkBehaviour
{
    //init
    [SerializeField] GameObject deathAnimoidPrefab = null;
    [SerializeField] AudioClip[] hurtAudioClips = null;
    [SerializeField] AudioClip[] dieAudioClips = null;
    [SerializeField] Sprite[] spritesByHealth = null;
    ClientInstance playerAtThisComputer;
    UIManager uim;
    public Slider healthBar;
    [SerializeField] TextMeshProUGUI healthTMP = null;

    SpriteRenderer sr;
    AudioClip chosenHurtSound;
    AudioClip chosenDieSound;
    Rigidbody2D rb;

    //param
    public float startingHealth = 1;
    public float armorLevel = 0;
    public bool canMove = false;

    //hood
    bool isDying = false;

    [SyncVar(hook = nameof(UpdateHealthBar))]
    public float currentHealth;

    GameObject ownerOfLastDamageDealerToBeHitBy;


    void Start()
    {
        if (hasAuthority)
        {
            playerAtThisComputer = ClientInstance.ReturnClientInstance();
            uim = FindObjectOfType<UIManager>();
            healthBar = uim.GetHealthBar(playerAtThisComputer);
        }
        sr = transform.root.GetComponentInChildren<SpriteRenderer>();
        currentHealth = startingHealth;
        UpdateHealthBar(0,0);
        if (canMove)
        {
            rb = transform.root.GetComponentInChildren<Rigidbody2D>();
        }
        SelectDieSound();
    }


    public void Reinitialize()
    {
        Start();
    }
    private void UpdateHealthBar(float oldValue, float newValue)
    {
        if (!healthBar) { return; }
        healthBar.maxValue = startingHealth;
        healthBar.minValue = 0;
        healthBar.value = currentHealth;
    }


    private void SelectDieSound()
    {
        if (dieAudioClips.Length == 0) { return; }
        int selectedSound = UnityEngine.Random.Range(0, dieAudioClips.Length);
        chosenDieSound = dieAudioClips[selectedSound];
    }

    // Update is called once per frame
    void Update()
    {
        LiveOrDie();
    }

    private void LiveOrDie()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDying = true;
        if (chosenDieSound)
        {

            AudioSource.PlayClipAtPoint(chosenDieSound, transform.position);
        }
        BroadcastMessage("DyingActions", SendMessageOptions.DontRequireReceiver);
        GameObject deathAnimoid = Instantiate(deathAnimoidPrefab, transform.position, transform.rotation) as GameObject;
        Animator anim = deathAnimoid.GetComponent<Animator>();
        Destroy(deathAnimoid, anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(transform.root.gameObject);
        ClientInstance.ReturnClientInstance().SetupAvatarRespawn();


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject otherGO = other.gameObject;
        HandleDamage(otherGO);
    }

    private void HandleDamage(GameObject other)
    {
        DamageDealer dd = other.GetComponent<DamageDealer>();
        if (!dd) { return; }
        if (dd.Simulated == true) { return; }
        if (dd.GetAttackSource())
        {
            ownerOfLastDamageDealerToBeHitBy = dd.GetAttackSource();
        }
        if (dd.GetKnockBackAmount() != 0)
        {
            rb.AddForce(dd.GetKnockBackAmount() * dd.GetComponent<Rigidbody2D>().velocity.normalized, ForceMode2D.Impulse);
        }
        dd.HandleImpactWithTarget(gameObject);

        float incomingDamage = dd.GetDamage();
        if (incomingDamage == 0) { return; }

        ModifyHealth(incomingDamage * -1);
        //Destroy(other);

    }

    public void ModifyHealth(float amount)
    {
        SelectHurtSound();
        if (chosenHurtSound)
        {
            AudioSource.PlayClipAtPoint(chosenHurtSound, transform.position);
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, -1, startingHealth);
        UpdateHealthBar(0,0);
        AdjustSpriteToHealthLevel();
    }

    private void AdjustSpriteToHealthLevel()
    {
        if (spritesByHealth.Length == 0) { return; }
        if (currentHealth <= startingHealth * .66f)
        {
            if (currentHealth <= startingHealth * .33f)
            {
                sr.sprite = spritesByHealth[2];
            }
            else
            {
                sr.sprite = spritesByHealth[1];
            }
        }
    }

    private void SelectHurtSound()
    {
        if (hurtAudioClips.Length == 0) { return; }
        int selectedSound = UnityEngine.Random.Range(0, hurtAudioClips.Length);
        chosenHurtSound = hurtAudioClips[selectedSound];
    }
}

