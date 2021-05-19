using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretPanelDriver : MonoBehaviour
{
    //init
    [SerializeField] RectTransform panel = null;
    [SerializeField] RectTransform retractPos = null;
    [SerializeField] RectTransform extendPos = null;
    [SerializeField] Image[] optionImages = null;
    [SerializeField] TextMeshProUGUI[] costsTMP = null;
    TurretMaker tm;


    //param
    float lerpSpeed = 400f; //pixels per second. panel is 200 pixel wide

    //hood
    bool isExtended = false;


    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void SetTurretMaker(TurretMaker thisTM)
    {
        tm = thisTM;
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

    public void HandleClickOnOption(int selection)
    {
        tm.PrepareOption(selection);
    }

    public void HandleClickOnCancel()
    {
        tm.ClearCurrentSelection();
    }

    public void SetOptionImages(List<Sprite> sourceSprites)
    {
        for (int i = 0; i < optionImages.Length; i++)
        {
            optionImages[i].sprite = sourceSprites[i];
        }
    }

    public void SetOptionCosts(int[] sourceCosts)
    {
        for (int i = 0; i < sourceCosts.Length; i++)
        {
            costsTMP[i].text = "$" + sourceCosts[i].ToString();
        }
    }

}
