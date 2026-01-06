using UnityEngine;

public class Map_SlowingZone : MonoBehaviour
{
    [Header("Slow Multipliers")]
    [Range(0.05f, 1f)] public float playerSlow = 0.4f;  // 玩家更慢
    [Range(0.05f, 1f)] public float enemySlow = 0.85f;  // 敌人减速更少

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy（用 InParent，避免 collider 在子物体）
        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.speedMultiplier = enemySlow;
            return;
        }

        // Player（你的脚本类名就是 PlayerMovement）
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm != null)
        {
            pm.speedMultiplier = playerSlow;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.speedMultiplier = 1f;
            return;
        }

        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm != null)
        {
            pm.speedMultiplier = 1f;
        }
    }
}
