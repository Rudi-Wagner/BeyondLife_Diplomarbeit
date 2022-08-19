using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class PlayerLogic : MonoBehaviour
{
    [Header("Player")]
    public GameManager manager;
    public PlayerControls inputControls;

    public Animator animate { get; private set; }

    [Header("Weapons")]
    public WeaponLogic[] weapons;
    public WeaponLogic startWeapon;
    public bool shootingAllowed = true;
    public bool weaponSwitchingAllowed = true;
    public float currentSelected { get; private set; }
    public WeaponLogic weapon{ get; private set; }
    public InputAction weaponSelect{ get; private set; }
    public GameObject WeaponPos;
    public GameObject weaponArm;

    [Header("Other")]
    public LayerMask wallLayer;
    public LayerMask enemyLayer;
    public Rigidbody2D rigidBody;
    public BoxCollider2D boxCollider { get; private set; }
    
    //Player Values
    [Header("Health")]
    public float health;
    public float maxHealth;
    public bool immortal;

    //Movement Values
    [Header("Movement")]
    public bool InputAllowed = true;
    public float speed = 5f;

    //Sliding
    [Header("Sliding")]
    public bool isSliding = false;
    public float slideSpeed = 10f;
    public float slideDuration = 1f;
    [HideInInspector] public float nextSlide;
    public float slideDelay;

    //Sprinting
    public float sprintMultValue { get; private set; } = 2f;

    //Sneaking
    public float sneakMultValue { get; private set; } = 0.5f;

    //Jumping
    [Header("Jumping")]
    public float wallJumpDelay;
    [HideInInspector] public float nextWallJump = 0f;
    public float jumpStrength;

    //Dash
    [Header("Dashing")]
    public bool alreadyDashed = false;
    public float dashLength;

    public float nextFire  { get; private set; } = 0f;
    public bool faceRight { get; private set; }  = true;
    
    //Input & MoveDirection
    [Header("Direction")]
    public Vector2 moveDirection;
    public InputAction move{ get; private set; }
    public InputAction fire{ get; private set; }
    public InputAction sprint{ get; private set; }
    public InputAction look{ get; private set; }
    public InputAction dash{ get; private set; }

    private void Awake()
    {
        this.animate = GetComponent<Animator>();
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.inputControls = new PlayerControls();

        this.weapon = this.startWeapon;
        this.weapon.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (this.InputAllowed && this.shootingAllowed)
        {
            this.weapon.gameObject.SetActive(true);

            //Check for weapon switch ('-1' --> keine Vorgabe)
            SwitchWeapon(-1);

            //Shooting
            if(this.fire.ReadValue<float>() == 1 && Time.time > this.nextFire)
            {
                this.nextFire = Time.time + this.weapon.fireRate;
                this.weapon.ShootBullet();
            }
        }

        //Check Health Status
        if(this.health <= 0 && !this.immortal)
        {
            this.manager.ShowDeathScreen();
            //Destroy(gameObject);
        }
    }

    private void LateUpdate () 
    {
        if (this.InputAllowed)
        {
            //Set Position of weapon
            this.weapon.transform.position = new Vector3(this.WeaponPos.transform.position.x, this.WeaponPos.transform.position.y, -2);
            this.weapon.transform.rotation = this.WeaponPos.transform.rotation;

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
            

            //Rotate the weapon and arm
            this.weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            this.weaponArm.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
    }

    public void Flip()
    {
        //Flip the player
        this.faceRight = !this.faceRight;
        this.transform.Rotate(0f, 180f, 0f);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
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

    public void SwitchWeapon(int weaponType)
    {
        if (this.weaponSwitchingAllowed)
        {
            if (this.weaponSelect.ReadValue<float>() != 0 || weaponType != -1)
            {
                /*
                    0: Melee
                    1: Pistol
                    2: Rifle
                    3: Shotgun
                    4: Rocket Launcher
                */

                //Read value
                float weaponSelection = this.weaponSelect.ReadValue<float>();
                if (weaponType != -1)
                {
                    weaponSelection = -1f;
                }
                
                //Deaktivate all weapons
                foreach (Transform t in this.GetComponentsInChildren<Transform>())
                {
                    if (t.CompareTag("weapon")) 
                    {
                        t.gameObject.SetActive(false);
                    }
                }

                //Activate new weapon
                if (weaponSelection == 1f || weaponType == 1)
                {
                    this.weapon = this.weapons[0];
                    Transform weapon = this.gameObject.transform.Find(this.weapons[0].name);
                    weapon.gameObject.SetActive(true);
                } else if (weaponSelection == 2f || weaponType == 2)
                {
                    this.weapon = this.weapons[1];
                    Transform weapon = this.gameObject.transform.Find(this.weapons[1].name);
                    weapon.gameObject.SetActive(true);
                } else if (weaponSelection == 3f || weaponType == 3)
                {
                    this.weapon = this.weapons[2];
                    Transform weapon = this.gameObject.transform.Find(this.weapons[2].name);
                    weapon.gameObject.SetActive(true);
                } else if (weaponSelection == 4f || weaponType == 4)
                {
                    this.weapon = this.weapons[3];
                    Transform weapon = this.gameObject.transform.Find(this.weapons[3].name);
                    weapon.gameObject.SetActive(true);
                } else if (weaponSelection == 5f || weaponType == 5)
                {
                    this.weapon = this.weapons[4];
                    Transform weapon = this.gameObject.transform.Find(this.weapons[4].name);
                    weapon.gameObject.SetActive(true);
                }

                this.currentSelected = weaponSelection;

                //Set delay
                this.nextFire = Time.time + this.weapon.fireRate;
            }
        }
    }

    private void OnEnable()
    {
        this.move = this.inputControls.Player.Move;
        this.fire = this.inputControls.Player.Fire;
        this.sprint = this.inputControls.Player.Sprint;
        this.look = this.inputControls.Player.Look;
        this.dash = this.inputControls.Player.Dash;
        this.weaponSelect = this.inputControls.Toolbar.WeaponSelect;
        this.move.Enable();
        this.fire.Enable();
        this.sprint.Enable();
        this.look.Enable();
        this.dash.Enable();
        this.weaponSelect.Enable();
    }

    public void enablePlayerControls()
    {
        this.InputAllowed = true;
        this.shootingAllowed = true;
    }

    private void OnDisable()
    {
        this.move.Disable();
        this.fire.Disable();
        this.sprint.Disable();
        this.look.Disable();
        this.dash.Disable();
        this.weaponSelect.Disable();
    }

    public void ResetState()
    {
        this.gameObject.transform.position = this.manager.playerSpawn;
        this.InputAllowed = true;
        this.health = this.maxHealth;
    }
}