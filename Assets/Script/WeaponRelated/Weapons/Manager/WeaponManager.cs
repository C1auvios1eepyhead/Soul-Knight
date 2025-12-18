using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon pick and switch setting")]
    public float pickupRange = 1.2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode switchKey = KeyCode.Q;

    [Header("Current weapon")]
    [SerializeField] private WeaponBase primaryWeapon;
    [SerializeField] private WeaponBase secondaryWeapon;
    [SerializeField] private WeaponBase currentWeapon;

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
        if (currentWeapon != null)
        {
            // 旧武器掉落地图上显示
            currentWeapon.OnUnequip(true);
            currentWeapon.gameObject.SetActive(true);
        }

        // 更新槽位
        if (currentWeapon == primaryWeapon)
            primaryWeapon = newWeapon;
        else
            secondaryWeapon = newWeapon;

        currentWeapon = newWeapon;

        // 装备新武器到手上并显示
        currentWeapon.OnEquip(PlayerHandAnchor.HandPoint);
        currentWeapon.gameObject.SetActive(true);
    }
}
