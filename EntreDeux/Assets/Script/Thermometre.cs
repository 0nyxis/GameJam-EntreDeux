using UnityEngine;

public class Thermometre : MonoBehaviour
{
    float speed = 1f;

    void Update()
    {
        Vector3 scale = transform.localScale;
        if (scale.x < 35)
        {
            float deltaScale = speed * Time.deltaTime;

            scale.x += deltaScale;
            transform.localScale = scale;

            transform.position += transform.right * (deltaScale / 2f);
        }
    }
}
