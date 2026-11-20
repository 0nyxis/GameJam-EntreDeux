using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [HideInInspector]
    public CreditsManager creditsManager;

    private void Start()
    {
        creditsManager = gameObject.GetComponent<CreditsManager>();
        creditsManager.canvas.enabled = false;
    }


    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
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
