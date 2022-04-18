using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    public bool doLoop = true;
    public Sprite[] sprites = new Sprite[0];
    public float delay = 0.25f;

    public int currentFrame { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(changeSprite), delay, delay);
    }

    private void changeSprite()
    {
        if (!this.spriteRenderer.enabled) 
        {
            return;
        }

        this.currentFrame++;

        //Set currentFrame back to 0 to loop the animation
        if (this.currentFrame >= this.sprites.Length && this.doLoop) 
        {
            this.currentFrame = 0;
        }

        //Set the actual sprite of the object
        if (this.currentFrame >= 0 && this.currentFrame < this.sprites.Length) 
        {
            this.spriteRenderer.sprite = this.sprites[this.currentFrame];
        }
    }

    public void Restart()
    {
        //Set currentFrame to -1 to start the next time at 0
        this.currentFrame = -1;
        changeSprite();
    }

}
