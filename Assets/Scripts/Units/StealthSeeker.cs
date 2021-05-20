using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]

public class StealthSeeker : MonoBehaviour
{
    //init
    ControlSource cs;
    CircleCollider2D collider;

    //param
    public float SeekerRange;
    public float SeekerChangeRate; 

    //hood
    public bool isAvatarOfLocalPlayer = false;

    public Action<GameObject> OnSeekerDetection;
    public Action<GameObject> OnSeekerLostContact;

    void Start()
    {
        cs = GetComponentInParent<ControlSource>();
        if (cs)
        {
            isAvatarOfLocalPlayer = cs.CheckIfAvatarOfLocalPlayer();
        }
        collider = GetComponent<CircleCollider2D>();
        collider.radius = 0.001f;
    }

    // Update is called once per frame
    void Update()
    {
        GrowShrinkSeekerRange();
    }

    private void GrowShrinkSeekerRange()
    {
        if (collider.radius < SeekerRange)
        {
            //grow collider
            collider.radius += SeekerChangeRate * Time.deltaTime;
        }
        if (collider.radius > SeekerRange)
        {
            //shrink collider
            collider.radius -= SeekerChangeRate * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root == transform.root) { return; } //dont detect oneself;

        //Debug.Log("detection");

        OnSeekerDetection?.Invoke(collision.transform.root.gameObject);

        if (isAvatarOfLocalPlayer)
        {
            //Debug.Log($"detected {collision.transform.root.name} and trying to make it visible to player");
            collision.GetComponent<StealthHider>().MakeObjectFullyVisible();
            
        }
        //cs.RequestScan();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("back to hiding");

        OnSeekerLostContact?.Invoke(collision.transform.root.gameObject);
        if (isAvatarOfLocalPlayer)
        {
            collision.GetComponent<StealthHider>().FadeOrTurnInvisible();
        }
    }
}
