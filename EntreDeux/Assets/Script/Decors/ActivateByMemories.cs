using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ActivateByMemories : MonoBehaviour
{
    [Tooltip("Un point par pilier. Un souvenir sera envoyé sur chacun.")]
    [SerializeField] private Transform[] pillars;

    [Tooltip("Coché : une fois activé, reste activé définitivement.")]
    [SerializeField] private bool triggerOnce = true;

    [Tooltip("Coché : les souvenirs sont consommés (détruits) à l'activation.")]
    [SerializeField] private bool consumeMemories = false;

    [Header("Events")]
    public UnityEvent onActivated;     // ex : ouvrir la porte
    public UnityEvent onDeactivated;   // ex : refermer la porte (si triggerOnce est décoché)

    private MemoryCollectibleBehavior[] assigned;  // un souvenir par pilier
    private MemoryCarrier carrier;
    private int insideCount;
    private bool activated;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        assigned = new MemoryCollectibleBehavior[pillars.Length];
    }

    private void Update()
    {
        if (carrier == null) return;
        if (activated && triggerOnce) return;

        AssignFreePillars();   // vérifié à chaque frame, au cas où un souvenir arrive
        CheckActivation();
    }

    private void AssignFreePillars()
    {
        foreach (MemoryCollectibleBehavior memory in carrier.Memories)
        {
            if (memory == null || IsAssigned(memory)) continue;

            int free = FindFreePillar();
            if (free < 0) return;   // tous les piliers sont occupés

            assigned[free] = memory;
            memory.GotoTarget(pillars[free].position);
        }
    }

    private void CheckActivation()
    {
        if (activated || pillars.Length == 0) return;

        // Tous les piliers doivent avoir un souvenir ARRIVÉ dessus
        for (int i = 0; i < assigned.Length; i++)
        {
            if (assigned[i] == null || !assigned[i].HasArrived) return;
        }

        activated = true;
        onActivated?.Invoke();

        if (consumeMemories)
        {
            for (int i = 0; i < assigned.Length; i++)
            {
                assigned[i].ConsumeCollectible();
                assigned[i] = null;
            }
        }
    }

    private void ReleaseAll()
    {
        // Les souvenirs retournent suivre le joueur
        for (int i = 0; i < assigned.Length; i++)
        {
            if (assigned[i] != null) assigned[i].DoNotGotoTargetAnymore();
            assigned[i] = null;
        }

        if (activated && !triggerOnce)
        {
            activated = false;
            onDeactivated?.Invoke();
        }
    }

    private bool IsAssigned(MemoryCollectibleBehavior memory)
    {
        for (int i = 0; i < assigned.Length; i++)
            if (assigned[i] == memory) return true;
        return false;
    }

    private int FindFreePillar()
    {
        for (int i = 0; i < assigned.Length; i++)
            if (assigned[i] == null) return i;   // couvre aussi un souvenir détruit
        return -1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        MemoryCarrier found = other.GetComponentInParent<MemoryCarrier>();
        if (found == null) return;

        carrier = found;
        insideCount++;   // le joueur peut avoir plusieurs colliders
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<MemoryCarrier>() == null) return;

        insideCount--;
        if (insideCount <= 0)
        {
            insideCount = 0;
            ReleaseAll();
            carrier = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pillars == null) return;
        Gizmos.color = Color.cyan;
        foreach (Transform p in pillars)
            if (p != null) Gizmos.DrawWireSphere(p.position, 0.3f);
    }
}