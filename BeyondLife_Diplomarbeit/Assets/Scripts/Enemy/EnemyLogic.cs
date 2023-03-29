using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyLogic : MonoBehaviour
{
    [Header("Weapon")]
    public WeaponLogic weapon;
    public bool weaponUpdate = true;
    public bool shootingAllowed = true;


    [Header("WeaponArm")]
    public GameObject WeaponPos;
    public GameObject weaponLimbSolver;
    public GameObject weaponCCDSolver;
    public GameObject weaponArmShoulder;
    public GameObject weaponArmHand;
    public float distanceFromShoulder;
    public bool allowArmMovement = true;
    public GameObject patrolAimPoint;
    public bool disableAiming;

    //Animation
    [Header("Animation")]
    public AnimatorOverrideController overrideControllerResetOverrider;
    public AnimatorOverrideController overrideControllerStartDeath;

    //Other
    [Header("Other")]
    public bool faceRight = true;
    public AIPath aiPath;

    private void OnEnable()
    {
        this.weapon.gameObject.SetActive(true);
    }

    public void Flip()
    {
        //Flip the player
        this.faceRight = !this.faceRight;
        this.transform.Rotate(0f, 180f, 0f);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("bullet"))
        {
            destroySelf(other.gameObject);
        }
    }

    protected virtual void destroySelf(GameObject other){}


    private void LateUpdate() 
    {
        if (this.weaponUpdate)
        {
            this.weapon.gameObject.SetActive(true);

            //Get angle from mouse position and player positiont
            GameObject player = GameObject.FindWithTag("Player");
            Vector3 aimAtPos = player.transform.position;

            if(this.disableAiming)
            {
                aimAtPos = this.patrolAimPoint.transform.position;
            }

            Vector3 startPos = this.weaponArmShoulder.transform.position;

            //Set Position of weapon
            this.weapon.transform.position = new Vector3(this.WeaponPos.transform.position.x, this.WeaponPos.transform.position.y, -2);     

            //Caluclate the angle
            Vector3 dir = aimAtPos - startPos;
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
                Debug.DrawRay(this.weaponArmShoulder.transform.position, direction, Color.red);
                Debug.DrawRay(this.weaponArmShoulder.transform.position + direction, direction, Color.green);
            }

            //Rotate the weapon
            if (!this.weapon.freezeRotation)
            {
                this.weapon.transform.rotation = Quaternion.Euler(0, 0, angle);
                if (!this.faceRight)
                {//Add 180° to Weapon-rotation to fix pointing direction
                    this.weapon.transform.rotation = this.weapon.transform.rotation * Quaternion.Euler(180, 0, 0);
                }
            }
        }
    }
}
