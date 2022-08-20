using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    public float health;

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Take bullet damage
        if (other.gameObject.layer == LayerMask.NameToLayer("player")) 
        {
            PlayerLogic player = other.gameObject.GetComponent<PlayerLogic>();
            player.health += this.health;
            if (player.health > player.maxHealth)
            {
                player.health = player.maxHealth;
            }
            Destroy(gameObject);
        }
    }
}
