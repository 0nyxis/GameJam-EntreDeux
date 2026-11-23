using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ligne d'électrocardiogramme qui suit des points (Empty)
/// À placer sur un GameObject avec un LineRenderer obligatoirement.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ECGPulseLine : MonoBehaviour
{
    [Header("Chemin")]
    [Tooltip("Empty GameObjects dans l'ordre du niveau.")]
    public Transform[] waypoints;
    [Tooltip("Coché = courbe lissée (Catmull-Rom). Décoché = lignes droites entre les points.")] // Merci internet et les maths
    public bool curved = true;
    [Range(4, 40)] public int samplesPerSegment = 16;
    [Tooltip("Normale du plan de jeu. (0,0,1) pour un jeu 2D / side-scroller, (0,1,0) pour une vue du dessus.")]
    public Vector3 planeNormal = Vector3.forward;
    [Tooltip("Rafraîchit le chemin chaque frame (si les waypoints bougent).")]
    public bool autoRebuildPath = false;

    [Header("Battement")]
    [Tooltip("Vitesse de propagation du battement (unités / seconde).")]
    public float speed = 6f;
    [Tooltip("Temps entre deux battements (secondes).")]
    public float beatInterval = 1.2f;
    [Tooltip("Longueur (en unités) d'un battement complet : P-QRS-T.")]
    public float beatLength = 3f;
    [Tooltip("Hauteur du pic R (unités).")]
    public float amplitude = 0.6f;
    [Tooltip("Distance entre deux points de la ligne. Plus petit = plus précis, plus coûteux.")]
    public float pointSpacing = 0.03f;
    public int maxPoints = 8000;

    [Header("Rendu")]
    public float lineWidth = 0.04f;
    public Color color = Color.white;
    [Tooltip("Valeur > 1 pour dépasser le seuil du Bloom (HDR).")]
    public float intensity = 2f;

    LineRenderer lr;
    MaterialPropertyBlock block;
    readonly List<Vector3> path = new List<Vector3>();
    readonly List<Vector3> pathNormals = new List<Vector3>();
    float totalLength;
    Vector3[] positions;
    float clock;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        block = new MaterialPropertyBlock();
        lr.useWorldSpace = true;
        lr.numCornerVertices = 2;
        lr.numCapVertices = 2;
    }

    void Start()
    {
        BuildPath();
        ApplyVisuals();
    }

    void OnValidate()
    {
        speed = Mathf.Max(0f, speed);
        beatInterval = Mathf.Max(0.05f, beatInterval);
        beatLength = Mathf.Max(0.1f, beatLength);
        pointSpacing = Mathf.Max(0.005f, pointSpacing);
        if (lr != null) ApplyVisuals();
    }

    void Update()
    {
        if (autoRebuildPath) BuildPath();
        if (totalLength <= 0f) return;

        clock += Time.deltaTime;
        UpdateLine();
    }

    // ---------- API publique ----------
    public void SetSpeed(float newSpeed) => speed = Mathf.Max(0f, newSpeed);
    public void SetBeatInterval(float seconds) => beatInterval = Mathf.Max(0.05f, seconds);

    // ---------- Construction du chemin ----------
    public void BuildPath()
    {
        path.Clear();
        pathNormals.Clear();
        totalLength = 0f;

        if (waypoints == null || waypoints.Length < 2)
        {
            return;
        }

        var pts = new List<Vector3>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                waypoints[i].name = i.ToString();
                pts.Add(waypoints[i].position);
            }
        }
        if (pts.Count < 2)
        {
            return;
        }


        var raw = new List<Vector3>();
        if (!curved)
        {
            raw.AddRange(pts);
        }
        else
        {
            for (int i = 0; i < pts.Count - 1; i++)
            {
                Vector3 p0 = pts[Mathf.Max(i - 1, 0)];
                Vector3 p1 = pts[i];
                Vector3 p2 = pts[i + 1];
                Vector3 p3 = pts[Mathf.Min(i + 2, pts.Count - 1)];

                for (int s = 0; s < samplesPerSegment; s++)
                {
                    raw.Add(CatmullRom(p0, p1, p2, p3, s / (float)samplesPerSegment));
                }
            }
            raw.Add(pts[pts.Count - 1]);
        }


        float spacing = Mathf.Max(pointSpacing, 0.005f);
        float rawLen = 0f;
        for (int i = 1; i < raw.Count; i++)
        {
            rawLen += Vector3.Distance(raw[i - 1], raw[i]);
        }
        if (rawLen / spacing > maxPoints)
        {
            spacing = rawLen / maxPoints;
        }

        path.Add(raw[0]);
        float carry = 0f;
        for (int i = 1; i < raw.Count; i++)
        {
            Vector3 a = raw[i - 1], b = raw[i];
            float segLen = Vector3.Distance(a, b);
            if (segLen < 1e-6f) continue;
            float d = spacing - carry;
            while (d <= segLen)
            {
                path.Add(Vector3.Lerp(a, b, d / segLen));
                d += spacing;
            }
            carry = segLen - (d - spacing);
        }
        if ((path[path.Count - 1] - raw[raw.Count - 1]).sqrMagnitude > 1e-8f)
        {
            path.Add(raw[raw.Count - 1]);
        }

        totalLength = (path.Count - 1) * spacing;
        pointSpacing = spacing;

        // 3) Normales (direction du déplacement vertical du tracé)
        Vector3 n = planeNormal.sqrMagnitude > 0f ? planeNormal.normalized : Vector3.forward;
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 prev = path[Mathf.Max(i - 1, 0)];
            Vector3 next = path[Mathf.Min(i + 1, path.Count - 1)];
            Vector3 tangent = (next - prev).normalized;
            Vector3 side = Vector3.Cross(n, tangent);
            if (side.sqrMagnitude < 1e-6f)
            {
                side = Vector3.up;
            }
            pathNormals.Add(side.normalized);
        }

        positions = new Vector3[path.Count];
        lr.positionCount = path.Count;
    }

    static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) // Aussecour hein
    {
        float t2 = t * t, t3 = t2 * t;
        return 0.5f * ((2f * p1) +
                       (-p0 + p2) * t +
                       (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                       (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
    }

    // ---------- Mise à jour de la ligne ----------
    void UpdateLine()
    {
        if (positions == null || positions.Length != path.Count)
        {
            return;
        }

        float spacingBetweenBeats = Mathf.Max(beatInterval * speed, beatLength);
        float head = clock * speed;

        for (int i = 0; i < path.Count; i++)
        {
            float s = i * pointSpacing;
            float d = Mod(head - s, spacingBetweenBeats);
            float offset = 0f;
            if (d < beatLength)
            {
                float u = 1f - d / beatLength;
                offset = ECGShape(u) * amplitude;
            }
            positions[i] = path[i] + pathNormals[i] * offset;
        }
        lr.SetPositions(positions);
    }

    static float Mod(float a, float m) => a - m * Mathf.Floor(a / m);

    static float Gauss(float u, float center, float width, float height)
    {
        float x = (u - center) / width;
        return height * Mathf.Exp(-x * x);
    }

    /// <summary>Forme P-Q-R-S-T. u dans [0,1], retourne ~[-0.25, 1].</summary>
    static float ECGShape(float u)
    {
        float v = 0f;
        v += Gauss(u, 0.15f, 0.045f, 0.12f);
        v += Gauss(u, 0.36f, 0.014f, -0.14f);
        v += Gauss(u, 0.40f, 0.016f, 1.00f);
        v += Gauss(u, 0.445f, 0.016f, -0.28f);
        v += Gauss(u, 0.70f, 0.070f, 0.28f);

        float edge = Mathf.SmoothStep(0f, 0.05f, u) * Mathf.SmoothStep(0f, 0.05f, 1f - u);
        return v * edge;
    }

    // ---------- Apparence ----------
    void ApplyVisuals()
    {
        lr.startWidth = lr.endWidth = lineWidth;
        lr.startColor = lr.endColor = Color.white;

        // Couleur HDR (> 1) pour déclencher le Bloom, compatible URP/Built-in/Unlit
        Color hdr = new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a);
        lr.GetPropertyBlock(block);
        block.SetColor(BaseColorId, hdr);
        block.SetColor(ColorId, hdr);
        block.SetColor(EmissionId, hdr);
        lr.SetPropertyBlock(block);
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null)
            {
                continue;
            }
            Gizmos.DrawWireSphere(waypoints[i].position, 0.15f);
            if (i > 0 && waypoints[i - 1] != null)
            {
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }
    }
}
