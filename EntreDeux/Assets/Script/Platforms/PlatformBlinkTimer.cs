using System.Collections;
using UnityEngine;

public class PlatformTimer : MonoBehaviour
{
    [SerializeField] private float invisibleTime = 3f;
    [SerializeField] private float visibleTime = 3f;
    [SerializeField] private float firstDispawn = 3f;

    private float animationTime = 1f;
    //private bool started = false;


    private Collider2D collider2D;
    private SpriteRenderer renderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        collider2D = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();

        StartCoroutine(Wait(firstDispawn, Dispawn()));
    }

    // Update is called once per frame
    private void Update()
    {
        //if (!started)
        //{
        //    float CurrentTime = Time.time;
        //    if (CurrentTime > firstDispawn)
        //    {
        //        StartCoroutine(Dispawn());
        //    }
        //}
    }

    private IEnumerator Wait(float _secondToWait, IEnumerator _function)
    {
        yield return new WaitForSeconds(_secondToWait);

        StartCoroutine(_function);
    }

    private IEnumerator Wait(float _secondToWait)
    {
        yield return new WaitForSeconds(_secondToWait);
    }

    private IEnumerator Dispawn()
    {
        collider2D.enabled = false;
        renderer.enabled = false;

        yield return new WaitForSeconds(animationTime);

        StartCoroutine(Wait(invisibleTime - animationTime, Respawn()));
    }

    private IEnumerator Respawn()
    {
        renderer.enabled = true;
        collider2D.enabled = true;

        yield return new WaitForSeconds(animationTime);

        StartCoroutine(Wait(visibleTime - animationTime, Dispawn()));
    }
}
