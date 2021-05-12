using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthSeeker : MonoBehaviour
{
    //init
    ControlSource cs;

    //hood
    public bool isAvatarOfLocalPlayer = false;

    void Start()
    {
        cs = GetComponentInParent<ControlSource>();
        isAvatarOfLocalPlayer = cs.CheckIfAvatarOfLocalPlayer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root == transform.root) { return; } //dont detect oneself;

        //Debug.Log("detection");

        if (isAvatarOfLocalPlayer)
        {
            //Debug.Log($"detected {collision.transform.root.name} and trying to make it visible to player");
            collision.GetComponent<StealthHider>().MakeObjectFullyVisible();
        }
        cs.RequestScan();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("back to hiding");

        if (isAvatarOfLocalPlayer)
        {
            collision.GetComponent<StealthHider>().FadeOrTurnInvisible();
        }
    }
}
