using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public PlayerLogic playerlogic;

    private float sprintMult = 1;

    private void FixedUpdate()
    {
        if (this.playerlogic.InputAllowed)
        {
            //Horizontal Movement
            this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.moveDirection.x * this.playerlogic.speed * this.sprintMult, this.playerlogic.rigidBody.velocity.y);

            // Movement and round the Vector to the nearest Int
            this.playerlogic.moveDirection = this.playerlogic.move.ReadValue<Vector2>();
            this.playerlogic.moveDirection = new Vector2Int((int) Math.Round(this.playerlogic.moveDirection.x, 0), (int) Math.Round(this.playerlogic.moveDirection.y, 0));

            //Check if sprinting
            if (this.playerlogic.sprint.ReadValue<float>() == 1)
            {
                this.sprintMult = this.playerlogic.sprintMultValue;
            }
            else if(this.playerlogic.sprint.ReadValue<float>() == 0)
            {
                this.sprintMult = 1;
            }

            //Face direction
            if (this.playerlogic.rigidBody.velocity.x > 0 && !this.playerlogic.faceRight)
            {
                this.playerlogic.Flip();
            } else if (this.playerlogic.rigidBody.velocity.x < 0 && this.playerlogic.faceRight)
            {
                this.playerlogic.Flip();
            }

            //Spezial Movement
            if(this.playerlogic.moveDirection.y >= 0.5f)
            {//Start JumpLogic
                doJump();
            }

            if(this.playerlogic.moveDirection.y <= -0.5f && this.playerlogic.sprint.ReadValue<float>() == 0)
            {//Start CrouchLogic
            Debug.Log("crouch");
                doCrouch();
            }
            else if(!(this.playerlogic.checkIfWallOnTop(1)))
            {//Reset to StandLogic
                doStand();
            }

            if(this.playerlogic.moveDirection.y <= -0.5f && this.playerlogic.sprint.ReadValue<float>() == 1)
            {//Start SlideLogic
            Debug.Log("slide");
                doSlide();
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
        
        if (!this.playerlogic.checkIfGrounded(3) && this.playerlogic.moveDirection.y >= 0.5f  
        && Time.time > this.playerlogic.nextWallJump && !this.playerlogic.checkIfWallOnTop(2))
        {
            //Disable player control for 0.2 seconds
            this.playerlogic.InputAllowed = false;
            Invoke(nameof(this.ActivateInput), 0.2f);

            //Set the WallJumpDelay to disable another WallJump in a given time
            this.playerlogic.nextWallJump = Time.time + this.playerlogic.wallJumpDelay;

            //Move the player in the opposit diretion from the wall
            float jumpDirection = -1; //set left
            if (this.playerlogic.faceRight)
            {
                jumpDirection = 1;    //set right
            }

            //Do a Walljump
            this.playerlogic.rigidBody.velocity = new Vector2(jumpDirection * this.playerlogic.speed * -2, this.playerlogic.jumpStrength);
            this.playerlogic.Flip();
            return;
        }
    }

    public void ActivateInput()
    {
        this.playerlogic.InputAllowed = true;
    }

    public void doJump()
    {
        if((this.playerlogic.checkIfGrounded(2) || this.playerlogic.checkIfEnemyBelow(2)) && !(this.playerlogic.checkIfWallOnTop(1)))
        {//Vertical Movement (only jumping)
            /*Only do a normal jump when:
                1. The player presses the jump Button
                2. The player is touching the ground or an enemy*/

            //Change to Jump Sprite
            this.playerlogic.spriteRenderer.sprite = this.playerlogic.jumping;
            //Do normal jump
            this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.moveDirection.x * this.playerlogic.speed, this.playerlogic.jumpStrength);    
            //Set WallJumpDelay so that the player doesn't do a walljump immidiatly after the normal jump
            this.playerlogic.nextWallJump = Time.time + this.playerlogic.wallJumpDelay;     
        }
    }

    public void doCrouch()
    {//Crouching
        //Change to crouch sprite
        this.playerlogic.spriteRenderer.sprite = this.playerlogic.crouching;
        //Crouch
        this.sprintMult = this.playerlogic.sneakMultValue;
        this.playerlogic.boxCollider.size = new Vector2(3, 1.9f);
    }

    public void doSlide()
    {//Sliding
        if (!this.playerlogic.isSliding)
        {
            //Set state
            this.playerlogic.isSliding = true;
            this.playerlogic.InputAllowed = false;
            //Change to crouch sprite
            this.playerlogic.spriteRenderer.sprite = this.playerlogic.sliding;
            //Slide
            float slideDirection = -1; //set left
            if (this.playerlogic.faceRight)
            {
                slideDirection = 1;    //set right
            }
            this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.slideSpeed * slideDirection, 0);
            this.playerlogic.boxCollider.size = new Vector2(3, 1.9f);
            Invoke(nameof(stopSliding), this.playerlogic.slideDuration);
        }
    }

    private void stopSliding()
    {
        this.playerlogic.isSliding = false;
        this.playerlogic.InputAllowed = true;
        doStand();
    }

    public void doStand()
    {
        this.playerlogic.spriteRenderer.sprite = this.playerlogic.standing;
        this.playerlogic.boxCollider.size = new Vector2(3, 3.8f);
        this.playerlogic.boxCollider.offset = new Vector2(0, -0.1f);
    }
}
