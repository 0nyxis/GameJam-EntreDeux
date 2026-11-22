using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int levelIndex;

    public static void PlayerDead()
    {
        levelIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene("GameOver");

    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(levelIndex);
    }
}
