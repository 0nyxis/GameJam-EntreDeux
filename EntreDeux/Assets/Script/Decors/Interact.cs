using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [Header("Shockwave")]
    [SerializeField] private Material leShaderGraph;
    [SerializeField] private float duration = 1f;

    [Tooltip("Rayon (en unités monde) atteint par l'onde quand _Progress = 1. À régler pour coller au visuel.")]
    [SerializeField] private float maxWorldRadius = 20f;

    private static readonly int FocalPointId = Shader.PropertyToID("_FocalPoint");
    private static readonly int ProgressId = Shader.PropertyToID("_Progress");

    private readonly HashSet<PlatformWave> hitPlatforms = new HashSet<PlatformWave>();

    private bool canInteract = true;
    public bool _canInteract => canInteract;

    // Pour le gizmo
    private Vector2 debugCenter;
    private float debugRadius;

    void Start()
    {
        if (leShaderGraph == null)
        {
            leShaderGraph = Resources.Load<Material>("SG_Shockwave");
        }
    }

    public IEnumerator Boom(Vector2 worldPos, Vector2 viewportPos)
    {
        canInteract = false;
        hitPlatforms.Clear();
        debugCenter = worldPos;

        leShaderGraph.SetVector(FocalPointId, viewportPos);

        float t = 0f;

        while (t < 1f)
        {
            leShaderGraph.SetFloat(ProgressId, t);
            debugRadius = t * maxWorldRadius;

            CheckPlatforms(worldPos, debugRadius);

            t += Time.deltaTime / duration;
            yield return null;
        }

        leShaderGraph.SetFloat(ProgressId, 1f);
        canInteract = true;
    }

    private void CheckPlatforms(Vector2 center, float radius)
    {
        var platforms = PlatformWave.All;
        for (int i = 0; i < platforms.Count; i++)
        {
            PlatformWave p = platforms[i];
            if (hitPlatforms.Contains(p))
            {
                continue;
            }

            // Distance entre le centre de l'onde et le point le plus proche de la plateforme
            if (p.DistanceTo(center) <= radius)
            {
                hitPlatforms.Add(p);
                p.OnWaveHit();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (canInteract)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(debugCenter, debugRadius);
    }
}