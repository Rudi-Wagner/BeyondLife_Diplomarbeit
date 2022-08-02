using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLogic : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float damage;
    public float lifeSpan;
    public float explosionLifeSpan;

    [Header("Other")]
    public Rigidbody2D rigidBody;
    public GameObject Bullet;
    public GameObject Explosion;
    public GameObject smokeTrail;

    private void Start()
    {
        //this.rigidBody.velocity = this.transform.right * speed;
        Invoke(nameof(destroySelf), lifeSpan);
    }

    private IEnumerator destroySelf()
    {
        //Explode
        this.Explosion.SetActive(true);
        this.smokeTrail.SetActive(false);
        this.rigidBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(explosionLifeSpan);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls") ||
            other.gameObject.layer == LayerMask.NameToLayer("player") ||
            other.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            StartCoroutine(destroySelf());
        }
    }
}
