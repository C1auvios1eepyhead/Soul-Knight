using UnityEngine;

public class Fireball : Bullet
{
    [Header("Fireball Settings")]
    public float aoeRadius = 2f;        // 命中后AOE范围
    public float effectDuration = 2f;   // 燃烧效果持续时间
    public float burnDamage = 5f;       // 燃烧每次伤害

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 只处理 Tag 为 Enemy 的对象
        if (collision.CompareTag("Enemy"))
        {
            // TODO: 调用敌人接口处理直接伤害
            // 例如：
            // IDamageable enemy = collision.GetComponent<IDamageable>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage(damage);
            //     enemy.ApplyBurn(burnDamage, effectDuration);
            // }

            // AOE 伤害
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
            foreach (var h in hits)
            {
                if (h.CompareTag("Enemy"))
                {
                    // TODO: 调用敌人接口处理 AOE 伤害和燃烧效果
                }
            }
        }

        Destroy(gameObject); // 命中即销毁
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
