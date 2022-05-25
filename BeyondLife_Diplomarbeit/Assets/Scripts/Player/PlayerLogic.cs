using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class PlayerLogic : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite standing;
    public Sprite crouching;
    public Sprite jumping;
    public Sprite sliding;
    public WeaponLogic weapon;
    public BoxCollider2D boxCollider { get; private set; }
    public LayerMask wallLayer;
    public LayerMask enemyLayer;
    public Rigidbody2D rigidBody;
    
    //Player Values
    public float health;

    //Movement Values
    public bool InputAllowed = true;
    public float speed = 5f;

    //Sliding
    public bool isSliding = false;
    public float slideSpeed = 10f;
    public float slideDuration = 1f;
    [HideInInspector]
    public float nextSlide;
    public float slideDelay;

    //Sprinting & Sneaking
    public float sprintMultValue { get; private set; } = 2f;
    public float sneakMultValue { get; private set; } = 0.5f;

    //Jumping
    public float wallJumpDelay;
    [HideInInspector]
    public float nextWallJump = 0f;
    public float jumpStrength;

    public float nextFire  { get; private set; } = 0f;
    public bool faceRight { get; private set; }  = true;
    
    public Vector2 moveDirection;
    public PlayerControls inputControls;
    public InputAction move{ get; private set; }
    public InputAction fire{ get; private set; }
    public InputAction sprint{ get; private set; }
    public InputAction look{ get; private set; }

    public AnimatedSprite normal;
    public AnimatedSprite death;

    

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.inputControls = new PlayerControls();
    }

    private void Update()
    {
        //Shooting
        if(this.fire.ReadValue<float>() == 1 && Time.time > this.nextFire)
        {
            this.nextFire = Time.time + this.weapon.fireRate;
            this.weapon.ShootBullet();
        }

        //Check Health Status
        if(this.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate () 
    {
        //Get angle from mouse position and player positiont
        Vector2 mousePos = this.look.ReadValue<Vector2>();
        Vector3 weaponPos = this.weapon.transform.position;

        //Caluclate the angle
        var dir = Camera.main.ScreenToWorldPoint(mousePos) - weaponPos;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //Lock angle
        //Dont allow the weapon to move above a certain angle (to prevent shooting yourself)

        if (this.faceRight)
        {
            if (angle >= 45.0f)
            {
                angle = 45.0f;
            } else if (angle <= -45.0f)
            {
                angle = -45.0f;
            }
        }
        else
        {
            if (angle <= 135.0f && angle >= 0.0f)  //Needs the extra 'angle >= 0.0f' because the angle isn't measured in 360°, it is in 180° and -180°
            {
                angle = 135.0f;
            } else if (angle >= -135.0f && angle <= 0.0f)
            {
                angle = -135.0f;
            }
        }
        

        //Rotate the wapon
        this.weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Flip()
    {
        //Flip the player
        this.faceRight = !this.faceRight;
        this.transform.Rotate(0f, 180f, 0f);
    }

    public bool checkIfGrounded(float length)
    {
        //Check if the player is on the ground
        return Physics2D.BoxCast(this.transform.position, new Vector2(3, 0.5f), 0f, Vector2.down, length, this.wallLayer);
    }

    public bool checkIfEnemyBelow(float length)
    {
        //Check if the player is on an enemy to allow a jump
        return Physics2D.BoxCast(this.transform.position, new Vector2(3, 0.5f), 0f, Vector2.down, length, this.enemyLayer);
    }

    public bool checkIfWall(float length, Vector2 direction)
    {
        return Physics2D.BoxCast(this.transform.position, new Vector2(3, 0.5f), 0f, direction, length, this.wallLayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Take bullet damage
        if (other.gameObject.layer == LayerMask.NameToLayer("bullet")) {
            takeBullet(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Death layer
        //Kill the player if he touches this layer
        if(other.gameObject.layer == LayerMask.NameToLayer("walls") && other.gameObject.CompareTag("deathLayer"))
        {
            this.health = -999f;
        }
    }    

    private void takeBullet(GameObject other)
    {
        if (this.health >= 0)
        {
            BulletLogic bullet = other.gameObject.GetComponent<BulletLogic>();
            this.health -= bullet.damage;
        }
    }

    private void OnEnable()
    {
        this.move = this.inputControls.Player.Move;
        this.fire = this.inputControls.Player.Fire;
        this.sprint = this.inputControls.Player.Sprint;
        this.look = this.inputControls.Player.Look;
        this.move.Enable();
        this.fire.Enable();
        this.sprint.Enable();
        this.look.Enable();
    }

    private void OnDisable()
    {
        this.move.Disable();
        this.fire.Disable();
        this.sprint.Disable();
        this.look.Disable();
    }
}