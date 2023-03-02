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

    [Header("Bullet Particel")]
    public GameObject Bullet;
    public GameObject Explosion;
    public GameObject smokeTrail;
    public GameObject aoeDamageSphere;

    [Header("Other")]
    public Rigidbody2D rigidBody;
    private ScreenShaker camShake;

    private void Start()
    {
        camShake = Camera.main.GetComponent<ScreenShaker>();
        StartCoroutine(destroySelfAfterDelay());
        this.aoeDamageSphere.GetComponent<AOEDamage>().damage = 2;
    }

    private IEnumerator destroySelfAfterDelay()
    {
        yield return new WaitForSeconds(lifeSpan);
        camShake.start = true;
        StartCoroutine(destroySelf());
    }

    private IEnumerator destroySelf()
    {
        //Explode
        
        this.Explosion.SetActive(true);
        this.smokeTrail.SetActive(false);
        this.rigidBody.velocity = Vector2.zero;
        Instantiate(this.aoeDamageSphere, this.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(explosionLifeSpan);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls") ||
            other.gameObject.layer == LayerMask.NameToLayer("player") ||
            other.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            camShake = Camera.main.GetComponent<ScreenShaker>();
            camShake.start = true;
            StopAllCoroutines();
            StartCoroutine(destroySelf());
        }
    }
}
