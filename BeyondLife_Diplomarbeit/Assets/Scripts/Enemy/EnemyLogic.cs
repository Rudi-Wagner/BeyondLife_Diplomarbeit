using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public bool faceRight = true;

    [Header("WeaponArm")]
    public bool weaponUpdate = true;
    public GameObject WeaponPos;
    public GameObject weaponLimbSolver;
    public GameObject weaponCCDSolver;
    public GameObject weaponArmShoulder;
    public GameObject weaponArmHand;
    public float distanceFromShoulder;
    public bool allowArmMovement = true;

    //Other
    [Header("Other")]
    public WeaponLogic weapon;

    public void Flip()
    {
        //Flip the player
        this.faceRight = !this.faceRight;
        this.transform.Rotate(0f, 180f, 0f);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("bullet")) {
            destroySelf(other.gameObject);
        }
    }

    protected virtual void destroySelf(GameObject other)
    {
        //Destroy(gameObject);  //Fürs erste später wsl wieder sinnvoll
        this.gameObject.SetActive(false);
    }

    private void LateUpdate() 
    {
        if (this.weaponUpdate)
        {
            //Get angle from mouse position and player positiont
            GameObject player = GameObject.FindWithTag("Player");
            PlayerLogic comp = player.GetComponent<PlayerLogic>();
            Vector2 aimAtPos = comp.look.ReadValue<Vector2>();
            Vector3 startPos = this.weaponArmShoulder.transform.position;

            //Set Position of weapon
            this.weapon.transform.position = new Vector3(this.WeaponPos.transform.position.x, this.WeaponPos.transform.position.y, -2);     

            //Caluclate the angle
            Vector3 dir = Camera.main.ScreenToWorldPoint(aimAtPos) - startPos;
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
            if (this.weapon.gameObject.name != "MeleeWeapon" && !this.weapon.freezeRotation)
            {//Normal Rotation for Weapon
                this.weapon.transform.rotation = Quaternion.Euler(0, 0, angle);
                if (!this.faceRight)
                {//Add 180° to Weapon-rotation to fix pointing direction
                    this.weapon.transform.rotation = this.weapon.transform.rotation * Quaternion.Euler(180, 0, 0);
                }
            }
            else if (this.weapon.gameObject.name == "MeleeWeapon" && !this.weapon.freezeRotation)
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
}
