using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("bullet")) {
            destroySelf(other.gameObject);
        }
    }

    protected virtual void destroySelf(GameObject other)
    {
        Destroy(gameObject);
    }
}
