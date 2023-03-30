using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideKickLogic : MonoBehaviour
{
    public GameObject particleEffect;

    public float amplitude = 1;
    public float frequenzy = 1;

    private float startY;

    void Start()
    {
        this.startY = this.transform.position.y;
    }

    void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x, Mathf.Sin(Time.time * frequenzy) * amplitude + this.startY, this.transform.position.z);
    }
}
