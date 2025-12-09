using UnityEngine;

public class RF : Gun
{
    protected override void Awake()
    {
        base.Awake();

        weaponName = "Rifle";
        damage = 20f;           
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

        // 查找最远的目标
        Transform target = FindFarthestTarget();
        AimAtTarget(target);

        // 发射子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = Mathf.RoundToInt(damage);
            bulletScript.speed = weaponBulletSpeed;
            bulletScript.lifeTime = weaponRange / bulletScript.speed;
        }
    }

    // 查找最远敌人
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
