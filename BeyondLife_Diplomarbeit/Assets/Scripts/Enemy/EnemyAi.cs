using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    // Reference to the player's position
    public Transform player;

    // Reference to the enemy's navmesh agent
    public UnityEngine.AI.NavMeshAgent nav;

    // Reference to the enemy's shooting component
    public WeaponLogic shooter;

    // The maximum distance at which the enemy will start chasing the player
    public float chaseDistance = 100.0f;

    // The maximum distance at which the enemy will start shooting at the player
    public float shootDistance = 50.0f;

    // The distance at which the enemy will start running away from the player
    public float fleeDistance = 5.0f;

    // The patrol waypoints for the enemy
    public Transform[] patrolWaypoints;

    // The current index of the patrol waypoint that the enemy is heading towards
    private int waypointIndex = 0;

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance between the enemy and the player
        float distance = Vector3.Distance(transform.position, player.position);

        // If the distance between the enemy and the player is less than the chase distance
        if (distance < chaseDistance)
        {
            // Set the enemy's destination to the player's position
            //nav.SetDestination(player.position);

            // If the distance between the enemy and the player is less than the shoot distance
            if (distance < shootDistance)
            {
                // Make the enemy shoot at the player
                shooter.ShootBullet(true);
            }
            // If the distance between the enemy and the player is less than the flee distance
            else if (distance < fleeDistance)
            {
                // Make the enemy run away from the player
                //nav.SetDestination(transform.position - player.position);
            }
        }
        // If the distance between the enemy and the player is greater than the chase distance
        else
        {
            // Make the enemy patrol
            //Patrol();
        }
    }

    // Function to make the enemy patrol between the specified waypoints
    void Patrol()
    {
        // Set the enemy's destination to the current patrol waypoint
        nav.SetDestination(patrolWaypoints[waypointIndex].position);

        // Calculate the distance between the enemy and the current patrol waypoint
        float distance = Vector3.Distance(transform.position, patrolWaypoints[waypointIndex].position);

        // If the enemy has reached the current patrol waypoint
        if (distance < nav.stoppingDistance)
        {
            // Increment the waypoint index
            waypointIndex = (waypointIndex + 1) % patrolWaypoints.Length;
        }
    }
}