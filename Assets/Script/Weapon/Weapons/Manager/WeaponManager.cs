using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [Header("WeaponUI")]
    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("Weapon pick and switch setting")]
    public float pickupRange = 1.2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode switchKey = KeyCode.Q;

    [Header("Current weapon")]
    [SerializeField] private WeaponBase primaryWeapon;
    [SerializeField] private WeaponBase secondaryWeapon;
    [SerializeField] private WeaponBase currentWeapon;
    public WeaponBase CurrentWeapon => currentWeapon;

    [Header("Drop target (Method 1: Find by name/path)")]
    [SerializeField] private string levelManagerName = "Map_LevelManager";
    [SerializeField] private string droppedContainerName = "DroppedWeapons";

    [Header("Drop placement")]
    [SerializeField] private Vector3 dropOffset = new Vector3(0.5f, 0f, 0f);

    // ---------- 新增 ----------
    private Transform playerTransform; // 玩家 Transform

    // ---------- 修改 Start 为 IEnumerator ----------
    IEnumerator Start()
    {
        // 等到 HandPoint 被生成（PlayerHandAnchor 的 Awake 执行完）
        while (PlayerHandAnchor.HandPoint == null)
            yield return null; // 等一帧再试

        // 尝试先找玩家
        TryFindPlayer();

        // 自动加载主武器
        WeaponBase hgPrefab = Resources.Load<WeaponBase>("Gun/HG");
        if (hgPrefab != null)
        {
            primaryWeapon = Instantiate(hgPrefab);
            primaryWeapon.OnEquip(PlayerHandAnchor.HandPoint);
            primaryWeapon.gameObject.SetActive(true);
        }

        // 自动加载副武器
        WeaponBase knifePrefab = Resources.Load<WeaponBase>("Melee/Knife");
        if (knifePrefab != null)
        {
            secondaryWeapon = Instantiate(knifePrefab);
            secondaryWeapon.OnEquip(PlayerHandAnchor.HandPoint);
            secondaryWeapon.gameObject.SetActive(false);
        }

        currentWeapon = primaryWeapon;
    }

    void LateUpdate()
    {
        // 如果还没找到玩家，每帧尝试一次
        if (playerTransform == null)
            TryFindPlayer();

        // 跟随玩家
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
            transform.rotation = playerTransform.rotation;
        }
    }

    private void TryFindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player"); // 玩家预制体必须打 Player tag
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        UpdateWeaponUI();

        if (Input.GetKeyDown(switchKey))
            SwitchWeapon();

        if (Input.GetKeyDown(pickupKey))
        {
            WeaponBase nearest = FindNearestWeapon();
            if (nearest != null)
                PickupWeapon(nearest);
        }

        if (Input.GetKey(KeyCode.Space) && currentWeapon != null && currentWeapon.CanAttack())
        {
            currentWeapon.Attack();
        }
    }

    void SwitchWeapon()
    {
        if (primaryWeapon == null || secondaryWeapon == null) return;

        currentWeapon.gameObject.SetActive(false);
        currentWeapon = currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;
        currentWeapon.OnEquip(PlayerHandAnchor.HandPoint);
        currentWeapon.gameObject.SetActive(true);
        Weapon_SoundManager.Instance?.PlaySound(WeaponSoundType.WeaponSwitch);
    }

    WeaponBase FindNearestWeapon()
    {
        WeaponBase[] weapons = GameObject.FindObjectsOfType<WeaponBase>();
        WeaponBase nearest = null;
        float minDist = float.MaxValue;

        foreach (WeaponBase w in weapons)
        {
            if (w.isEquipped) continue;
            float dist = Vector2.Distance(transform.position, w.transform.position);
            if (dist <= pickupRange && dist < minDist)
            {
                minDist = dist;
                nearest = w;
            }
        }

        return nearest;
    }

    void PickupWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null || newWeapon == currentWeapon) return;

        if (currentWeapon != null)
            DropWeaponToMapLevelManager(currentWeapon);

        if (currentWeapon == primaryWeapon)
            primaryWeapon = newWeapon;
        else
            secondaryWeapon = newWeapon;

        currentWeapon = newWeapon;
        currentWeapon.OnEquip(PlayerHandAnchor.HandPoint);
        currentWeapon.gameObject.SetActive(true);
        Weapon_SoundManager.Instance?.PlaySound(WeaponSoundType.WeaponPickup);
    }

    private void DropWeaponToMapLevelManager(WeaponBase weapon)
    {
        if (weapon == null) return;

        weapon.OnUnequip(true);
        weapon.transform.position = transform.position + dropOffset;
        SceneManager.MoveGameObjectToScene(weapon.gameObject, SceneManager.GetActiveScene());

        Transform parent = FindDropParentByName();
        if (parent != null)
            weapon.transform.SetParent(parent, true);
        else
            weapon.transform.SetParent(null, true);

        weapon.isEquipped = false;
        weapon.gameObject.SetActive(true);
    }

    private void UpdateWeaponUI()
    {
        if (currentWeapon is Gun gun)
        {
            float percent = (gun.magazineSize <= 0) ? 0f : (float)gun.currentAmmo / gun.magazineSize;
            energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount, percent, 10f * Time.deltaTime);
            energyText.text = $"{gun.currentAmmo}/{gun.magazineSize}";
        }
        else
        {
            energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount, 1f, 10f * Time.deltaTime);
            energyText.text = "--";
        }
    }

    private Transform FindDropParentByName()
    {
        GameObject lm = GameObject.Find(levelManagerName);
        if (lm == null)
        {
            Debug.LogWarning($"[WeaponManager] Cannot find GameObject named '{levelManagerName}'. Old weapon will be dropped to scene root.");
            return null;
        }

        Transform container = lm.transform.Find(droppedContainerName);
        return container != null ? container : lm.transform;
    }
}
