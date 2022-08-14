using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCollectible : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Take bullet damage
        if (other.gameObject.layer == LayerMask.NameToLayer("player")) {
            Destroy(gameObject);
        }
    }
}
