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
    public Vector2 collierSize { get; private set; }
    public Vector2 collierOffset { get; private set; }

    [Header("Weapons")]
    public WeaponLogic[] weapons;
    public WeaponLogic startWeapon;
    public bool shootingAllowed = true;
    public bool weaponUpdate = true;
    public float currentSelected { get; private set; }
    public WeaponLogic weapon{ get; private set; }
    public InputAction weaponSelect{ get; private set; }

    [Header("WeaponArm")]
    public GameObject WeaponPos;
    public GameObject weaponLimbSolver;
    public GameObject weaponCCDSolver;
    public GameObject weaponArmShoulder;
    public GameObject weaponArmHand;
    public float distanceFromShoulder;
    public bool allowArmMovement = true;

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

    //Animator
    [Header("Animatior")]
    public AnimatorOverrideController overrideControllerStartFlip;
    public AnimatorOverrideController overrideControllerResetOverrider;
    public Animator animate { get; private set; }

    public float nextFire  { get; private set; } = 0f;
    public bool faceRight = true;
    
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

        this.collierSize = this.boxCollider.size;
        this.collierOffset = this.boxCollider.offset;
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
                this.weapon.ShootBullet(false);
            }
        }

        //Check Health Status
        if(this.health <= 0 && !this.immortal)
        {
            this.manager.ShowDeathScreen();
            this.animate.Play("Player_Idle");
            ResetAnimator();
        }
    }

    private void LateUpdate() 
    {
        if (this.InputAllowed && this.weaponUpdate)
        {
            //Get angle from mouse position and player positiont
            Vector2 mousePos = this.look.ReadValue<Vector2>();
            Vector3 startPos = this.weaponArmShoulder.transform.position;

            //Set Position of weapon
            this.weapon.transform.position = new Vector3(this.WeaponPos.transform.position.x, this.WeaponPos.transform.position.y, -2);        
            //Caluclate the angle
            Vector3 dir = Camera.main.ScreenToWorldPoint(mousePos) - startPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //Set Rotation/Position for WeaponArm
            if (this.allowArmMovement)
            {
                //Vector between Shoulder and Hand
                //                  Direction        Distance
                Vector3 direction = dir.normalized * this.distanceFromShoulder;

                //Strecht the Vector if below the minimum
                var x = direction.x;
                var y = direction.y;
                float minLength = 1.5f;
                while (Mathf.Sqrt(x*x + y*y) < minLength)
                {
                    direction = direction * 1.05f;
                    x = direction.x;
                    y = direction.y;
                }
                //Debug.Log("x: " + x + " ,y: " + y + " ,z: " + z + "         " + Mathf.Sqrt(x*x + y*y));
                

                //Rotate WeaponArm                        Start Position
                this.weaponLimbSolver.transform.position = this.weaponArmShoulder.transform.position + direction;
                this.weaponCCDSolver.transform.position = this.weaponArmShoulder.transform.position + direction * 2f;
                Debug.DrawRay(this.weaponArmShoulder.transform.position, direction, Color.green);
            }

            //Rotate the weapon
            if (this.weapon.gameObject.name != "MeleeWeapon")
            {//Normal Rotation for WeaponArm
                this.weapon.transform.rotation = Quaternion.Euler(0, 0, angle);
                if (!this.faceRight)
                {//Add 180° to Weapon-rotation to fix pointing direction
                    this.weapon.transform.rotation = this.weapon.transform.rotation * Quaternion.Euler(180, 0, 0);
                }
            }
            else if (this.weapon.gameObject.name == "MeleeWeapon")
            {//Rotation for the melee Weapon
                this.weapon.transform.rotation = this.weaponArmHand.transform.rotation * Quaternion.Euler(0, 0, 190);

                //Fix Arm
                float x1 = -1.458f;
                float x2 = -1.066f;

                if (!this.faceRight)
                {
                    x1 = -x1;
                    x2 = -x2;
                }
                weaponLimbSolver.transform.position = this.gameObject.transform.position + new Vector3(x1, -0.638f, 0f);
                weaponCCDSolver.transform.position = this.gameObject.transform.position + new Vector3(x2, -1.358f, 0f);
                return; //Arm Movement not allowed --> leave early
            }
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
        return Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, Vector2.down, length, this.wallLayer);
    }

    public bool checkIfEnemyBelow(float length)
    {
        //Check if the player is on an enemy to allow a jump
        return Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, Vector2.down, length, this.enemyLayer);
    }

    public bool checkIfWall(float length, Vector2 direction)
    {
        return Physics2D.BoxCast(this.transform.position, new Vector2(1, 0.5f), 0f, direction, length, this.wallLayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Take bullet damage
        if (other.gameObject.layer == LayerMask.NameToLayer("bullet")) 
        {
            BulletLogic doDamageToPlayer = other.gameObject.GetComponent<BulletLogic>();

            if (doDamageToPlayer == null || doDamageToPlayer.damagePlayer)
            {
                takeBullet(other.gameObject);
            }
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
        if (this.weaponUpdate)
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

    public void ResetAnimator()
    {
        this.animate.SetFloat("Movement", 0);
        this.animate.SetFloat("Sprinting", 0);
        this.animate.SetBool("Crouching", false);
        this.animate.SetBool("Sliding", false);
        this.animate.SetBool("Jumping", false);
        this.animate.SetBool("Landing", false);
        this.animate.SetBool("Falling", false);
        this.animate.SetBool("WallJumping", false);
    }
}