
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private GameObject explosionPrefab;

    [Header("Collision Settings")]
    [SerializeField] private float collisionCooldown = 1.5f;

    private bool isDead = false;
    private float nextCollisionTime = 0f;

    private Driving driving;
    private Rigidbody2D rb;

    private void Awake()
    {
        driving = GetComponent<Driving>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // Bridge barrier: truck explodes immediately
        if (collision.gameObject.CompareTag("Barrier"))
        {
            isDead = true;
            lifeManager.InstantDeath();

            ExplodeTruck();
            return;
        }

        // Other vehicle: destroy that vehicle and lose one life
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (Time.time < nextCollisionTime) return;

            nextCollisionTime = Time.time + collisionCooldown;

            ExplodeVehicle(collision.gameObject);

            lifeManager.LoseLife();

            // If the last life was lost, explode the truck too
            if (lifeManager.IsGameOver())
            {
                isDead = true;
                ExplodeTruck();
            }
        }
    }

    private void ExplodeVehicle(GameObject vehicle)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, vehicle.transform.position, Quaternion.identity);
        }

        Destroy(vehicle);
    }

    private void ExplodeTruck()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (driving != null)
            driving.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        SpriteRenderer truckSprite = GetComponent<SpriteRenderer>();

        if (truckSprite != null)
            truckSprite.enabled = false;

        Collider2D truckCollider = GetComponent<Collider2D>();

        if (truckCollider != null)
            truckCollider.enabled = false;
    }
}
