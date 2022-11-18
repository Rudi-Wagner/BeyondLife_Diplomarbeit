using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyLogic
{
    //Stats
    [Header("Stats")]
    public float health;
    public float maxHealth;
    private float nextFire = 0f;
    private Vector2 startPos;

    private void Awake()
    {
        startPos = this.transform.position;
    }

    private void Update()
    {
        this.weapon.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        //Check Health Status
        if(this.health <= 0)
        {
            //Destroy(gameObject);  //Fürs erste später wsl wieder sinnvoll
            this.gameObject.SetActive(false);
        }

        //Firing
        if (RayCastToPlayer() && Time.time > this.nextFire)
        {
            nextFire = Time.time + this.weapon.fireRate;
            this.weapon.ShootBullet(true);
            
        }
    }

    private bool RayCastToPlayer()
    {
        //Debug.DrawRay(this.weapon.BulletSpawn.transform.position, Vector2.right * 50f, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(this.weapon.BulletSpawn.transform.position, this.weapon.BulletSpawn.transform.right, 50f);
        if (hit.collider != null)
	    {
            if(hit.collider.gameObject.CompareTag("Player"))
            {
		        return true;
            }
	    }
        return false;
    }

    protected override void destroySelf(GameObject other)
    {
        if (this.health >= 0)
        {
            BulletLogic bullet = other.gameObject.GetComponent<BulletLogic>();
            if (bullet != null)
            {
                this.health -= bullet.damage;
                return;
            }
            
            RocketLogic rocket = other.gameObject.GetComponent<RocketLogic>();
            if (rocket != null)
            {
                this.health -= rocket.damage;
                return;
            }

            AOEDamage aoeDamage = other.gameObject.GetComponent<AOEDamage>();
            if (aoeDamage != null)
            {
                this.health -= aoeDamage.damage;
                return;
            }
        }
    }

    public void ResetState()
    {
        this.health = this.maxHealth;
        this.transform.position = this.startPos;
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
