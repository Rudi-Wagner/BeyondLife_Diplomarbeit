using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class PlayerLogic : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public WeaponLogic weapon;

    public new Collider2D collider { get; private set; }
    public BoxCollider2D boxCollider { get; private set; }
    public LayerMask wallLayer;
    public LayerMask enemyLayer;
    public Rigidbody2D rigidBody;

    public float health;
    public float speed = 5f;
    public float sprintMult;
    public float jumpStrength;
    private float nextFire = 0f;
    private bool faceRight = true;
    
    public Vector2 moveDirection { get; private set; }
    public PlayerControls inputControls;
    private InputAction move;
    private InputAction fire;
    private InputAction sprint;

    public AnimatedSprite normal;
    public AnimatedSprite death;

    

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.collider = GetComponent<Collider2D>();
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.inputControls = new PlayerControls();
    }

    private void Update()
    {
        //Shooting
        if(this.fire.ReadValue<float>() == 1 && Time.time > this.nextFire)
        {
            nextFire = Time.time + this.weapon.fireRate;
            this.weapon.ShootBullet();
        }

        //Check Health Status
        if(this.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReadMouseInput(InputAction.CallbackContext context)
    {//Weapon rotating
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector2 objectPosition = (Vector2) Camera.main.WorldToScreenPoint(this.weapon.transform.position);
        Vector2 direction = (mousePosition - objectPosition).normalized;
 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        this.weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void FixedUpdate()
    {
        this.moveDirection = move.ReadValue<Vector2>();
        
        //Check if sprinting
        if (this.sprint.ReadValue<float>() == 1)
        {
            this.sprintMult = 1.5f;
        }
        else
        {
            this.sprintMult = 1;
        }

        //Horizontal Movement
        this.rigidBody.velocity = new Vector2(this.moveDirection.x * speed * sprintMult, this.rigidBody.velocity.y);

        //Face direction
        if (this.rigidBody.velocity.x > 0 && !this.faceRight)
        {
            Flip();
        } else if (this.rigidBody.velocity.x < 0 && this.faceRight)
        {
            Flip();
        }

        if(this.moveDirection.y >= 0.5f && (checkIfGrounded() || checkIfEnemyBelow()))
        {
            //Vertical Movement (Gravity is always on --> only jumping)
            this.rigidBody.velocity = new Vector2(this.moveDirection.x * speed, jumpStrength);
        } else if(this.moveDirection.y <= -0.5f)
        {
            //Crouch
        }
    }

    private void Flip()
    {
        this.faceRight = !this.faceRight;
        this.transform.Rotate(0f, 180f, 0f);
    }

    private bool checkIfGrounded()
    {
        return Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, Vector2.down, 2f, this.wallLayer);
    }

    private bool checkIfEnemyBelow()
    {
        return Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, Vector2.down, 2f, this.enemyLayer);
    }

    public void ResetState()
    {
        //Reset pacman to a "normal" state
        this.enabled = true;
        this.spriteRenderer.enabled = true;
        this.collider.enabled = true;
        this.gameObject.SetActive(true);
        this.gameObject.transform.position = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("bullet")) {
            destroySelf(other.gameObject);
        }
    }

    private void destroySelf(GameObject other)
    {
        if (this.health >= 0)
        {
            BulletLogic bullet = other.gameObject.GetComponent<BulletLogic>();
            this.health -= bullet.damage;
        }
    }

    public void deathAnimation()
    {
        //Play death animation after collision with a ghost
        this.collider.enabled = false;
        this.transform.rotation = Quaternion.AngleAxis(90 * Mathf.Rad2Deg, Vector3.forward);
        this.normal.enabled = false;
        this.normal.doLoop = false;
        this.death.enabled = true;
        this.death.Restart();
        this.death.doLoop = false;
        Invoke(nameof(die), 1.0f);
    }

    private void die()
    {
        //Disable player controls
        this.gameObject.SetActive(false);
        this.collider.enabled = true;
        this.normal.enabled = true;
        this.normal.doLoop = true;
        this.death.enabled = false;
        this.death.doLoop = false;
    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b) 
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private void OnEnable()
    {
        this.move = this.inputControls.Player.Move;
        this.fire = this.inputControls.Player.Fire;
        this.sprint = this.inputControls.Player.Sprint;
        this.move.Enable();
        this.fire.Enable();
        this.sprint.Enable();
    }

    private void OnDisable()
    {
        this.move.Disable();
        this.fire.Disable();
    }
}