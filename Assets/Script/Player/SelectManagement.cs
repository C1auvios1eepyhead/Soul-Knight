using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    public static PlayerSelectionManager Instance;
    public PlayerConfig SelectedPlayerConfig;
    public PlayerType SelectedPlayer;
    public PlayerConfig Player { get; set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 关键
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

