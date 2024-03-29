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

    [Header("Muzzleflash")]
    public bool muzzleflashActivated = true;
    public GameObject Flash;

    [Header("Other")]
    public GameObject Bullet = null;
    public bool freezeRotation = false;
    public GameObject BulletSpawn = null;
    private GameObject spawnedBullet;
    private Rigidbody2D rigidBody;

    [Header("Melee Knife")]
    public GameObject aoeDamageSphere;
    public AnimatorOverrideController overrideControllerStartMelee;
    public AnimatorOverrideController overrideControllerEndMelee;
    public AnimatorOverrider overrider;

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
                if(muzzleflashActivated) Instantiate(this.Flash, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                spawnedBullet.GetComponent<BulletLogic>().damagePlayer = damagePlayer;
                rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed;
                break;

            case "SniperWeapon":
                spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                if(muzzleflashActivated) Instantiate(this.Flash, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                spawnedBullet.GetComponent<BulletLogic>().damagePlayer = damagePlayer;
                rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed;
                break;

            case "RocketLauncherWeapon": 
                spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                if(muzzleflashActivated) Instantiate(this.Flash, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
                rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
                rigidBody.velocity = BulletSpawn.transform.right * this.rocketLogic.speed;
                break;

            case "RifleWeapon": 
                StartCoroutine(doRifle(damagePlayer));
                break;
            
            case "ShotgunWeapon": 
                if(muzzleflashActivated) Instantiate(this.Flash, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
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
        //Get Player Script & disable PlayerControl
        PlayerLogic player = this.gameObject.transform.parent.gameObject.GetComponent<PlayerLogic>();
        player.allowArmMovement = false;
        player.InputAllowed = false;
        player.rigidBody.velocity = Vector2.zero;
        overrider.SetAnimations(overrideControllerStartMelee);
        
        //Get/Reset Arm Position
        Transform LimbSolver = this.gameObject.transform.parent.gameObject.transform.Find("rightArmLimbSolver2D");
        LimbSolver.position = player.gameObject.transform.position + new Vector3(-1.458f, -0.638f, 0f);
        Transform CCDSolver = this.gameObject.transform.parent.gameObject.transform.Find("rightArmCCDSolver2D");
        CCDSolver.position = player.gameObject.transform.position + new Vector3(-1.066f, -1.358f, 0f);

        //Start Animation
        player.animate.Play("Player_Placeholder");
        player.animate.SetBool("ReleasePlaceholder", true);

        //End Stabbing
        yield return new WaitForSeconds(1f);
        overrider.SetAnimations(overrideControllerEndMelee);
        player.allowArmMovement = true;
        player.InputAllowed = true;
    }

    public IEnumerator doRifle(bool damagePlayer)
    {
        for (int i = 0; i < burstAmount; i++)
        {
            spawnedBullet = Instantiate(this.Bullet, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
            if(muzzleflashActivated) Instantiate(this.Flash, this.BulletSpawn.transform.position, this.BulletSpawn.transform.rotation);
            spawnedBullet.GetComponent<BulletLogic>().damagePlayer = damagePlayer;
            rigidBody = spawnedBullet.GetComponent<Rigidbody2D>();
            rigidBody.velocity = BulletSpawn.transform.right * this.bulletLogic.speed;
            
            yield return new WaitForSeconds(burstDelay);
        }
    }
}
