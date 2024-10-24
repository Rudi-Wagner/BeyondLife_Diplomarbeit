using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public PlayerLogic playerlogic;

    private float sprintMult = 1;
    private float BoxCastLength = 5; 
    private float nextParticleSpawn;
    private Coroutine fallingRoutine;

    private void FixedUpdate()
    {
        if (this.playerlogic.InputAllowed)
        {
            // Movement and round the Vector to the nearest Int
            this.playerlogic.moveDirection = this.playerlogic.move.ReadValue<Vector2>();
            this.playerlogic.moveDirection = new Vector2Int((int) Math.Round(this.playerlogic.moveDirection.x, 0), (int) Math.Round(this.playerlogic.moveDirection.y, 0));
            this.playerlogic.animate.SetFloat("Movement", Mathf.Abs(this.playerlogic.moveDirection.x));

            if (this.playerlogic.moveDirection.x != 0)
            {
                //Flip Player on movement
                if (this.playerlogic.moveDirection.x == 1 && !this.playerlogic.faceRight)
                {
                    this.playerlogic.Flip();
                } 
                else if (this.playerlogic.moveDirection.x == -1 && this.playerlogic.faceRight)
                {
                    this.playerlogic.Flip();
                }
                //Horizontal Movement
                this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.moveDirection.x * this.playerlogic.speed * this.sprintMult, this.playerlogic.rigidBody.velocity.y);
            
                //Check if falling
                if (!this.playerlogic.checkIfGrounded(this.BoxCastLength) && !this.playerlogic.isSliding)
                {
                    StartCoroutine("fallingAnimation");
                }
                else
                {//Spawn Walking particle effect
                    dealWithParticles();
                }
            }
            else
            {
                if (!this.playerlogic.alreadyDashed)
                {
                    //If no input is given player slowly stops moving
                    this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.rigidBody.velocity.x / 1.5f, this.playerlogic.rigidBody.velocity.y);
                }
            }

            //Check if sprinting
            if (this.playerlogic.sprint.ReadValue<float>() == 1 && !this.playerlogic.animate.GetBool("Crouching"))
            {
                this.sprintMult = this.playerlogic.sprintMultValue;
                this.playerlogic.animate.speed = 2;
                this.playerlogic.animate.SetFloat("Sprinting", Mathf.Abs(this.playerlogic.moveDirection.x * 2));
                this.playerlogic.animate.SetFloat("Movement", 0);
            }
            else if(this.playerlogic.sprint.ReadValue<float>() == 0)
            {
                this.sprintMult = 1;
                this.playerlogic.animate.speed = 1.35f; //Normal Walk-animation speed
                this.playerlogic.animate.SetFloat("Sprinting", 0);
            }

            ////Spezial Movement
            if(this.playerlogic.moveDirection.y >= 0.5f)
            {//Start JumpLogic
                doJump();
            }

            if(this.playerlogic.moveDirection.y <= -0.5f && this.playerlogic.sprint.ReadValue<float>() == 0)
            {//Start CrouchLogic
                doCrouch();
                this.playerlogic.animate.SetBool("Crouching", true);
            }
            else if(!(this.playerlogic.checkIfWall(this.BoxCastLength, Vector2.up)))
            {//Reset to StandLogic
                this.playerlogic.animate.SetBool("Crouching", false);
            }


            if(this.playerlogic.moveDirection.y <= -0.5f && this.playerlogic.sprint.ReadValue<float>() == 1 
            && Mathf.Abs(this.playerlogic.moveDirection.x) > 0 && !this.playerlogic.animate.GetBool("Crouching")
            && this.playerlogic.checkIfGrounded(this.BoxCastLength))
            {//Start SlideLogic
                doSlide();
            }
            else
            {
                this.playerlogic.animate.SetBool("SlidingEnd", false);
            }

            if (this.playerlogic.dash.ReadValue<float>() == 1)
            {//Dash
                doDash();
            } else if (this.playerlogic.checkIfGrounded(this.BoxCastLength))
            {//Reset dash on grounded
                this.playerlogic.alreadyDashed = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls") && other.gameObject.CompareTag("wallJumpEnabled"))
        {//Start WallJumpLogic
            doWallJump();
        }
    }

    private void dealWithParticles()
    {
        if (!this.playerlogic.particlesActive || this.nextParticleSpawn > Time.time)
        {
            return;
        }

        this.nextParticleSpawn = Time.time + UnityEngine.Random.Range(this.playerlogic.minDelay, this.playerlogic.maxDelay);

        GameObject particles = Instantiate(this.playerlogic.particleEffect, this.playerlogic.particleSpawn.transform.position, this.playerlogic.particleSpawn.transform.rotation);
    }

    public void doWallJump()
    {
        //Walljump
        /*Only do the Walljump if:
            1. The player is not touching the ground
            2. The player is touching a wall
            3. The player presses the Jump Button
            4. The delay for the next Walljump is over
            5. There is no wall above the player
        */
        
        if (!this.playerlogic.checkIfGrounded(this.BoxCastLength) && this.playerlogic.moveDirection.y >= 0.5f  
        && Time.time > this.playerlogic.nextWallJump && !this.playerlogic.checkIfWall(this.BoxCastLength, Vector2.up))
        {
            this.playerlogic.animate.SetBool("WallJumping", true);
            dealWithParticles();
            //Disable player control for 0.2 seconds
            this.playerlogic.InputAllowed = false;
            Invoke(nameof(this.ActivateInput), 0.2f);

            //Set the WallJumpDelay to disable another WallJump in a given time
            this.playerlogic.nextWallJump = Time.time + this.playerlogic.wallJumpDelay;

            //Move the player in the opposit diretion from the wall
            float jumpDirection = -1; //set left
            if (this.playerlogic.checkIfWall(2, Vector2.right))
            {
                jumpDirection = 1;    //set right
            }

            //Do a Walljump
            this.playerlogic.rigidBody.velocity = new Vector2(jumpDirection * this.playerlogic.speed * -2, this.playerlogic.jumpStrength);
            this.playerlogic.Flip();
        }
    }

    public void ActivateInput()
    {
        this.playerlogic.InputAllowed = true;
        this.playerlogic.animate.SetBool("WallJumping", false);
    }

    public void doJump()
    {
        if(this.playerlogic.checkIfGrounded(this.BoxCastLength) && !(this.playerlogic.checkIfWall(this.BoxCastLength, Vector2.up)))
        {//Vertical Movement (only jumping)
            /*Only do a normal jump when:
                1. The player presses the jump Button
                2. The player is touching the ground or an enemy*/

            StartCoroutine(jumpAnimation());
            //Do normal jump
            this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.moveDirection.x * this.playerlogic.speed, this.playerlogic.jumpStrength);    
            //Set WallJumpDelay so that the player doesn't do a walljump immidiatly after the normal jump
            this.playerlogic.nextWallJump = Time.time + this.playerlogic.wallJumpDelay;     
        }
    }

    private IEnumerator jumpAnimation()
    {
        float delayInBetween = 0.2f;
        this.playerlogic.animate.SetBool("Jumping", true);
        yield return new WaitForSeconds(delayInBetween);
        this.playerlogic.animate.SetBool("Jumping", false);
        yield return new WaitForSeconds(delayInBetween);
        while(!this.playerlogic.checkIfGrounded(this.BoxCastLength))
        {
            yield return null;
        }
        this.playerlogic.animate.SetBool("Landing", true);
        yield return new WaitForSeconds(delayInBetween);
        this.playerlogic.animate.SetBool("Landing", false);
    }

    private IEnumerator fallingAnimation()
    {
        float delayInBetween = 0.2f;
        this.playerlogic.animate.SetBool("Falling", true);
        while(!this.playerlogic.checkIfGrounded(this.BoxCastLength))
        {
            yield return null;
        }
        this.playerlogic.animate.SetBool("Landing", true);
        dealWithParticles();
        yield return new WaitForSeconds(delayInBetween);

        //Reset Variables
        this.playerlogic.animate.SetBool("Landing", false);
        this.playerlogic.animate.SetBool("Falling", false);
        this.playerlogic.allowArmMovement = true;
        this.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(this.playerlogic.overrideControllerResetOverrider);
        

        //Set Player face direction
        if (this.playerlogic.faceRight)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
    }

    public void doDash()
    {
        if(!this.playerlogic.checkIfGrounded(this.BoxCastLength) && !this.playerlogic.alreadyDashed)
        {//Dash
            this.playerlogic.isDashing = true;
            this.playerlogic.InputAllowed = false;
            this.playerlogic.alreadyDashed = true;
            //Caluclate dash-direction
            Vector2 mousePos = this.playerlogic.look.ReadValue<Vector2>();
            var dir = Camera.main.ScreenToWorldPoint(mousePos) - this.playerlogic.transform.position;
            dir *= this.playerlogic.dashLength;
            //Do normal dash
            this.playerlogic.rigidBody.velocity = Vector3.ClampMagnitude(dir, 100f);    
            Invoke(nameof(this.ActivateInput), 0.2f);  

            if (this.playerlogic.faceRight)
            {
                this.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(this.playerlogic.overrideControllerStartFlipRight);
            }
            else
            {
                this.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(this.playerlogic.overrideControllerStartFlipLeft);
            }
            
            this.playerlogic.allowArmMovement = false;
            this.playerlogic.animate.SetBool("ReleasePlaceholder", true);
            this.playerlogic.animate.Play("Player_Placeholder");
            this.playerlogic.isDashing = false;
            StartCoroutine("fallingAnimation");
        }
    }

    public void doCrouch()
    {//Crouching
        //Crouch
        this.sprintMult = this.playerlogic.sneakMultValue;
    }

    public void doSlide()
    {//Sliding
        if (!this.playerlogic.isSliding && Time.time > this.playerlogic.nextSlide)
        {
            StopCoroutine("fallingAnimation");

            this.playerlogic.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(this.playerlogic.overrideControllerStartSlide);
            this.playerlogic.animate.Play("Player_SlidingStart");
            this.playerlogic.animate.SetBool("SlidingEnd", false);
            this.playerlogic.animate.SetBool("Falling", false);
            
            this.playerlogic.nextSlide = Time.time + this.playerlogic.slideDelay;
            //Set state
            this.playerlogic.isSliding = true;
            this.playerlogic.allowArmMovement = false;
            this.playerlogic.weapon.freezeRotation = true;
            float slideDirection = -1; //set left
            if (this.playerlogic.faceRight)
            {
                slideDirection = 1;    //set right
            }
            this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.slideSpeed * slideDirection, 0);
            this.playerlogic.InputAllowed = false;
            StartCoroutine(slidingMid());
        }
        else
        {
            this.playerlogic.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(this.playerlogic.overrideControllerResetOverrider);
        }
    }

    private IEnumerator slidingMid()
    {
        bool slidingFlag = true;
        float endTime = Time.time + this.playerlogic.slideDuration;
        while(slidingFlag)
        {
            Debug.DrawRay(this.playerlogic.gameObject.transform.position, Vector2.up, Color.green);
            if(!(this.playerlogic.checkIfWall(2, Vector2.up)) && Time.time > endTime)
            {
                slidingFlag = false;
            }
            yield return null;
        }

        stopSliding();
    }

    private void stopSliding()
    {
        this.playerlogic.isSliding = false;
        this.playerlogic.InputAllowed = true;
        this.playerlogic.animate.SetBool("SlidingEnd", true);
        this.playerlogic.allowArmMovement = true;
        this.playerlogic.weapon.freezeRotation = false;
        this.playerlogic.gameObject.GetComponent<AnimatorOverrider>().SetAnimations(this.playerlogic.overrideControllerResetOverrider);
    }
}
