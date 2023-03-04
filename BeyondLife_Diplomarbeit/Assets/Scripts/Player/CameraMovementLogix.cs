using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementLogix : MonoBehaviour
{
    public GameObject player;
    public float smoothness = 8f;
    private Vector3 velocity = Vector3.zero;
    public Vector3 offset;

    void Start () 
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    void LateUpdate () 
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        if(player != null)
        {
            Vector3 targetPos = player.transform.position + offset;
            //Vector3 smothedPos = Vector3.Lerp(this.transform.position, targetPos, smoothness * Time.deltaTime);
            Vector3 smothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothness);
            transform.position = smothedPos;
        }
    }
}
