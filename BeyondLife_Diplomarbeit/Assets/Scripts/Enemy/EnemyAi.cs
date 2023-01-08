using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float followDistance;
    public float attackDistance;
    public float retreatDistance;
    public bool faceRight;
    private bool followingPlayer;
    private bool attackingPlayer;
    private Vector2 moveDirection;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public int currentPatrolPoint;
    public float patrolWaitTime;
    private float patrolTimer;
    private bool patrolling;

    [Header("References")]
    public Rigidbody2D rb;
    public WeaponLogic weapon;
    public Transform player;

    void Start()
    {
        // Initialize movement variables
        followingPlayer = false;
        attackingPlayer = false;
        moveDirection = Vector2.zero;

        // Initialize patrol variables
        patrolTimer = 0f;
        patrolling = true;
    }

    void Update()
    {
        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If the enemy is within follow distance, start following the player
        if (distanceToPlayer <= followDistance)
        {
            followingPlayer = true;
        }
        else
        {
            followingPlayer = false;
        }

        // If the enemy is within attack distance, start attacking the player
        if (distanceToPlayer <= attackDistance)
        {
            attackingPlayer = true;
        }
        else
        {
            attackingPlayer = false;
        }

        // If the enemy is following the player, face the player and move towards them
        if (followingPlayer)
        {
            // Face the player
            if (player.position.x > transform.position.x)
            {
                faceRight = true;
            }
            else
            {
                faceRight = false;
            }
            Flip();

            // Move towards the player
            moveDirection = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

            // If the enemy is within retreat distance, run away from the player
            if (distanceToPlayer <= retreatDistance)
            {
                moveDirection *= -1;
                rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
            }
        }
        // If the enemy is not following the player, patrol
        else
        {
            Patrol();
        }

        // If the enemy is attacking the player, shoot at them
        if (attackingPlayer)
        {
            weapon.ShootBullet(true);
        }
    }
    void Patrol()
{
    // If the enemy has reached their patrol point, stop and wait
    if (Vector2.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 0.1f)
    {
        patrolling = false;
        patrolTimer = patrolWaitTime;
    }
    // If the enemy is waiting at a patrol point, count down the timer
    else if (!patrolling)
    {
        patrolTimer -= Time.deltaTime;

        // If the timer has reached zero, start moving to the next patrol point
        if (patrolTimer <= 0f)
        {
            patrolling = true;

            // Increment the current patrol point index, or reset it if it has reached the end of the array
            if (currentPatrolPoint + 1 < patrolPoints.Length)
            {
                currentPatrolPoint++;
            }
            else
            {
                currentPatrolPoint = 0;
            }
        }
    }
    // If the enemy is patrolling, move towards the current patrol point
    else
    {
        // Face the patrol point
        if (patrolPoints[currentPatrolPoint].position.x > transform.position.x)
        {
            faceRight = true;
        }
        else
        {
            faceRight = false;
        }
        Flip();

        // Move towards the patrol point
        moveDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }
}

void Flip()
{
    if (faceRight)
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
    else
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
    }
}
}