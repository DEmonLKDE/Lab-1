using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class HelicopterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Audio")]
    public AudioClip pickupSound;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded())
        {
            movement = Vector2.zero;
            return;
        }

        movement = ReadArrowKeys();
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

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded()) return;

        if (collision.collider.CompareTag("Tree"))
        {
            GameManager.Instance?.GameOver();
            rb.linearVelocity = Vector2.zero;
        }
    }
}