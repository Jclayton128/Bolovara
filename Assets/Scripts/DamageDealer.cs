﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    //init
    [SerializeField] GameObject weaponImpactAnimationPrefab = null;
    Rigidbody2D rb;

    //param

    //hood
    public bool Simulated { get; set; } = false;
    float damage;
    float knockback;
    GameObject attackSource = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetAttackSource(GameObject obj)
    {
        attackSource = obj;
    }

    public GameObject GetAttackSource()
    {
        return attackSource;
    }
    public float GetDamage()
    {
        return damage;
    }
    public void SetDamage(float value)
    {
        damage = value;
    }

    public float GetKnockBackAmount()
    {
        return knockback;
    }

    public void SetKnockBackAmount(float amount)
    {
        knockback = amount;
    }

    public void HandleImpactWithTarget(GameObject targetGO)
    {
        if (!weaponImpactAnimationPrefab) { return; }

        GameObject animation = Instantiate(weaponImpactAnimationPrefab, transform.position, transform.rotation) as GameObject;
        //animation.transform.parent = targetGO.transform;
        Animator anim = animation.GetComponent<Animator>();
        Destroy(animation, anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!weaponImpactAnimationPrefab) { return; }
        if (collision.gameObject == transform.root.gameObject) { return; }
        if (!collision.enabled) { return; }

        GameObject animation = Instantiate(weaponImpactAnimationPrefab, transform.position, transform.rotation) as GameObject;
        animation.transform.parent = collision.transform;
        Animator anim = animation.GetComponent<Animator>();
        Destroy(animation, anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }


}