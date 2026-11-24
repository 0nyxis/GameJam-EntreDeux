using UnityEngine;

public class PlayerDead : MonoBehaviour
{
    [SerializeField] LayerMask targetLayerMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayerMask) != 0)
        {
            collision.GetComponent<Player>().TakeDamage(1000);
        }
    }
}