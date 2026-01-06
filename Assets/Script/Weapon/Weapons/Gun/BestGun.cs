using UnityEngine;

public class BestGun : Gun
{
    [Header("BestGun Settings")]
    public int pellets = 7;
    public float spreadAngle = 30f;

    protected override void Awake()
    {
        base.Awake();
        weaponName = "BestGun";
        damage = 80f;
        pellets = 8;
        spreadAngle = 30f;
        attackRate = 0.3f;
        weaponBulletSpeed = 45f;
        weaponRange = 21f;
        magazineSize = 30;
        reloadTime = 1f;
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

        // ���ദ����ת�߼�
        Transform target = FindTarget();
        RotateGunToTarget(target);

        // ����ɢ��
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
        Weapon_SoundManager.Instance?.PlaySound(WeaponSoundType.GunFire);

    }
}
