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
    private BulletLogic bulletLogic;
    private GameObject spawnedBullet;
    private Rigidbody2D rigidBody;

    [Header("Rifle burst")]
    public float burstDelay;
    public float burstAmount;

    [Header("Shotgun spread")]
    public int pelletAmount;
    public float spreadAngle;

    public void Start()
    {
        bulletLogic = Bullet.GetComponent<BulletLogic>();
    }
    
    public void ShootBullet()
    {
        switch (this.gameObject.name)
        {
            case "PistolWeapon": 
                spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed;
                break;

            case "RifleWeapon": 
                StartCoroutine(doRifle());
                break;
            
            case "ShotgunWeapon": 
                for (int i = 0; i < pelletAmount; i++)
                {
                    spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                    float angle = Random.Range(-spreadAngle, spreadAngle);
                    rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                    rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed + new Vector3(0, angle, 0);
                }
                break;
        }
        
    }

    public IEnumerator doRifle()
    {
        for (int i = 0; i < burstAmount; i++)
        {
            spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
            rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
            rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed;
            
            yield return new WaitForSeconds(burstDelay);
        }
    }
}
