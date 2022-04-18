using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class PlayerLogic : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public new Collider2D collider { get; private set; }
    public MovementLogic movement { get; private set; }

    public AnimatedSprite normal;
    public AnimatedSprite death;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.collider = GetComponent<Collider2D>();
        this.movement = GetComponent<MovementLogic>();
    }

    private void Update()
    {
        
    }

    public void ResetState()
    {
        //Reset pacman to a "normal" state
        this.movement.SetDirection(Vector2.zero);
        this.enabled = true;
        this.spriteRenderer.enabled = true;
        this.collider.enabled = true;
        this.gameObject.SetActive(true);
        this.gameObject.transform.position = new Vector3(0, -8.5f, -5);
    }

    public void deathAnimation()
    {
        //Play death animation after collision with a ghost
        this.collider.enabled = false;
        this.movement.SetDirection(Vector2.zero);
        this.transform.rotation = Quaternion.AngleAxis(90 * Mathf.Rad2Deg, Vector3.forward);
        this.normal.enabled = false;
        this.normal.doLoop = false;
        this.death.enabled = true;
        this.death.Restart();
        this.death.doLoop = false;
        this.movement.enabled = false;
        Invoke(nameof(die), 1.0f);
    }

    private void die()
    {
        //Disable player controls
        this.gameObject.SetActive(false);
        this.collider.enabled = true;
        this.normal.enabled = true;
        this.normal.doLoop = true;
        this.death.enabled = false;
        this.death.doLoop = false;
        this.movement.enabled = true;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }
}