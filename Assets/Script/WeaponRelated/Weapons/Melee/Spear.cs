using UnityEngine;

public class Spear : Melee
{
    protected override void Awake()
    {
        base.Awake();

        weaponName = "Spear";
        damage = 12f;        // 长枪伤害
        attackRange = 2f;    // 攻击半径，长半径模拟直线攻击
        attackAngle = 5f;   // 小角度扇形
        attackRate = 2f;    // 攻击间隔
    }

    protected override void PerformAttack()
    {
        // 1. 查找最近敌人
        Transform target = FindTarget();

        if (target != null)
        {
            // 2. 让 firePoint 朝向最近敌人
            Vector2 dir = target.position - firePoint.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 3. 扇形检测范围内所有敌人
        Collider2D[] hits = Physics2D.OverlapCircleAll(firePoint.position, attackRange);
        int hitCount = 0;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Vector2 directionToTarget = hit.transform.position - (Vector3)firePoint.position;
                float angleToTarget = Vector2.Angle(firePoint.right, directionToTarget);

                if (angleToTarget <= attackAngle / 2f)
                {
                    hitCount++;

                    // TODO: 调用敌人受击接口
                    // 例如: hit.GetComponent<Enemy>()?.TakeDamage(damage);
                }
            }
        }

        // 4. 日志显示命中数量
        Debug.Log($"Spear Attack: {hitCount} enemies detected in range.");
    }
}
