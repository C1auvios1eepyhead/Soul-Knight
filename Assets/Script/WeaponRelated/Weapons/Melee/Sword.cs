using UnityEngine;

public class Sword : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Sword";
        damage = 15f;       // 单体剑的伤害
        attackRate = 0.7f;  // 攻击间隔
        attackRange = 2f;   // 攻击范围，单体可小一点
    }

    protected override void PerformAttack()
    {
        // 查找最近的目标
        Transform target = FindTarget();
        // 执行单体攻击
        PerformSingleAttack(target);

        // TODO: 将来这里可以调用敌人的受击接口
        // 例如：target.GetComponent<Enemy>()?.TakeDamage(damage);
    }
}
