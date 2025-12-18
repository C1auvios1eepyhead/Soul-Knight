using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun Settings")]
    public int pellets = 5;
    public float spreadAngle = 30f;

    protected override void Awake()
    {
        base.Awake();
        weaponName = "Shotgun";
        damage = 8f;
        pellets = 7;
        spreadAngle = 35f;
        attackRate = 1f;
        weaponBulletSpeed = 15f;
        weaponRange = 6f;
        magazineSize = 6;
        reloadTime = 2f;
        currentAmmo = magazineSize;
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

        // 父类处理旋转逻辑
        Transform target = FindTarget();
        RotateGunToTarget(target);

        // 发射散弹
        for (int i = 0; i < pellets; i++)
        {
            float angleOffset = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angleOffset);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = Mathf.RoundToInt(damage);
                bulletScript.speed = weaponBulletSpeed;
                bulletScript.lifeTime = weaponRange / bulletScript.speed;
            }
        }
    }
}
