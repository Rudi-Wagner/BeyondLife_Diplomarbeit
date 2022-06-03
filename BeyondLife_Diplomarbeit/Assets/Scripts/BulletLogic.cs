using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float damage;
    public float lifeSpan;

    [Header("Other")]
    public Rigidbody2D rigidBody;
    public GameObject Bullet;

    private void Start()
    {
        this.rigidBody.velocity = this.transform.right * speed;
        Invoke(nameof(destroySelf), lifeSpan);
    }

    private void destroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls") ||
            other.gameObject.layer == LayerMask.NameToLayer("player") ||
            other.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            destroySelf();
        }
    }
}
