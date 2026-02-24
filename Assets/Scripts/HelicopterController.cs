using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    public float speed = 5f;
    public AudioClip pickupSound;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameEnded()) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        movement = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Soldier"))
        {
            if (GameManager.Instance.CanPickUp())
            {
                GameManager.Instance.PickUp();
                Destroy(other.gameObject);

                if (pickupSound != null)
                    audioSource.PlayOneShot(pickupSound);
            }
        }

        if (other.CompareTag("Hospital"))
        {
            GameManager.Instance.DropOff();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Tree"))
        {
            GameManager.Instance.GameOver();
            rb.linearVelocity = Vector2.zero;
        }
    }
}
