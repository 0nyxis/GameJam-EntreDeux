using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlatformJump : MonoBehaviour
{
    bool isPlatformActive = false;

    public List<GameObject> PlatformObjectA;
    public List<GameObject> PlatformObjectB;

    void Start()
    {
        PlatformChange(isPlatformActive);
    }

    private void OnEnable()
    {
        Player.OnJumpPressed += PlayJumpFeedback;
    }

    private void OnDisable()
    {
        Player.OnJumpPressed -= PlayJumpFeedback;
    }

    private void PlayJumpFeedback()
    {
        isPlatformActive = !isPlatformActive;
        PlatformChange(isPlatformActive);
    }

    private void PlatformChange(bool change)
    {
        foreach (GameObject platA in PlatformObjectA)
        {
            //platA.SetActive(change);
            platA.GetComponent<Renderer>().enabled = change;
            platA.GetComponent<Collider2D>().enabled = change;
        }

        foreach (GameObject platB in PlatformObjectB)
        {
            //platB.SetActive(!change);
            platB.GetComponent<Renderer>().enabled = !change;
            platB.GetComponent<Collider2D>().enabled = !change;

        }
    }
}
