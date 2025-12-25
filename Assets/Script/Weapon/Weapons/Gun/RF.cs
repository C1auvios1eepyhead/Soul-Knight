using UnityEngine;

public class RF : Gun
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Rifle";
        damage = 80f;
        attackRate = 1.2f;
        weaponBulletSpeed = 40f;
        weaponRange = 25f;
        magazineSize = 5;
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

        // ʹ���Զ��������߼�����Զ���ˣ�
        Transform target = FindFarthestTarget();

        // ʹ�ø�����ת�߼�
        RotateGunToTarget(target);

        // �����ӵ�
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

    protected Transform FindFarthestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform farthest = null;
        float maxDist = 0f;
        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(firePoint.position, e.transform.position);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthest = e.transform;
            }
        }
        return farthest;
    }
}
