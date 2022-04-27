using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyLogic
{
    public float health;
    private float nextFire = 0f;
    public WeaponLogic weapon;


    private void FixedUpdate()
    {
        //Check Health Status
        if(this.health <= 0)
        {
            Destroy(gameObject);
        }

        //Firing
        if (RayCastToPlayer() && Time.time > this.nextFire)
        {
            nextFire = Time.time + this.weapon.fireRate;
            this.weapon.ShootBullet();
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
            this.health -= bullet.damage;
        }
    }
}
