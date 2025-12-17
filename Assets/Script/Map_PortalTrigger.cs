using UnityEngine;
using UnityEngine.SceneManagement;

public class Map_PortalTrigger : MonoBehaviour
{
    public enum PortalMode
    {
        LoadScene,
        EndGame
    }

    [Header("UI")]
    [SerializeField] private GameObject promptText;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Mode")]
    [SerializeField] private PortalMode mode = PortalMode.LoadScene;
    [SerializeField] private string sceneToLoad; // mode=LoadScene 时用

    private bool playerInRange;

    private void Start()
    {
        if (promptText != null) promptText.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (mode == PortalMode.EndGame)
            {
                ExitGame();
            }
            else
            {
                if (!string.IsNullOrEmpty(sceneToLoad)) 
                {
                    CleanupPickups();
                    SceneManager.LoadScene(sceneToLoad);
                }

                else
                    Debug.LogWarning("[Portal] sceneToLoad is empty!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (promptText != null) promptText.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (promptText != null) promptText.SetActive(false);
    }

    private void CleanupPickups()
    {
        int count = 0;

        // 1) 按 Tag 清（你自己确认武器 Tag 是不是 Weapon）
        var tagged = GameObject.FindGameObjectsWithTag("Weapon");
        for (int i = 0; i < tagged.Length; i++)
        {
            Destroy(tagged[i]);
            count++;
        }

        // 2) 双保险：按名字清（防止 Tag 不对）
        var all = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var t in all)
        {
            if (t.name.StartsWith("Weapon_"))   // Weapon_Dummy(Clone) 会匹配
            {
                Destroy(t.gameObject);
                count++;
            }
        }

        Debug.Log($"[Portal] CleanupPickups destroyed: {count}");
    }

    private void ExitGame()
    {
        Debug.Log("[Portal] Boss exit -> end game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
