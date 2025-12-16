using UnityEngine;
using TMPro;

public class Map_TreasureChest : MonoBehaviour
{
    [Header("Chest Sprites")]
    public Sprite closedSprite;     // 未开启宝箱
    public Sprite openedSprite;     // 已开启宝箱

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Collider2D chestCollider;        // 实体碰撞（BoxCollider2D）
    public TMP_Text promptText;              // “Press E to open”

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Reward")]
    public GameObject weaponPrefab;          // 掉落武器

    private bool playerInRange = false;
    private bool opened = false;

    private void Start()
    {
        // 初始化状态
        if (spriteRenderer != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || opened) return;

        if (Input.GetKeyDown(interactKey))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        opened = true;

        // 1️⃣ 换贴图
        if (spriteRenderer != null && openedSprite != null)
            spriteRenderer.sprite = openedSprite;

        // 2️⃣ 关闭碰撞（宝箱不再挡路）
        if (chestCollider != null)
            chestCollider.enabled = false;

        // 3️⃣ 隐藏提示
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        // 4️⃣ 生成武器
        if (weaponPrefab != null)
            Instantiate(weaponPrefab, transform.position, Quaternion.identity);
    }

    // ===== 由 Trigger 调用 =====
    public void OnPlayerEnter()
    {
        if (opened) return;
        playerInRange = true;

        if (promptText != null)
            promptText.gameObject.SetActive(true);
    }

    public void OnPlayerExit()
    {
        playerInRange = false;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
}
