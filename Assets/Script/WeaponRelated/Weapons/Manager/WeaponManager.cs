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
    [Tooltip("Hierarchy 里 Map_LevelManager 的对象名字（必须完全一致）")]
    [SerializeField] private string levelManagerName = "Map_LevelManager";

    [Tooltip("如果 Map_LevelManager 下有这个子节点，就把掉落武器放进去；没有则直接挂到 Map_LevelManager 下")]
    [SerializeField] private string droppedContainerName = "DroppedWeapons";

    [Header("Drop placement")]
    [Tooltip("掉落在玩家脚边的偏移（避免和玩家重叠）")]
    [SerializeField] private Vector3 dropOffset = new Vector3(0.5f, 0f, 0f);

    void Start()
    {
        // 自动加载主武器
        WeaponBase hgPrefab = Resources.Load<WeaponBase>("Gun/HG");
        if (hgPrefab != null)
        {
            primaryWeapon = Instantiate(hgPrefab);
            primaryWeapon.OnEquip(PlayerHandAnchor.HandPoint);
            primaryWeapon.gameObject.SetActive(true); // 当前武器显示
        }

        // 自动加载副武器
        WeaponBase knifePrefab = Resources.Load<WeaponBase>("Melee/Knife");
        if (knifePrefab != null)
        {
            secondaryWeapon = Instantiate(knifePrefab);
            secondaryWeapon.OnEquip(PlayerHandAnchor.HandPoint);
            secondaryWeapon.gameObject.SetActive(false); // 非当前武器隐藏
        }

        currentWeapon = primaryWeapon;
    }

    void Update()
    {
        UpdateWeaponUI();
        // 切换武器
        if (Input.GetKeyDown(switchKey))
            SwitchWeapon();

        // 尝试拾取附近武器
        if (Input.GetKeyDown(pickupKey))
        {
            WeaponBase nearest = FindNearestWeapon();
            if (nearest != null)
                PickupWeapon(nearest);
        }

        // 玩家手上武器攻击
        if (Input.GetKey(KeyCode.Space) && currentWeapon != null && currentWeapon.CanAttack())
        {
            currentWeapon.Attack();
        }
    }

    /// <summary>
    /// 切换当前手上武器
    /// </summary>
    void SwitchWeapon()
    {
        if (primaryWeapon == null || secondaryWeapon == null) return;

        // 隐藏当前手上武器
        currentWeapon.gameObject.SetActive(false);

        // 切换
        currentWeapon = currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;

        // 装备到手上并显示
        currentWeapon.OnEquip(PlayerHandAnchor.HandPoint);
        currentWeapon.gameObject.SetActive(true);
    }

    /// <summary>
    /// 查找玩家周围最近的可拾取武器
    /// </summary>
    WeaponBase FindNearestWeapon()
    {
        WeaponBase[] weapons = GameObject.FindObjectsOfType<WeaponBase>();
        WeaponBase nearest = null;
        float minDist = float.MaxValue;

        foreach (WeaponBase w in weapons)
        {
            if (w.isEquipped) continue; // 手上武器跳过

            float dist = Vector2.Distance(transform.position, w.transform.position);
            if (dist <= pickupRange && dist < minDist)
            {
                minDist = dist;
                nearest = w;
            }
        }

        return nearest;
    }

    /// <summary>
    /// 拾取武器并替换当前手上武器
    /// </summary>
    void PickupWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null) return;

        // 如果捡到的是自己手上的武器（极少数情况），直接返回
        if (newWeapon == currentWeapon) return;

        // ① 旧武器掉落到 Map_LevelManager
        if (currentWeapon != null)
        {
            DropWeaponToMapLevelManager(currentWeapon);
        }

        // ② 更新槽位
        if (currentWeapon == primaryWeapon)
            primaryWeapon = newWeapon;
        else
            secondaryWeapon = newWeapon;

        // ③ 切换当前武器并装备
        currentWeapon = newWeapon;
        currentWeapon.OnEquip(PlayerHandAnchor.HandPoint);
        currentWeapon.gameObject.SetActive(true);
    }

    /// <summary>
    /// 方法一：根据名字找到 Map_LevelManager，然后把旧武器直接挂到它下面（或其 DroppedWeapons 子节点）
    /// </summary>
    private void DropWeaponToMapLevelManager(WeaponBase weapon)
    {
        if (weapon == null) return;

        // 1) 卸下（解除 HandPoint 的父子关系，避免继续跟着 DDOL 玩家）
        weapon.OnUnequip(true);

        // 2) 放到玩家脚边
        weapon.transform.position = transform.position + dropOffset;

        // 3) 关键：把武器从 DontDestroyOnLoad 场景搬回“当前活动场景”
        SceneManager.MoveGameObjectToScene(weapon.gameObject, SceneManager.GetActiveScene());

        // 4) 按“路径”找 Map_LevelManager -> (可选) DroppedWeapons
        Transform parent = FindDropParentByName();
        if (parent != null)
            weapon.transform.SetParent(parent, true);
        else
            weapon.transform.SetParent(null, true); // 找不到就先丢在场景根

        // 5) 确保状态正确（可拾取、可见）
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
    /// <summary>
    /// 找到掉落物的父节点：
    /// - 先找 Map_LevelManager
    /// - 再尝试找它下面的 DroppedWeapons
    /// </summary>
    private Transform FindDropParentByName()
    {
        GameObject lm = GameObject.Find(levelManagerName);
        if (lm == null)
        {
            Debug.LogWarning($"[WeaponManager] Cannot find GameObject named '{levelManagerName}'. " +
                             $"Old weapon will be dropped to scene root.");
            return null;
        }

        // 如果有子节点 DroppedWeapons，就挂到那里
        Transform container = lm.transform.Find(droppedContainerName);
        return container != null ? container : lm.transform;
    }
}
