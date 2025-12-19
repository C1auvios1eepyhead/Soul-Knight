using UnityEngine;
using UnityEngine.SceneManagement;

public class Map_GameOverMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameOver"; 

    public void TryAgain()
    {
        Time.timeScale = 1f; // 防止之前暂停过
        SceneManager.LoadScene(gameSceneName);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
