using UnityEngine;

public class Map_TreasureChestTrigger : MonoBehaviour
{
    public Map_TreasureChest chest;

    private void Reset()
    {
        chest = GetComponentInParent<Map_TreasureChest>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            chest.OnPlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            chest.OnPlayerExit();
    }
}
