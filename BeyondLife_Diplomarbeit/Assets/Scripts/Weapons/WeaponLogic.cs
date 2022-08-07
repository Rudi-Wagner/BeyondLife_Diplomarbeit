using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [Header("Stats")]
    public float fireRate;
    private BulletLogic bulletLogic;
    private RocketLogic rocketLogic;

    [Header("Other")]
    public GameObject Bullet = null;
    public GameObject BulletSpawn = null;
    private GameObject spawnedBullet;
    private Rigidbody2D rigidBody;

    [Header("Melee Knife")]
    public GameObject aoeDamageSphere;

    [Header("Rifle burst")]
    public float burstDelay;
    public float burstAmount;

    [Header("Shotgun spread")]
    public int pelletAmount;
    public float spreadAngle;

    [Header("Rocket Launcher")]
    public float explosionRange;

    public void Start()
    {
        bulletLogic = Bullet.GetComponent<BulletLogic>();
        rocketLogic = Bullet.GetComponent<RocketLogic>();
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

            case "RocketLauncherWeapon": 
                spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                rigidBody.velocity = BulletSpawn.transform.right * this.rocketLogic.speed;
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
            
            case "MeleeWeapon": 
                StartCoroutine(doMelee());
                break;
        }
        
    }

    public IEnumerator doMelee()
    {
        //Start Stabbing
        yield return new WaitForSeconds(0.2f);

        //Spawn Damage Sphere
        GameObject sphere = Instantiate(this.aoeDamageSphere, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
        sphere.SetActive(true);

        //End Stabbing
        yield return new WaitForSeconds(0.2f);
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
