using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [HideInInspector]
    public CreditsManager creditsManager;
    [HideInInspector]
    public SettingsManager settingsManager;

    private void Start()
    {

        //if component Exist, attribute it
        if (!gameObject.GetComponent<CreditsManager>().IsUnityNull())
        {
            creditsManager = gameObject.GetComponent<CreditsManager>();
            creditsManager.canvas.enabled = false;
        }
        if (!gameObject.GetComponent<SettingsManager>().IsUnityNull())
        {
            settingsManager = gameObject.GetComponent<SettingsManager>();
            settingsManager.canvas.enabled = false;
        }

    }

    //works with any canvas specified below
    public void ToggleCanvas(string name)
    {
        if (creditsManager.GetType().ToString().Remove(creditsManager.GetType().ToString().IndexOf('M')).Contains(name))
        {
            creditsManager.canvas.enabled = !creditsManager.canvas.enabled;
        }
        else if (settingsManager.GetType().ToString().Remove(settingsManager.GetType().ToString().IndexOf('M')).Contains(name))
        {
            settingsManager.canvas.enabled = !settingsManager.canvas.enabled;
        }


    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
