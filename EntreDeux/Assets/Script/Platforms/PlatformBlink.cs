using System.Collections;
using UnityEngine;

public class PlatformBlink : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [Range(0.5f,5f)]
    [SerializeField] private float timerOn = 5f;
    [Range(0.5f, 5f)]
    [SerializeField] private float timerOff = 1f;

    private Collider2D collider2D;
    private SpriteRenderer renderer;
    private bool wait = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!wait && collider2D.IsTouchingLayers(playerLayerMask))
        {
            StartCoroutine(Wait(timerOn, Dispawn()));
        }
    }

    private IEnumerator Wait(float _secondToWait, IEnumerator _function)
    {
        wait = true;

        yield return new WaitForSeconds(_secondToWait);

        StartCoroutine(_function);
    }

    private IEnumerator Dispawn()
    {
        collider2D.enabled = false;
        renderer.enabled = false;

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(Wait(timerOff - 0.3f, Respawn()));
    }

    private IEnumerator Respawn()
    {
        renderer.enabled = true;
        collider2D.enabled = true;

        yield return new WaitForSeconds(0.3f);

        wait = false;
    }
}
