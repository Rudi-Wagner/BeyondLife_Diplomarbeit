using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementLogic : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedMult = 1.0f;
    public Vector2 initialDirection;
    public LayerMask wallLayer;

    public new Rigidbody2D rigidbody { get; private set; }
    public GameManager manager;
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startPos { get; private set; }

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.startPos = this.transform.position;
    }

    private void Start()
    {
        ResetMovement();
    }

    public void ResetMovement()
    {
        this.direction = this.initialDirection;
        this.nextDirection = Vector2.zero;
        this.transform.position = this.startPos;
        this.rigidbody.isKinematic = false;
        this.enabled = true;
    }

    private void Update()
    {
        if (this.nextDirection != Vector2.zero) {
            SetDirection(this.nextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction * this.speed * this.speedMult * Time.fixedDeltaTime;

        this.rigidbody.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !checkMove(direction))
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
        }
        else
        {
            this.nextDirection = direction;
        }
    }

    public bool checkMove(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, this.wallLayer);
        return hit.collider != null;
    }

}
