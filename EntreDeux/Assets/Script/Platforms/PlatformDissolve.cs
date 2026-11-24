using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformDissolve : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float timeBeforeColliderOff = 1f;
    [SerializeField] private float timeForAnimation = 2f;
    [SerializeField] private Light2D light;

    private Collider2D collider2D;
    private bool wait = false;
    private bool dissolving = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (collider2D.IsTouchingLayers(playerLayerMask) && !wait)
        {
            StartCoroutine(Wait());
        }

        if (dissolving)
        {
            // Appeler le Shader pour dissolution
        }
    }

    private IEnumerator Wait()
    {
        wait = true;

        yield return new WaitForSeconds(timeBeforeColliderOff);

        StartCoroutine(DestroyItself());
    }

    private IEnumerator DestroyItself()
    {
        collider2D.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        light.enabled = false;

        dissolving = true;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound("rockfall");
        }

        yield return new WaitForSeconds(timeForAnimation);
    }
}
