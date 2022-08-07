using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamage : MonoBehaviour
{
    [Header("Stats")]
    public float lifeSpan;
    public float damage;

    private void OnEnable()
    {
        if(lifeSpan == -1)
        {
            return;     //Never Kill this AOEDamageSphere
        }
        Invoke(nameof(disable), lifeSpan);
    }

    private void disable()
    {
        Destroy(gameObject);
    }
        
}
