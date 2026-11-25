using UnityEngine;

public class EndDoor : MonoBehaviour
{
    private BoxCollider2D collider;

    public void ActiveEndDoor()
    {
        collider.enabled = true;
    }

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.enabled = false;
    }

    private void Update()
    {
        if (collider.enabled)
        {
            if (collider.IsTouchingLayers())
            {
                GameManager.Instance.ChangeSceneTo("Win");
            }
        }
    }
}
