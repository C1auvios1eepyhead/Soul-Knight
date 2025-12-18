using UnityEngine;
using System.Collections;

public class Gun : WeaponBase
{
    [Header("Bullet Prefab")]
    public GameObject bulletPrefab;

    [Header("Weapon Range")]
    public float weaponRange = 10f;

    [Header("Bullet Settings")]
    public float weaponBulletSpeed = 20f;

    [Header("Ammo Settings")]
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    [HideInInspector]
    public int currentAmmo;
    protected bool isReloading = false;

    protected override void Awake()
    {
        base.Awake();
        currentAmmo = magazineSize;

        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load<GameObject>("Bullet");
            if (bulletPrefab == null)
                Debug.LogError("Bullet Prefab 找不到！请放到 Assets/Resources/Bullet.prefab");
        }
    }

    // 统一的攻击旋转逻辑
    protected void RotateGunToTarget(Transform target)
    {
        if (target == null) return;

        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 如果朝左，水平翻转
        Vector3 scale = transform.localScale;
        if (dir.x < 0) scale.y = -Mathf.Abs(scale.y);
        else scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;

        // 旋转枪贴图
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // firePoint 同步旋转
        if (firePoint != null)
            firePoint.rotation = transform.rotation;
    }

    public override void Attack()
    {
        if (isReloading) return;
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        if (!CanAttack()) return;

        ResetAttackCD();
        currentAmmo--;

        // 默认目标（最近敌人）
        Transform target = FindTarget();

        // 统一旋转逻辑
        RotateGunToTarget(target);

        // 发射子弹
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = Mathf.RoundToInt(damage);
                bulletScript.speed = weaponBulletSpeed;
                bulletScript.lifeTime = weaponRange / bulletScript.speed;
            }
        }
    }

    protected IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
    }
}
