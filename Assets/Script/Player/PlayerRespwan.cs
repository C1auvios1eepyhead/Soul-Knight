using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Prefabs")]
    public GameObject knightPrefab;
    public GameObject dwarfPrefab;
    public GameObject nunPrefab;
    public GameObject ghostPrefab;
    public GameObject savagePrefab;
    public GameObject wizardPrefab;

    public Transform spawnPoint;

    public WeaponManager weaponManager;
    void Start()
    {
        PlayerType type = PlayerSelectionManager.Instance.SelectedPlayer;
        PlayerConfig selectedPlayerConfig = PlayerSelectionManager.Instance.SelectedPlayerConfig;
        GameObject player = null;

        switch (type)
        {
            case PlayerType.knight:
                player = Instantiate(knightPrefab, spawnPoint.position, Quaternion.identity);
                break;
            case PlayerType.dwarf:
                player = Instantiate(dwarfPrefab, spawnPoint.position, Quaternion.identity);
                break;
            case PlayerType.nun:
                player = Instantiate(nunPrefab, spawnPoint.position, Quaternion.identity);
                break;
            case PlayerType.ghost:
                player = Instantiate(ghostPrefab, spawnPoint.position, Quaternion.identity);
                break;
            case PlayerType.savage:
                player = Instantiate(savagePrefab, spawnPoint.position, Quaternion.identity);
                break;
            case PlayerType.wizard:
                player = Instantiate(wizardPrefab, spawnPoint.position, Quaternion.identity);
                break;
        }


    }
}
