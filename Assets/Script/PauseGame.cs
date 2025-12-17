using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true); // 显示UI
        Time.timeScale = 0f;       // 暂停游戏
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false); 
        Time.timeScale = 1f;       // 继续游戏
        isPaused = false;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;       // 一定要恢复时间
        SceneManager.LoadScene("Main");
    }
}
