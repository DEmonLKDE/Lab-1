using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class HelicopterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Fuel System")]
    public float baseFuelDrainPerSecond = 5f; 

    [Header("Boundary")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    [Header("Audio")]
    public AudioClip pickupSound;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded())
        {
            movement = Vector2.zero;
            return;
        }

        movement = ReadArrowKeys();

        if (movement.x < -0.01f) sr.flipX = true;
        else if (movement.x > 0.01f) sr.flipX = false;

        GameManager.Instance?.ConsumeFuel(baseFuelDrainPerSecond);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * moveSpeed;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    private Vector2 ReadArrowKeys()
    {
        if (Keyboard.current == null) return Vector2.zero;

        float x = 0f;
        float y = 0f;

        if (Keyboard.current.leftArrowKey.isPressed)  x -= 1f;
        if (Keyboard.current.rightArrowKey.isPressed) x += 1f;
        if (Keyboard.current.downArrowKey.isPressed)  y -= 1f;
        if (Keyboard.current.upArrowKey.isPressed)    y += 1f;

        Vector2 v = new Vector2(x, y);
        return v.sqrMagnitude > 1f ? v.normalized : v;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded()) return;

        if (other.CompareTag("Soldier"))
        {
            if (GameManager.Instance != null && GameManager.Instance.CanPickUp())
            {
                GameManager.Instance.PickUpSoldier();
                Destroy(other.gameObject);

                if (pickupSound != null)
                    audioSource.PlayOneShot(pickupSound);
            }
        }
        else if (other.CompareTag("Hospital"))
        {
            GameManager.Instance?.DropOffSoldiers();
            GameManager.Instance?.RefuelToFull();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded()) return;

        if (collision.collider.CompareTag("Tree"))
        {
            GameManager.Instance?.GameOver("GAME OVER");
            rb.linearVelocity = Vector2.zero;
        }
    }
}