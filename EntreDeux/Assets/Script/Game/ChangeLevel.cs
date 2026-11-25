 using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            string sceneActuelle = SceneManager.GetActiveScene().name;

            if (sceneActuelle == "Level1")
            {
                GameManager.Instance.ChangeSceneTo("Level2");
            }
            else if (sceneActuelle == "Level2")
            {
                GameManager.Instance.ChangeSceneTo("Level3");
            }
        }
    }
}
