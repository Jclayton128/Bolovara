using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : MonoBehaviour
{
    [SerializeField] GameObject deathAnimation = null;

    private void OnDestroy()
    {
        GameObject g = Instantiate(deathAnimation, transform.position, Quaternion.identity) as GameObject;
        Destroy(g, g.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }

}
