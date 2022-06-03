using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [Header("Stats")]
    public float fireRate;

    [Header("Other")]
    public GameObject Bullet;
    public GameObject BulletSpawn;

    
    public void ShootBullet()
    {
        Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
    }

    
}
