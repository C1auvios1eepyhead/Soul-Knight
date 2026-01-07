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
    protected Coroutine reloadCoroutine;   // ★ 新增：缓存换弹协程

    protected override void Awake()
    {
        base.Awake();

        currentAmmo = magazineSize;

        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load<GameObject>("Bullet");
            if (bulletPrefab == null)
            {
                Debug.LogError("Bullet Prefab not found at Assets/Resources/Bullet.prefab");
            }
        }
    }

    // ★ 新增：当武器被切换回来时的兜底逻辑
    protected virtual void OnEnable()
    {
        // 如果切回来时弹夹是空的，自动开始换弹
        if (currentAmmo <= 0 && !isReloading)
        {
            StartReload();
        }
    }

    // ★ 新增：核心修 bug 的地方
    protected virtual void OnDisable()
    {
        // 如果在换弹过程中被切走，强制停止协程
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        // ★ 关键：一定要把状态清掉
        isReloading = false;
    }

    // 统一的枪支旋转逻辑
    protected void RotateGunToTarget(Transform target)
    {
        if (target == null) return;

        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Vector3 scale = transform.localScale;
        if (dir.x < 0) scale.y = -Mathf.Abs(scale.y);
        else scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (firePoint != null)
            firePoint.rotation = transform.rotation;
    }

    public override void Attack()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        if (!CanAttack()) return;

        ResetAttackCD();
        currentAmmo--;

        // 打完这一枪刚好没子弹了 → 自动换弹
        if (currentAmmo <= 0)
        {
            StartReload();
        }

        Transform target = FindTarget();
        RotateGunToTarget(target);

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

        Weapon_SoundManager.Instance?.PlaySound(WeaponSoundType.GunFire);
    }

    // ★ 新增：统一的换弹入口（不要到处 StartCoroutine）
    protected void StartReload()
    {
        if (isReloading) return;

        reloadCoroutine = StartCoroutine(Reload());
    }

    protected IEnumerator Reload()
    {
        isReloading = true;
        Weapon_SoundManager.Instance?.PlaySound(WeaponSoundType.Reload);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;
        reloadCoroutine = null;
    }
}
