using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmunitionContainer : MonoBehaviour
{
    public int ammunition;
    public int ammoType;
    /*
      0 --> Pistol
      1 --> Rifle
      2 --> Shotgun
      3 --> Rocket
    */

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Take bullet damage
        if (other.gameObject.layer == LayerMask.NameToLayer("player")) 
        {
            PlayerLogic player = other.gameObject.GetComponent<PlayerLogic>();
            player.weapons[ammoType].ammunition += this.ammunition;
            if (player.weapons[ammoType].ammunition > player.weapons[ammoType].maxAmmunition)
            {
                player.weapons[ammoType].ammunition = player.weapons[ammoType].maxAmmunition;
            }
            Destroy(gameObject);
        }
    }
}
