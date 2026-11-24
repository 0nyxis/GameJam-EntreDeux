using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int levelIndex;

    private void Awake()
    {
        // Vérifie si une instance existe déjà
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Définit cette instance comme Singleton
        Instance = this;

        // Conserve le GameManager entre les scènes
        DontDestroyOnLoad(gameObject);
    }

    public void PlayerDead()
    {
        levelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("GameOver");
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(levelIndex);
    }
}