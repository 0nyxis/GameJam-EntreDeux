using UnityEngine;

public class Thermometre : MonoBehaviour
{
    float speed = 1f;
    [SerializeField] public BoxCollider2D colliderTermo;
    [SerializeField] public Transform spriteTermo;

    void Update()
    {
        Vector3 size = colliderTermo.size;
        Vector3 scale = spriteTermo.localScale;

        if (scale.x < 35)
        {
            float deltaSize = speed * Time.deltaTime;

            size.x += deltaSize;
            colliderTermo.size = size;

            Vector2 offset = colliderTermo.offset;
            offset.x += deltaSize / 2f;
            colliderTermo.offset = offset;

            scale.x += deltaSize;
            spriteTermo.localScale = scale;
            spriteTermo.localPosition += Vector3.right * (deltaSize / 2f);
        }
    }
}
