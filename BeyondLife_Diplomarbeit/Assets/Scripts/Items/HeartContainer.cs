using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    public float health;
    private float startY;

    public float amplitude = 1;
    public float frequenzy = 1;

    void Start()
    {
        this.startY = this.transform.position.y;
    }

    void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x, Mathf.Sin(Time.time * frequenzy) * amplitude + this.startY, this.transform.position.z);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Take bullet damage
        if (other.gameObject.layer == LayerMask.NameToLayer("player")) 
        {
            PlayerLogic player = other.gameObject.GetComponent<PlayerLogic>();
            player.health = player.maxHealth;
            player.healthbar.handle(this.health);
            this.gameObject.SetActive(false);
        }
    }
}
