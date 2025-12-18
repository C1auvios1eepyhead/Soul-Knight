using UnityEngine;

// 近战武器基类
public abstract class Melee : WeaponBase
{
    [Header("Melee Settings")]
    public float attackRange = 1f;       // 攻击范围
    public float attackAngle = 60f;      // 扇形AOE角度（只对AOE类武器有效）

    // 子类决定是单体攻击还是AOE攻击
    protected abstract void PerformAttack();

    public override void Attack()
    {
        if (!CanAttack()) return;

        ResetAttackCD();
        PerformAttack();
    }

    // 单体攻击模板，子类可调用
    protected void PerformSingleAttack(Transform target)
    {
        if (target == null) return;

        // 计算距离
        float distance = Vector2.Distance(firePoint.position, target.position);

        if (distance > attackRange)
        {
            Debug.Log($"{target.name} is out of range!");
            return;
        }

        // TODO: 调用敌人受击接口
        Debug.Log($"Single attack hits {target.name} for {damage} damage");
    }


    // AOE攻击模板，子类可调用
    protected void PerformAOEAttack()
    {
        // 检测所有带 "Enemy" Tag 的对象
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Vector2 directionToEnemy = enemy.transform.position - firePoint.position;
            float distance = directionToEnemy.magnitude;

            // 在攻击范围内
            if (distance <= attackRange)
            {
                // 扇形AOE检测
                float angleToEnemy = Vector2.Angle(firePoint.right, directionToEnemy);
                if (angleToEnemy <= attackAngle / 2f)
                {
                    // TODO: 调用敌人受击接口
                    // enemy.GetComponent<Enemy>()?.TakeDamage(damage);
                    Debug.Log($"AOE attack hits {enemy.name} for {damage} damage");
                }
            }
        }
    }

    // 可视化攻击范围，方便调试
    protected virtual void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, attackRange);
        }
    }
}
