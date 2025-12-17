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
            primaryWeapon.OnEquip(transform);
        }

        // 自动加载副武器
        WeaponBase knifePrefab = Resources.Load<WeaponBase>("Melee/Knife");
        if (knifePrefab != null)
        {
            secondaryWeapon = Instantiate(knifePrefab);
            secondaryWeapon.OnEquip(transform); // 副武器也挂在玩家上，但不作为当前武器
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

    void SwitchWeapon()
    {
        if (primaryWeapon == null || secondaryWeapon == null) return;

        // 切换当前手上武器
        currentWeapon = currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;
        currentWeapon.OnEquip(transform);
    }

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

    void PickupWeapon(WeaponBase newWeapon)
    {
        if (currentWeapon != null)
            currentWeapon.OnUnequip(true); // 掉落旧武器

        // 更新槽位
        if (currentWeapon == primaryWeapon)
            primaryWeapon = newWeapon;
        else
            secondaryWeapon = newWeapon;

        currentWeapon = newWeapon;
        currentWeapon.OnEquip(transform);
    }
}
