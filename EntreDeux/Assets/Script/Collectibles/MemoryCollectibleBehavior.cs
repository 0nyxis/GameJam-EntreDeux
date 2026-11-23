using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MemoryCollectibleBehavior : MonoBehaviour
{
    private enum State { Idle, GoingTo, Following }

    [Header("Movement")]
    [Tooltip("Temps de lissage : plus c'est grand, plus le souvenir est lent et flottant.")]
    [SerializeField] private float smoothTime = 0.6f;

    [Tooltip("Vitesse maximale du souvenir.")]
    [SerializeField] private float maxSpeed = 10f;

    [Tooltip("Layer du joueur ou de l'entité qui récupèrera le contrôle du souvenir.")]
    [SerializeField] private LayerMask targetLayerMask;

    [Header("Follow")]
    [Tooltip("Rayon autour de la cible dans lequel le souvenir flâne.")]
    [SerializeField] private float followRadius = 2.5f;

    [Tooltip("Vitesse d'évolution du point de flânerie (plus petit = plus lent).")]
    [SerializeField] private float wanderSpeed = 0.35f;

    [Header("Idle")]
    [Tooltip("Rayon de flânerie quand le souvenir est au repos.")]
    [SerializeField] private float idleRadius = 0.4f;

    [Header("Bobbing")]
    [Tooltip("Amplitude du petit flottement vertical.")]
    [SerializeField] private float bobAmplitude = 0.12f;

    [Tooltip("Fréquence du flottement.")]
    [SerializeField] private float bobFrequency = 1.3f;

    [Header("Arrival")]
    [Tooltip("Distance à partir de laquelle on considère être arrivé à destination.")]
    [SerializeField] private float arrivalDistance = 0.15f;

    //////////////////////////////////////////////////////////////////////////////////////////
    //[Tooltip("Empty if the memory hasn't been collected")]
    //[HideInInspector]
    //public Player playerRef;

    ////animation variables
    //private Vector3 startPosition;
    //private Vector3 endPosition;
    //////////////////////////////////////////////////////////////////////////////////////////

    private State state = State.Idle;
    private Transform target;
    private Vector2 destination;
    private Vector2 idleCenter;

    // Position "logique" lissée, sans le flottement visuel.
    private Vector2 basePosition;
    private Vector2 velocity;

    // Décalage aléatoire pour que chaque souvenir ait son propre rythme.
    private float seed;
    private float startZ;

    void Start()
    {
        //startPosition = transform.position;
        //endPosition = new Vector3(startPosition.x, startPosition.y + 1, startPosition.z);
        seed = Random.value * 100f;
        startZ = transform.position.z;
        basePosition = transform.position;
        idleCenter = basePosition;
    }

    void Update()
    {
        //if (playerRef != null)
        //{
        //    //Follow behavior
        //    FollowPlayer();
        //}
        //else
        //{
        //    //anim behavior
        //    AnimationUpdate();
        //}

        Vector2 anchor = ComputeAnchor();

        // Déplacement lissé vers l'ancre : accélération et décélération douces.
        basePosition = Vector2.SmoothDamp(basePosition, anchor, ref velocity, smoothTime, maxSpeed);

        // Petit flottement vertical par-dessus.
        float flotte = Mathf.Sin((Time.time + seed) * bobFrequency * Mathf.PI * 2f) * bobAmplitude;

        transform.position = new Vector3(basePosition.x, basePosition.y + flotte, startZ);
    }

    [Tooltip("Function to call when using a memory")]
    public void ConsumeCollectible()
    {
        Destroy(gameObject);
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    //private void AnimationUpdate()
    //{
    //    timer += Time.deltaTime;
    //    float t = (Mathf.Sin(timer) + 1) / 2.0f;
    //    //float t = Mathf.PingPong(timer * 0.5f, 1.0f);
    //    gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
    //}

    //private void FollowPlayer()
    //{
    //    transform.position = new Vector3(playerRef.transform.position.x - 1, playerRef.transform.position.y + 1, playerRef.transform.position.z);
    //}

    //////////////////////////////////////////////////////////////////////////////////////////

    private Vector2 ComputeAnchor()
    {
        switch (state)
        {
            case State.GoingTo:
                if (Vector2.Distance(basePosition, destination) <= arrivalDistance)
                {
                    idleCenter = destination;
                    state = State.Idle;
                }
                return destination;

            case State.Following:
                if (target == null)
                {
                    state = State.Idle;
                    idleCenter = basePosition;
                    return basePosition;
                }
                return (Vector2)target.position + WanderOffset(followRadius);

            default:
                return idleCenter + WanderOffset(idleRadius);
        }
    }

    /// <summary>
    /// Point qui évolue lentement et continuellement (bruit de Perlin),
    /// donc pas de changement de direction brusque.
    /// </summary>
    private Vector2 WanderOffset(float radius)
    {
        float t = (Time.time + seed) * wanderSpeed;
        float x = Mathf.PerlinNoise(t, seed) * 2f - 1f;
        float y = Mathf.PerlinNoise(seed + 50f, t) * 2f - 1f;
        return new Vector2(x, y) * radius;
    }

    /// <summary>
    /// Lance un nouveau déplacement vers un point unique.
    /// </summary>
    public void GotoTarget(Vector2 newDestination)
    {
        destination = newDestination;
        state = State.GoingTo;
    }

    /// <summary>
    /// Oublier le déplacement et suivre la cible.
    /// </summary>
    public void DoNotGotoTargetAnymore()
    {
        if (target != null)
            state = State.Following;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayerMask) != 0)
        {
            //Add it to the list the player has (component or inScript) so it can be affected by the follow player script 
            ///uncomment this (will need a memoryCollectibleList in the player)
            ///playerRef = collision.gameObject.GetComponent<Player>();
            ///playerRef.MemoryList.Add(this);

            target = collision.transform;
            state = State.Following;
        }
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.DrawWireSphere(target.position, followRadius);
        }
    }
}