using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [HideInInspector]
    public CreditsManager creditsManager;
    [HideInInspector]
    public SettingsManager settingsManager;
    [HideInInspector]
    public IngameMenuManager ingameMenuManager;

    public bool canPauseGame = false;

    private bool crdtupd;
    private bool sttupdg;
    private bool igmupd;
    InputAction pauseAction;

    private void Start()
    {

        //if component Exist, attribute it
        if (!gameObject.GetComponent<CreditsManager>().IsUnityNull())
        {
            creditsManager = gameObject.GetComponent<CreditsManager>();
            creditsManager.canvas.enabled = false;
            crdtupd = true;
        }
        else
        {
            crdtupd = false;
        }
        if (!gameObject.GetComponent<SettingsManager>().IsUnityNull())
        {
            settingsManager = gameObject.GetComponent<SettingsManager>();
            settingsManager.canvas.enabled = false;
            sttupdg = true;
        }
        else
        {
            sttupdg = false;
        }
        if (!gameObject.GetComponent<IngameMenuManager>().IsUnityNull())
        {
            ingameMenuManager = gameObject.GetComponent<IngameMenuManager>();
            ingameMenuManager.canvas.enabled = false;
            igmupd = true;
        }
        else
        {
            igmupd = false;
        }

        if (canPauseGame)
        {
            pauseAction = InputSystem.actions.FindAction("Player/Pause");
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
        }

    }

    //works with any canvas specified below
    public void ToggleCanvas(string name)
    {
        if (crdtupd)
        {
            if (creditsManager.GetType().ToString().Remove(creditsManager.GetType().ToString().IndexOf('M')).Contains(name))
            {
                creditsManager.canvas.enabled = !creditsManager.canvas.enabled;
            }
        }
        if (sttupdg)
        {
            if (settingsManager.GetType().ToString().Remove(settingsManager.GetType().ToString().IndexOf('M')).Contains(name))
            {
                settingsManager.canvas.enabled = !settingsManager.canvas.enabled;
            }
        }
        if (igmupd)
        {
            if (ingameMenuManager.GetType().ToString().Remove(ingameMenuManager.GetType().ToString().IndexOf('M')).Contains(name))
            {
                ingameMenuManager.canvas.enabled = !ingameMenuManager.canvas.enabled;
            }
        }
    }

    private void Update()
    {
        if (canPauseGame)
        {
            if (pauseAction.WasCompletedThisFrame())
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        ToggleCanvas("Ingame");
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
