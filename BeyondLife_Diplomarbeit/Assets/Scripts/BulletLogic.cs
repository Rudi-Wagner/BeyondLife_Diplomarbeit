using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public float speed;
    public float lifeSpan;
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("walls"))
        {
            destroySelf();
        }
    }
}
