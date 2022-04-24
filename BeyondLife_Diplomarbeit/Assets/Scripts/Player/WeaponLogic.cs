using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    public GameObject Bullet;
    public GameObject BulletSpawn;

    public float fireRate;
    public void ShootBullet()
    {

        Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
    }
}
