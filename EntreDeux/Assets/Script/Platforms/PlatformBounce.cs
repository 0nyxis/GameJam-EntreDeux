using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlatformBounce : MonoBehaviour
{
    [Header("Bounce Data")]
    [SerializeField] float bounceForce = 18f;
    [SerializeField] float topTolerance = 0.1f; // marge pour vérifier que le joueur est au-dessus au kazou

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent<Player>(out _)) return;

        Rigidbody2D playerRb = collision.rigidbody;
        if (playerRb == null) return;

        // Le joueur doit être au-dessus de la plateforme (pas de rebond par le côté ou le dessous)
        float playerBottom = collision.collider.bounds.min.y;
        float platformTop = GetComponent<Collider2D>().bounds.max.y;
        if (playerBottom < platformTop - topTolerance) return;

        // Le joueur ne doit pas être en train de monter
        if (playerRb.linearVelocity.y > 0.1f) return;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound("jump_bed");
        }
    }
}