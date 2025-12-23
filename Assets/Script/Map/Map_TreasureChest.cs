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

    [System.Serializable]
    public class ChestDropItem
    {
        public GameObject weaponPrefab;
        [Range(0f, 100f)]
        public float weight = 10f;
    }

    [Header("Reward")]
    [SerializeField] private ChestDropItem[] dropItems;
    [SerializeField] private bool noRepeatInThisChest = true;

    private GameObject lastDropped;

    [Header("Level Root")]
    [SerializeField] private Transform levelRoot;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chestOpenSFX;

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

        // 0️⃣ 播放宝箱开启音效
        if (audioSource != null && chestOpenSFX != null)
        {
            audioSource.PlayOneShot(chestOpenSFX, 0.8f);
        }

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
        GameObject chosen = GetRandomWeaponByWeight();

        if (chosen != null)
        {
            if (noRepeatInThisChest && chosen == lastDropped)
            {
                chosen = GetRandomWeaponByWeight();
            }

            lastDropped = chosen;

            GameObject w = Instantiate(chosen, transform.position, Quaternion.identity);

            if (levelRoot != null)
                w.transform.SetParent(levelRoot, true);
        }

    }

    private GameObject GetRandomWeaponByWeight()
    {
        if (dropItems == null || dropItems.Length == 0)
            return null;

        float totalWeight = 0f;

        for (int i = 0; i < dropItems.Length; i++)
        {
            if (dropItems[i].weaponPrefab != null && dropItems[i].weight > 0)
            {
                totalWeight += dropItems[i].weight;
            }
        }

        if (totalWeight <= 0f)
            return null;

        float rand = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < dropItems.Length; i++)
        {
            if (dropItems[i].weaponPrefab == null || dropItems[i].weight <= 0)
                continue;

            current += dropItems[i].weight;

            if (rand <= current)
            {
                return dropItems[i].weaponPrefab;
            }
        }

        return null;
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
