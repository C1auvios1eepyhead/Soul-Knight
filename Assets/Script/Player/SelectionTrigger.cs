using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("下一场景名称")]
    public string nextSceneName = "Game";

    private void OnTriggerEnter(Collider other)
    {
        // 判断碰到的是玩家
        if(other.CompareTag("Player"))
        {
            // 加载下一个场景
            Debug.Log("111");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
