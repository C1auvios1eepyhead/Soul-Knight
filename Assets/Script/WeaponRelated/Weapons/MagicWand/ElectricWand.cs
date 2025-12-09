using UnityEngine;

public class ElectricWand : MagicWand
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Electric Wand";
        damage = 20f;      // 默认伤害
        attackRate = 0.8f; // 默认攻击间隔
    }

    public override void Attack()
    {
        if (!CanAttack()) return;
        ResetAttackCD();

        // 查找最近的敌人
        Transform target = FindTarget();
        if (target != null)
        {
            // TODO: 调用敌人接口处理伤害
            // 例如：
            // IDamageable enemy = target.GetComponent<IDamageable>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage(damage);
            //     enemy.ApplyStun(effectDuration); // 使用父类统一的 effectDuration
            // }

            
        }
    }
}
