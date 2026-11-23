using System.Collections;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [Header("Other Data")]
    [SerializeField] Material leShaderGraph;

    private bool canInteract = false;
    public bool _canInteract => canInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Bonjour Monsieur Cenzo
        // Bonjour :)

        leShaderGraph = Resources.Load<Material>("SG_Shockwave");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Boom(Vector2 _pos)
    {
        canInteract = true;
        leShaderGraph.SetVector("_FocalPoint", _pos);

        float t = 0.0f;

        while (t < 1.0f)
        {
            leShaderGraph.SetFloat("_Progress", t);
            t += Time.deltaTime;
            yield return null;
        }

        canInteract = false;
    }
}
