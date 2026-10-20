using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    private bool isLoadingEndScene;

    private void OnTriggerEnter(Collider other)
    {
        if (isLoadingEndScene || !other.CompareTag("Player"))
        {
            return;
        }

        isLoadingEndScene = true;
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveFinalScore();
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("The level has no next scene in Build Settings.", this);
            isLoadingEndScene = false;
            return;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }
}
