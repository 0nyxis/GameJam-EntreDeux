using UnityEngine;

public class RespawnBed : MonoBehaviour
{
    public Transform respawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Jump"))
        {
            collision.transform.position = respawn.position;
        }
    }
}
