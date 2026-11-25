using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Datas")]
    [SerializeField]
    private List<GameObject> memoriesPrefab;

    public static GameManager Instance;
    public static int levelIndex;

    private Player playerPtr;
    private int nbMemo;

    #endregion

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

    public void ReloadLastLevel()
    {
        ChangeSceneTo(levelIndex);
    }
    public void ChangeSceneTo(string _sceneName)
    {
        if (playerPtr != null)
        {
            nbMemo = playerPtr.GetMemorie().Count;
        }

        SceneManager.LoadScene(_sceneName);
    }
    public void ChangeSceneTo(int _sceneBuildID)
    {
        if (playerPtr != null)
        {
            nbMemo = playerPtr.GetMemorie().Count;
        }

        SceneManager.LoadScene(_sceneBuildID);
    }

    public void RegisterPlayer(Player _player)
    {
        playerPtr = _player;
        levelIndex = SceneManager.GetActiveScene().buildIndex;
        SpawnMemories();
    }

    public void SpawnMemories()
    {
        if (nbMemo > 0)
        {
            int randomNumber = 0;
            Vector2 posTemp = playerPtr.transform.position;
            for (int i = 0; i < nbMemo; i++)
            {
                randomNumber = (int)Random.Range(0, memoriesPrefab.Count - 0.01f);
                Debug.Log("Random : " + randomNumber);
                Instantiate(memoriesPrefab[randomNumber], (Vector3)posTemp, Quaternion.identity);
            }
        }
    }
}