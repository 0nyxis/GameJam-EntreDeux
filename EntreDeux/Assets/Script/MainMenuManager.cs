using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //[SerializeField]
    //Canvas settingsCanvas;


    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }
    public void OpenSettings()
    {
        //enable the setting canvas if its done like this
    }
    public void QuitGame()
    {
        Application.Quit();
    }



}
