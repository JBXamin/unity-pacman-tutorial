using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Ghost : MonoBehaviour
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer flashing;

    public Movement movement { get; private set; }
    public AnimatedSprite animatedSprite { get; private set; }
    public Vector3 startingPosition { get; private set; }
    public bool vulnerable { get; private set; }

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.animatedSprite = GetComponent<AnimatedSprite>();
        this.startingPosition = this.transform.position;
    }

    public void ResetState()
    {
        this.transform.position = this.startingPosition;
        this.movement.SetDirection(this.movement.initialDirection);
        this.gameObject.SetActive(true);

        StopBlueMode();
    }

    public void StartBlueMode(float duration)
    {
        this.vulnerable = true;
        this.body.enabled = false;
        this.eyes.enabled = false;
        this.flashing.enabled = false;
        this.blue.enabled = true;

        CancelInvoke(nameof(StartFlashing));
        CancelInvoke(nameof(StopBlueMode));

        Invoke(nameof(StartFlashing), duration * 0.5f);
        Invoke(nameof(StopBlueMode), duration);
    }

    public void StopBlueMode()
    {
        CancelInvoke(nameof(StartFlashing));
        CancelInvoke(nameof(StopBlueMode));

        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.flashing.enabled = false;
        this.vulnerable = false;
    }

    private void StartFlashing()
    {
        this.blue.enabled = false;
        this.flashing.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            FindObjectOfType<GameManager>().GhostTouched(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null) {
            this.movement.SetNextDirection(node.RandomAvailableDirection());
        }
    }

}
