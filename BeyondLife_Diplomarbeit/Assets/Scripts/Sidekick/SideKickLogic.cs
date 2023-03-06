using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideKickLogic : MonoBehaviour
{
    public GameObject particleEffect;

    public float maxHeight;
    public float minHeight;
    public float hoverSpeed;


    void Update()
    {
        //Hovering
        float hoverHeight = (maxHeight + minHeight) / 2.0f;
        float hoverRange = maxHeight - minHeight;
        float hoverSpeed = 10.0f;
        
        //this.transform.position = Vector3.up * hoverHeight + Mathf.cos(Time.time * hoverSpeed) * hoverRange;
    }
}
