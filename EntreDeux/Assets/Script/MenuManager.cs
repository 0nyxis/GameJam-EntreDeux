using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour//rename to MenuManager
{
    [HideInInspector]
    public CreditsManager creditsManager;

    private void Start()
    {
        
        //if componentExist, attribute it
        if (!gameObject.GetComponent<CreditsManager>().IsUnityNull())
        {
            creditsManager = gameObject.GetComponent<CreditsManager>();
            creditsManager.canvas.enabled = false;
        }

    }

    public void PlayScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSettings()
    {
        //enable the setting canvas if its done like this
    }
    public void ToggleCredits()
    {
        creditsManager.canvas.enabled = !creditsManager.canvas.enabled;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
