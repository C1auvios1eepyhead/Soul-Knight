using UnityEngine;

public class SpiritAim : MonoBehaviour
{
    public TargetingSystem targetingSystem;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (targetingSystem.currentTarget == null)
            return;

        float targetX = targetingSystem.currentTarget.position.x;
        float selfX = transform.position.x;

        if (targetX > selfX)
            spriteRenderer.flipX = false; // 朝右
        else
            spriteRenderer.flipX = true;  // 朝左
    }
}
