using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorGhost : MonoBehaviour
{
    //init
    SpriteRenderer sr;

    //param
    float fadeRateSensorGhost = .2f; // 5 seconds at .2f

    //hood

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        FadeOutSensorGhost();
    }

    private void FadeOutSensorGhost()
    {
        float a = Mathf.MoveTowards(sr.color.a, 0, fadeRateSensorGhost * Time.deltaTime);
        sr.color = new Color(1, 1, 1, a);
        if (a <= Mathf.Epsilon)
        {
            Destroy(gameObject);
        }
    }
}
