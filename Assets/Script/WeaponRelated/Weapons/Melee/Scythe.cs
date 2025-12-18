using UnityEngine;

public class Scythe : Melee
{
    protected override void Awake()
    {
        base.Awake();

        weaponName = "Scythe";
        damage = 20f;       // 镰刀伤害
        attackRange = 1f;   // 攻击范围
        attackAngle = 90f;  // 大扇形AOE
        attackRate = 2f;    // 攻击间隔
    }

    protected override void PerformAttack()
    {
        // 可以直接复用 Spear 的扇形检测逻辑
        Transform target = FindTarget();

        if (target != null)
        {
            Vector2 dir = target.position - firePoint.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }

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
                }
            }
        }

        Debug.Log($"Scythe Attack: {hitCount} enemies detected in range.");
    }
}
