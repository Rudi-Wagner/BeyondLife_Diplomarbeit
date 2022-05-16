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
    
    public bool InputAllowed = true;
    public bool isSliding = false;
    public float health;
    public float speed = 5f;
    public float slideSpeed = 10f;
    public float slideDuration = 1f;
    public float sprintMultValue { get; private set; } = 2f;
    public float sneakMultValue { get; private set; } = 0.5f;
    public float wallJumpDelay;
    public float nextWallJump = 0f;
    public float jumpStrength;
    public float nextFire  { get; private set; } = 0f;
    public bool faceRight { get; private set; }  = true;
    
    public Vector2 moveDirection;
    public PlayerControls inputControls;
    public InputAction move{ get; private set; }
    public InputAction fire{ get; private set; }
    public InputAction sprint{ get; private set; }

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

    public bool checkIfWallOnTop(float length)
    {
        return Physics2D.BoxCast(this.transform.position, new Vector2(3, 0.5f), 0f, Vector2.up, length, this.wallLayer);
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

    public void ReadMouseInput(InputAction.CallbackContext context)
    {//Weapon rotating
        
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