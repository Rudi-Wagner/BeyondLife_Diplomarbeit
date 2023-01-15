using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BasicEnemy : EnemyLogic
{
    public Animator animate { get; private set; }

    //Stats
    [Header("Stats")]
    public float health;
    public float testMovement = 0;
    public float testSprinting = 0;
    public float maxHealth;
    private float nextFire = 0f;
    private Vector2 startPos;
    public GameObject projectilePrefab; // Prefab für das Projektil
    public float fireRate = 1f; // Feuerrate der Waffe
    public float range = 10f; // Reichweite der Waffe
    private Transform player; // Transform des Charakters
    public AIPath aiPath;
    

    private void Awake()
    {
        startPos = this.transform.position;
        this.animate = GetComponent<Animator>();
         
    }
    //Paul
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform; // Charakter Transform finden
    }
    

    private void Update()
    {
        this.weapon.gameObject.SetActive(true);
        this.animate.SetFloat("Movement", testMovement);
        this.animate.SetFloat("Sprinting", testSprinting);   
        //Paul
        if(aiPath.desiredVelocity.x >= 0.1f)
        {
            transform.localScale = new Vector3 (1.5f , 1.5f , 1f);
        } 
        else if (aiPath.desiredVelocity.x <= 0.1f)
        {
            transform.localScale = new Vector3 (-1.5f , 1.5f , 1f);
        }
        // Wenn der Charakter innerhalb der Reichweite ist und es Zeit ist, wieder zu feuern
        if (Vector3.Distance(transform.position, player.position) <= range && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate; // Zeitpunkt für nächsten Schuss setzen
           this.weapon.ShootBullet(true); //Schießen
        }
        
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
        /*if (RayCastToPlayer() && Time.time > this.nextFire)
        {
            nextFire = Time.time + this.weapon.fireRate;
            this.weapon.ShootBullet(true);
            
        }*/
    }

    /*private bool RayCastToPlayer()
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
    }*/

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
