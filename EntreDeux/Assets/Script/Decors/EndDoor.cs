using UnityEngine;

public class EndDoor : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool activated = false;
    private BoxCollider2D collider;

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = activated;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
        {

        }
    }
}
