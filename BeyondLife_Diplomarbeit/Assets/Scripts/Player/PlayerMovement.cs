using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerLogic playerlogic;

    public string state;

    private void FixedUpdate()
    {
        if (this.playerlogic.InputAllowed)
        {
            // Movement und so 
            this.playerlogic.moveDirection = this.playerlogic.move.ReadValue<Vector2>();
            
            //Check if sprinting
            if (this.playerlogic.sprint.ReadValue<float>() == 1)
            {
                this.playerlogic.sprintMult = 2f;
            }
            else if(this.playerlogic.sprint.ReadValue<float>() == 0 && this.playerlogic.moveDirection.y >= 0f)
            {
                this.playerlogic.sprintMult = 1;
            }

            //Horizontal Movement
            this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.moveDirection.x * this.playerlogic.speed * this.playerlogic.sprintMult, this.playerlogic.rigidBody.velocity.y);

            //Face direction
            if (this.playerlogic.rigidBody.velocity.x > 0 && !this.playerlogic.faceRight)
            {
                this.playerlogic.Flip();
            } else if (this.playerlogic.rigidBody.velocity.x < 0 && this.playerlogic.faceRight)
            {
                this.playerlogic.Flip();
            }

            //Spezial Movement
            if(this.state == "STANDING")
            {
                if(this.playerlogic.moveDirection.y >= 0.5f && (this.playerlogic.checkIfGrounded() || this.playerlogic.checkIfEnemyBelow()))
                {//Vertical Movement (only jumping)
                    /*Only do a normal jump when:
                        1. The player presses the jump Button
                        2. The player is touching the ground or an enemy
                    */
                    //Set state
                    this.state = "JUMP";
                    //Change to Jump Sprite
                    this.playerlogic.spriteRenderer.sprite = this.playerlogic.jumping;
                    //Do normal jump
                    this.playerlogic.rigidBody.velocity = new Vector2(this.playerlogic.moveDirection.x * this.playerlogic.speed, this.playerlogic.jumpStrength);    
                    //Set WallJumpDelay so that the player doesn't do a walljump immidiatly after the normal jump
                    this.playerlogic.nextWallJump = Time.time + this.playerlogic.wallJumpDelay;     
                } else if(this.playerlogic.moveDirection.y <= -0.5f)
                {//Crouching
                    //Set state
                    this.state = "CROUCH";
                    //Change to crouch sprite
                    this.playerlogic.spriteRenderer.sprite = this.playerlogic.crouching;
                    //Crouch
                    this.playerlogic.sprintMult = 0.5f;
                    this.playerlogic.boxCollider.size = new Vector2(3, 1.9f);
                    this.playerlogic.boxCollider.offset = new Vector2(0, -1);
                }
            }
            else
            {//Check if a reset to "STANDING" is allowed
                if(this.state == "CROUCH" && (bool) !Physics2D.BoxCast(this.playerlogic.transform.position, new Vector2(1, 0.5f), 0f, Vector2.up, 3f, this.playerlogic.wallLayer))
                {
                    //Set state
                    this.state = "STANDING";
                    this.playerlogic.spriteRenderer.sprite = this.playerlogic.standing;
                    this.playerlogic.boxCollider.size = new Vector2(3, 3.8f);
                    this.playerlogic.boxCollider.offset = new Vector2(0, -0.1f);
                } else if(this.state == "JUMP" && this.playerlogic.checkIfGrounded()
                && this.playerlogic.moveDirection.y <= -0.5f)
                {
                    //Set state
                    this.state = "STANDING";
                    this.playerlogic.spriteRenderer.sprite = this.playerlogic.standing;
                    this.playerlogic.boxCollider.size = new Vector2(3, 3.8f);
                    this.playerlogic.boxCollider.offset = new Vector2(0, -0.1f);
                }
            }
        }
    }
}
