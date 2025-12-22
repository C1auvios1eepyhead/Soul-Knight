using UnityEngine;

public class Scythe : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Scythe";
        damage = 40f;
        attackRange = 5f;
        attackAngle = 90f; // 大扇形
        attackRate = 2f;
    }

    protected override Transform[] PerformAttackWithReturnTargets()
    {
        // 1. 查找最近敌人
        Transform nearest = FindTarget();
        if (nearest == null)
        {
            return new Transform[0];
        }

        // 2. firePoint 指向最近敌人
        Vector2 dir = nearest.position - firePoint.position;
        firePoint.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        // 3. 检测攻击范围内所有敌人（扇形）
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var hitList = new System.Collections.Generic.List<Transform>();

        foreach (GameObject enemy in enemies)
        {
            Vector2 dirToEnemy = enemy.transform.position - firePoint.position;
            float distance = dirToEnemy.magnitude;

            if (distance <= attackRange)
            {
                float angleToEnemy = Vector2.Angle(firePoint.right, dirToEnemy);
                if (angleToEnemy <= attackAngle / 2f)
                {
                    enemy.GetComponent<Character>()?.TakeDamage(damage);
                    Debug.Log($"Scythe hits {enemy.name} for {damage} damage");
                    hitList.Add(enemy.transform);
                }
            }
        }

        return hitList.ToArray();
    }
}
