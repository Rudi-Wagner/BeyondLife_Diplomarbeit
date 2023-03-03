using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashLogic : MonoBehaviour
{
    public float lifeSpan = 0.5f;

    void Start()
    {
        StartCoroutine(destroySelfAfterDelay());
    }

    private IEnumerator destroySelfAfterDelay()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(this);
    }
}
