using UnityEngine;
using UnityEngine.InputSystem;

public class Thermometre : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxWidth = 21f;

    [SerializeField] private SpriteRenderer spriteRendererTermo;
    [SerializeField] private BoxCollider2D colliderTermo;

    public bool IsActivated { get; private set; } = false;

    public void Activate()
    {
        IsActivated = true;
    }

    void Update()
    {
        if (!IsActivated) return;

        float currentWidth = spriteRendererTermo.size.x;

        if (currentWidth < maxWidth)
        {
            float deltaSize = speed * Time.deltaTime;
            if (currentWidth + deltaSize > maxWidth)
            {
                deltaSize = maxWidth - currentWidth;
            }

            Vector2 newSize = spriteRendererTermo.size;
            newSize.x += deltaSize;
            spriteRendererTermo.size = newSize;

            Vector2 colSize = colliderTermo.size;
            colSize.x += deltaSize;
            colliderTermo.size = colSize;

            spriteRendererTermo.transform.localPosition += new Vector3(deltaSize / 2f, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.currentThermometre = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) && player.currentThermometre == this)
        {
            player.currentThermometre = null;
        }
    }
}
