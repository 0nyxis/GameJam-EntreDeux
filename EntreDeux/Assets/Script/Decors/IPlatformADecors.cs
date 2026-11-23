using System.Collections;
using UnityEngine;

public class IPlatformADecors : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Bonjour Monsieur Cenzo
        // Bonjour :)


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Boom(Vector2 _pos)
    {
        shockwave = true;
        leShaderGraph.SetVector("_FocalPoint", _pos);

        float t = 0.0f;

        while (t < 1.0f)
        {
            leShaderGraph.SetFloat("_Progress", t);
            t += Time.deltaTime;
            yield return null;
        }

        shockwave = false;
    }
}
