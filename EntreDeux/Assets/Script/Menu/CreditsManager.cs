using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;

    [SerializeField] List<GameObject> page1;
    [SerializeField] List<GameObject> page2;
    [SerializeField] public Canvas canvas;


    private int currentPageNb = 1;


    void Awake()
    {
        foreach (GameObject obj in page2 )
        {
            obj.SetActive(false);
        }
    }


    public void ChangePage(int nb)
    {
        currentPageNb += nb;
        if (currentPageNb < 1)
        {
            currentPageNb = 1;
        }
        TogglePage();
    }

    private void TogglePage()
    {
        if (currentPageNb == 1)
        {
            foreach (GameObject obj in page1)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in page2)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject obj in page1)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in page2)
            {
                obj.SetActive(true);
            }
        }
    }
}

