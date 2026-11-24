using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlatformWave : MonoBehaviour
{
    public static readonly List<PlatformWave> All = new List<PlatformWave>();

    [SerializeField] private SpriteRenderer platformBody;

    [Tooltip("SpriteRenderer enfant (même sprite que la plateforme) avec le matériau d'overlay.")]
    [SerializeField] private SpriteRenderer overlay;

    [SerializeField] private float revealDuration = 0.4f;
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.6f;

    [Range(0f, 1f)]
    [SerializeField] private float residualOutlineAlpha = 0.5f;
    public UnityEvent onWaveHit;

    private static readonly int RevealId = Shader.PropertyToID("_Reveal");
    private static readonly int AlphaId = Shader.PropertyToID("_Alpha");

    private Collider2D col;
    private MaterialPropertyBlock block;
    private Coroutine routine;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        block = new MaterialPropertyBlock();

        SetValues(0f, 0f);

        if (overlay != null) overlay.enabled = false;
        if (platformBody != null) platformBody.enabled = false;
    }

    private void OnEnable() 
    { 
        All.Add(this); 
    }

    private void OnDisable() 
    { 
        All.Remove(this); 
    }

    public float DistanceTo(Vector2 point)
    {
        Vector2 closest = col.ClosestPoint(point);
        return Vector2.Distance(point, closest);
    }

    public void OnWaveHit()
    {
        onWaveHit?.Invoke();
        if (overlay == null) 
        {
            return;
        }

        if (routine != null) 
        {
            StopCoroutine(routine); 
        }
        routine = StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        overlay.enabled = true;
        if (platformBody != null) platformBody.enabled = true;
        for (float t = 0f; t < 1f; t += Time.deltaTime / revealDuration)
        {
            SetValues(t, 1f);
            SetBodyAlpha(t);
            yield return null;
        }

        SetValues(1f, 1f);
        SetBodyAlpha(1f);

        yield return new WaitForSeconds(holdDuration);

        for (float t = 0f; t < 1f; t += Time.deltaTime / fadeDuration)
        {
            float currentAlpha = Mathf.Lerp(1f, residualOutlineAlpha, t);
            SetValues(1f, currentAlpha);
            SetBodyAlpha(1f - t);
            yield return null;
        }

        if (platformBody != null) platformBody.enabled = false;

        if (residualOutlineAlpha > 0f)
        {
            SetValues(1f, residualOutlineAlpha);
        }
        else
        {
            SetValues(0f, 0f);
            overlay.enabled = false;
        }

        routine = null;
    }

    private void SetValues(float reveal, float alpha)
    {
        if (overlay == null) return;
        overlay.GetPropertyBlock(block);
        block.SetFloat(RevealId, reveal);
        block.SetFloat(AlphaId, alpha);
        overlay.SetPropertyBlock(block);
    }

    private void SetBodyAlpha(float alpha)
    {
        if (platformBody == null) return;
        Color c = platformBody.color;
        c.a = alpha;
        platformBody.color = c;
    }
}