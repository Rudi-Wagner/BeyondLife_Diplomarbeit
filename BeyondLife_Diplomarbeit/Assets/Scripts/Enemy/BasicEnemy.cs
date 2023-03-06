using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BasicEnemy : EnemyLogic
{
    //Stats
    [Header("Stats")]
    public float health;
    public float testMovement = 0;
    public float testSprinting = 0;
    public float maxHealth;
    private float nextFire = 0f;
    private Vector2 startPos;
    private Transform player; // Transform des Charakters
    public float range = 10f; // Reichweite der Waffe
    public Animator animate { get; private set; }

    [Header("Death Fadeout")]
    public float fadeStrength = 0.1f;
    public float fadeTime = 1f;


    

    private void Awake()
    {
        startPos = this.transform.position;
        this.animate = GetComponent<Animator>();
    }
    //Paul
    void Start()
    {
        this.player = GameObject.FindWithTag("Player").transform; // Charakter Transform finden
    }
    

    private void Update()
    {
        animate.SetFloat("Movement", Mathf.Abs(aiPath.desiredVelocity.x * 10));
        if (aiPath.desiredVelocity.x >= 0.1f && !this.faceRight)
        {
            Flip();
        } 
        else if (aiPath.desiredVelocity.x <= -0.1f && this.faceRight)
        {
            Flip();
        }
    }
   
    private void FixedUpdate()
    {
        //Check Health Status
        if(this.health <= 0)
        {
            startDeathProcess();
        }
        
        // Wenn der Charakter innerhalb der Reichweite ist und es Zeit ist, wieder zu feuern
        if (Vector3.Distance(transform.position, player.position) <= range && Time.time > nextFire  && this.shootingAllowed)
        {
            nextFire = Time.time + this.weapon.fireRate; // Zeitpunkt für nächsten Schuss setzen
            this.weapon.ShootBullet(true); //Schießen
        }
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

    public void startDeathProcess()
    {
        //Disable Gravity
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        //Disable AI
        this.gameObject.GetComponent<AIPath>().enabled = false;
        this.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        this.shootingAllowed = false;
        this.weapon.gameObject.SetActive(false);

        //Start animation
        this.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(overrideControllerStartDeath);
        this.allowArmMovement = false;
        this.animate.SetBool("ReleasePlaceholder", false);
        this.animate.Play("Enemy_Placeholder");

        StartCoroutine(fadeProcess());
    }

    private IEnumerator fadeProcess()
    {
        Color col = this.gameObject.GetComponent<Renderer>().material.color;
        for (float i = 1; i > 0; i -= fadeStrength)
        {
            col.a = i;
            this.gameObject.GetComponent<Renderer>().material.color = col;
            yield return new WaitForSeconds(fadeTime);
        }
        yield return new WaitForSeconds(fadeTime * 3);
        this.gameObject.SetActive(false);
    }

    public void ResetState()
    {
        //Enable Gravity
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        //Enable AI
        this.gameObject.GetComponent<AIPath>().enabled = true;
        this.gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        this.shootingAllowed = true;
        this.weapon.gameObject.SetActive(true);

        //Other
        this.allowArmMovement = true;
        this.health = this.maxHealth;
        this.transform.position = this.startPos;

        //Reset Animations
        ResetAnimator();

        //Rotate
        if (!this.faceRight)
        {
            Flip();
            this.faceRight = true;
        }
    }

    public void ResetAnimator()
    {
        this.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(this.overrideControllerResetOverrider);
        
        this.animate.SetFloat("Movement", 0);
        this.animate.SetFloat("Sprinting", 0);
        this.animate.SetBool("ReleasePlaceholder", false);
        this.animate.Play("Enemy_Idle");
    }
}
