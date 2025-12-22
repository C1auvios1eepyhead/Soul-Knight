using UnityEngine;

public abstract class MagicWand : WeaponBase
{
    [Header("Magic Wand Base Settings")]
    public float effectDuration = 2f;   // 状态持续时间（燃烧/减速/僵直）
    public float attackRange = 5f;      // 魔杖攻击最大有效距离

    // 每种魔杖攻击方式完全由子类实现
    public override abstract void Attack();


    // 公共AOE伤害接口 (子类可调用)

    protected void ApplyAOEDamage(Vector3 pos, float radius, float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius);
        foreach (var h in hits)
        {
            if (h.CompareTag("Enemy"))
            {
                // TODO Enemy.TakeDamage(damage)
            }
        }
    }

    // 公共施加状态接口 (留给子类使用)
    protected virtual void ApplyEffect(Transform enemy, float duration, float value)
    {
        // 将来写 EnemyStatusManager 承载效果
        // enemy.GetComponent<Enemy>().ApplyBurn(duration,value);
    }


}
