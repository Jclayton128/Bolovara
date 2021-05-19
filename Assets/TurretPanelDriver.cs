using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretPanelDriver : MonoBehaviour
{
    //init
    [SerializeField] RectTransform panel = null;
    [SerializeField] RectTransform retractPos = null;
    [SerializeField] RectTransform extendPos = null;


    //param
    float lerpSpeed = 400f; //pixels per second. panel is 200 pixel wide

    //hood
    bool isExtended = false;


    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }


    public void TogglePanelState()
    {
        isExtended = !isExtended;
    }

    private void UpdateUI()
    {
        if (isExtended)
        {
            float x = Mathf.MoveTowards(panel.position.x, extendPos.position.x, lerpSpeed * Time.deltaTime);
            panel.position = new Vector2(x, panel.position.y);
        }
        if (!isExtended)
        {
            float x = Mathf.MoveTowards(panel.position.x, retractPos.position.x, lerpSpeed * Time.deltaTime);
            panel.position = new Vector2(x, panel.position.y);
        }
    }
}
