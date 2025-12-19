using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButtons : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f;          // 防止之前暂停
        SceneManager.LoadScene("Game"); // 你的游戏主场景名
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }
}
