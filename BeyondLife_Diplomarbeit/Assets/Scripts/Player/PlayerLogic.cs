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
    public bool InputAllowed = true;

    public float health;
    public float speed = 5f;
    public float sprintMult;
    public float wallJumpDelay;
    private float nextWallJump = 0f;
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
            this.nextFire = Time.time + this.weapon.fireRate;
            this.weapon.ShootBullet();
        }

        //Check Health Status
        if(this.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (this.InputAllowed)
        {
            this.moveDirection = move.ReadValue<Vector2>();
            
            //Check if sprinting
            if (this.sprint.ReadValue<float>() == 1)
            {
                this.sprintMult = 2f;
            }
            else if(this.sprint.ReadValue<float>() == 0 && this.moveDirection.y >= 0f)
            {
                this.sprintMult = 1;
            }

            //Horizontal Movement
            this.rigidBody.velocity = new Vector2(this.moveDirection.x * this.speed * this.sprintMult, this.rigidBody.velocity.y);

            //Face direction
            if (this.rigidBody.velocity.x > 0 && !this.faceRight)
            {
                Flip();
            } else if (this.rigidBody.velocity.x < 0 && this.faceRight)
            {
                Flip();
            }

            if(this.moveDirection.y >= 0.5f && (checkIfGrounded() || checkIfEnemyBelow()))
            {//Vertical Movement (only jumping)
                /*Only do a normal jump when:
                    1. The player presses the jump Button
                    2. The player is touching the ground or an enemy
                */
                //Do normal jump
                this.rigidBody.velocity = new Vector2(this.moveDirection.x * this.speed, this.jumpStrength);    
                //Set WallJumpDelay so that the player doesn't do a walljump immidiatly after the normal jump
                this.nextWallJump = Time.time + this.wallJumpDelay;     
            } else if(this.moveDirection.y <= -0.5f)
            {
                //Crouch
                this.sprintMult = 0.5f;
            }
        }
        
    }

    private void Flip()
    {
        //Flip the player
        this.faceRight = !this.faceRight;
        this.transform.Rotate(0f, 180f, 0f);
    }

    private bool checkIfGrounded()
    {
        //Check if the player is on the ground
        return Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, Vector2.down, 3f, this.wallLayer);
    }

    private bool checkIfEnemyBelow()
    {
        //Check if the player is on an enemy to allow a jump
        return Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, Vector2.down, 3f, this.enemyLayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Take bullet damage
        if (other.gameObject.layer == LayerMask.NameToLayer("bullet")) {
            takeBullet(other.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        //Walljump
        /*Only do the Walljump if:
            1. The player is not touching the ground
            2. The player is touching a wall
            3. The player presses the Jump Button
            4. The delay for the next Walljump is over
            5. There is no wall above the player
        */
        if (!checkIfGrounded() && other.gameObject.layer == LayerMask.NameToLayer("walls") && other.gameObject.CompareTag("wallJumpEnabled")
        && this.moveDirection.y >= 0.5f  && Time.time > this.nextWallJump
        && (bool) !Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, Vector2.up, 3f, this.wallLayer))
        {
            //Disable player control for 0.2 seconds
            this.InputAllowed = false;
            Invoke(nameof(ActivateInput), 0.2f);

            //Set the WallJumpDelay to disable another WallJump in a given time
            this.nextWallJump = Time.time + this.wallJumpDelay;

            //Move the player in the opposit diretion from the wall
            float jumpDirection = -1; //set left
            if (this.faceRight)
            {
                jumpDirection = 1;    //set right
            }

            //Do a Walljump
            this.rigidBody.velocity = new Vector2(jumpDirection * this.speed * -2, this.jumpStrength);
            Flip();
            return;
        }

        //Death layer
        //Kill the player if he touches this layer
        if(other.gameObject.layer == LayerMask.NameToLayer("walls") && other.gameObject.CompareTag("deathLayer"))
        {
            this.health = -999f;
        }
    }    

    public void ActivateInput()
    {
        this.InputAllowed = true;
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