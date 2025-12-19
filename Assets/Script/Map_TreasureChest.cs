using UnityEngine;
using TMPro;

public class Map_TreasureChest : MonoBehaviour
{
    [Header("Chest Sprites")]
    public Sprite closedSprite;     // 未开启宝箱
    public Sprite openedSprite;     // 已开启宝箱

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Collider2D chestCollider; 
    public TMP_Text promptText;              // “Press E to open”

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Reward")]
    [SerializeField] private GameObject[] weaponPrefabs;   // 多把武器预制件
    [SerializeField] private bool noRepeatInThisChest = true; // 同一个宝箱不重复(可选)
    private GameObject lastDropped;

    [Header("Level Root")]
    [SerializeField] private Transform levelRoot;

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

        // 4   生成武器
        if (weaponPrefabs != null && weaponPrefabs.Length > 0)
        {
            GameObject chosen = weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];

            if (noRepeatInThisChest && weaponPrefabs.Length > 1 && chosen == lastDropped)
            {
                chosen = weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
            }

            lastDropped = chosen;

            if (chosen != null)
            {
                GameObject w = Instantiate(chosen, transform.position, Quaternion.identity);

                w.transform.SetParent(transform, true);
            }
        }

    }

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
