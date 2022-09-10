using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [Header("Stats")]
    public float fireRate;
    public int ammunition;
    public int maxAmmunition;
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
        if(this.maxAmmunition > 0)
        {
            this.ammunition = this.maxAmmunition;
        }
        else
        {
            this.ammunition = 360;
        }
        this.gameObject.SetActive(false);
    }
    
    public void ShootBullet(bool damagePlayer)
    {
        if(this.ammunition <= 0)
        {
            return;
        }

        switch (this.gameObject.name)
        {
            case "PistolWeapon": 
                spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                spawnedBullet.GetComponent<BulletLogic>().damagePlayer = damagePlayer;
                rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed;
                break;

            case "RocketLauncherWeapon": 
                spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                rigidBody.velocity = BulletSpawn.transform.right * this.rocketLogic.speed;
                break;

            case "RifleWeapon": 
                StartCoroutine(doRifle(damagePlayer));
                break;
            
            case "ShotgunWeapon": 
                for (int i = 0; i < pelletAmount; i++)
                {
                    spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                    spawnedBullet.GetComponent<BulletLogic>().damagePlayer = damagePlayer;
                    float angle = Random.Range(-spreadAngle, spreadAngle);
                    rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                    rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed + new Vector3(0, angle, 0);
                }
                break;
            
            case "MeleeWeapon": 
                StartCoroutine(doMelee());
                break;
        }

        if(this.maxAmmunition > 0)
        {
            this.ammunition--;
        }
    }

    public IEnumerator doMelee()
    {
        //Start Stabbing
        //yield return new WaitForSeconds(0.2f);

        //Spawn Damage Sphere                                                                                                           Child of the BulletSpawn Point
        GameObject sphere = Instantiate(this.aoeDamageSphere, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation, this.BulletSpawn.transform);
        sphere.SetActive(true);

        //End Stabbing
        yield return new WaitForSeconds(0f);
    }

    public IEnumerator doRifle(bool damagePlayer)
    {
        for (int i = 0; i < burstAmount; i++)
        {
            spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
            spawnedBullet.GetComponent<BulletLogic>().damagePlayer = damagePlayer;
            rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
            rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed;
            
            yield return new WaitForSeconds(burstDelay);
        }
    }
}
