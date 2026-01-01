using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainscene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("select"); // 游戏场景名
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // 编辑器测试用
    }
}
