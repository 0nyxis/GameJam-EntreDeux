using System.Collections;
using UnityEngine;

public class PlatformBlink : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float timeBeforeColliderOff = 1f;
    [SerializeField] private float timeBeforeColliderOn = 5f;

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
            StartCoroutine(Wait(timeBeforeColliderOff, Dispawn()));
        }
    }

    private IEnumerator Wait(float _secondToWait, IEnumerator _function)
    {
        wait = true;

        yield return new WaitForSeconds(_secondToWait);

        StartCoroutine(_function);
    }

    private IEnumerator Wait(float _secondToWait)
    {
        wait = true;

        yield return new WaitForSeconds(_secondToWait);

        wait = false;
    }

    private IEnumerator Dispawn()
    {
        collider2D.enabled = false;

        int disparition = 0;

        while (disparition < 100)
        {
            disparition++;
            Debug.Log(disparition.ToString());
            yield return null;
        }

        renderer.enabled = false;

        StartCoroutine(Wait(timeBeforeColliderOn, Respawn()));
    }

    private IEnumerator Respawn()
    {
        renderer.enabled = true;

        int apparition = 0;

        while (apparition < 100)
        {
            apparition++;
            Debug.Log(apparition.ToString());
            yield return null;
        }

        collider2D.enabled = true;

        wait = false;
    }
}
