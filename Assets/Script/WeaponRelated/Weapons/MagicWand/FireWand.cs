using UnityEngine;

public class FireWand : MagicWand
{
    [Header("Fire Wand Settings")]
    public GameObject fireballPrefab;  // Fireball 预制体
    public float fireballSpeed = 8f;   // 火球飞行速度

    protected override void Awake()
    {
        base.Awake();

        // 设置武器名称
        weaponName = "Fire Wand";

        // 自动装载 Fireball Prefab
        if (fireballPrefab == null)
        {
            fireballPrefab = Resources.Load<GameObject>("Fireball");
            if (fireballPrefab == null)
            {
                Debug.LogError("Fireball Prefab 找不到！请放到 Assets/Resources/Fireball.prefab");
            }
        }
    }

    public override void Attack()
    {
        if (!CanAttack()) return;
        ResetAttackCD();

        // 查找最近敌人并瞄准
        Transform target = FindTarget();
        AimAtTarget(target);

        if (fireballPrefab != null && firePoint != null)
        {
            // 生成火球
            GameObject fireballObj = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

            // 设置 Fireball 参数
            Fireball fb = fireballObj.GetComponent<Fireball>();
            if (fb != null)
            {
                fb.damage = damage;                    // 来自 WeaponBase
                fb.aoeRadius = attackRange;            // 使用 MagicWand.attackRange 作为AOE半径
                fb.effectDuration = effectDuration;    // 来自 MagicWand
            }

            // 给火球 Rigidbody2D 设置速度
            Rigidbody2D rb = fireballObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.right * fireballSpeed;
            }

            // 根据射程和速度设置火球存在时间
            if (fb != null)
            {
                fb.lifeTime = Mathf.Max(fb.lifeTime, attackRange / fireballSpeed);
            }
        }
    }
}
