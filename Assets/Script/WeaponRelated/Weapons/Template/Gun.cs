using UnityEngine;
using System.Collections;

public class Gun : WeaponBase
{
    [Header("Bullet Prefab")]
    public GameObject bulletPrefab;      // 子弹预制体

    [Header("Weapon Range")]
    public float weaponRange = 10f;     // 默认射程

    [Header("Bullet Settings")]
    public float weaponBulletSpeed = 20f; // 子弹速度

    [Header("Ammo Settings")]
    public int magazineSize = 30;       // 弹匣容量
    public float reloadTime = 1.5f;     // 换弹时间
    [HideInInspector]
    public int currentAmmo;             // 当前剩余子弹
    protected bool isReloading = false;   // 是否正在换弹

    protected override void Awake()
    {
        base.Awake();
        currentAmmo = magazineSize;     //默认满弹匣

       
        // 自动装载 Bullet Prefab
        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load<GameObject>("Bullet");

            if (bulletPrefab == null)
            {
                Debug.LogError("Bullet Prefab 找不到！请放到 Assets/Resources/Bullet.prefab");
            }
        }
    }


   

    public override void Attack()
    {
        if (isReloading) return;          // 正在换弹不能射击
        if (currentAmmo <= 0)             // 弹匣空了自动换弹
        {
            StartCoroutine(Reload());
            return;
        }

        if (!CanAttack()) return;

        ResetAttackCD();
        currentAmmo--; // 发射前减少弹药

        // 查找目标并瞄准
        Transform target = FindTarget();
        AimAtTarget(target);

        // 发射子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = Mathf.RoundToInt(damage);
            bulletScript.speed = weaponBulletSpeed;

            // 让子弹根据武器射程自动消失
            bulletScript.lifeTime = weaponRange / bulletScript.speed;
        }

    }

    
    // 弹匣换弹协程
    protected IEnumerator Reload()
    {
        isReloading = true;
        // 可以在这里播放换弹动画或音效
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
    }
}
