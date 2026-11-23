using UnityEngine;

public class PlatformInvert : MonoBehaviour
{
    private Player playerS;

    [Header("Collider Automatique")]
    [SerializeField] private BoxCollider2D effectZoneCollider;

    [Header("Collider à renseigner")]
    [SerializeField] private BoxCollider2D platformCollider;

    private bool playerInside = false;

    private void Start()
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = (platformCollider.size * new Vector2(4f, 8f)) + new Vector2(0.1f, 0.1f);
        collider.offset = new Vector2(0, (collider.size.y / 2) - (platformCollider.size.y / 2));
        collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out _)) return;

        if (playerS == null)
        {
            playerS = collision.transform.GetComponent<Player>();
        }
        else
        {
            // Deux joueur ?? Pourquoi ?
        }

        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out _)) return;

        if (playerS != null && playerS.gameObject == collision.gameObject)
        {
            playerS.InverseControl(false);
            playerInside = false;
            playerS = null;
        }
        else
        {
            // Mauvais joueur, pourquoi ???
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent<Player>(out _)) return;

        if (playerInside)
        {
            playerS.InverseControl(true);
        }
        else
        {
            // Il touche la platform de dehors la platform ????
        }
    }

    private void Update()
    {
        if (playerS != null && playerInside && playerS._isInverted && !GetComponentInChildren<Player>())
        {
            playerS.InverseControl(false);
        }
    }
}
