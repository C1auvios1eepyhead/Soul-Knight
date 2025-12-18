using UnityEngine;

public class Spear : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Spear";
        damage = 12f;
        attackRange = 6f;
        attackAngle = 10f; // 小角度扇形，模拟长枪直线
        attackRate = 2f;
    }

    protected override Transform[] PerformAttackWithReturnTargets()
    {
        // 1. 查找最近敌人
        Transform nearest = FindTarget();
        if (nearest == null)
        {
            // 场景中没有敌人
            return new Transform[0];
        }

        // 2. 让 firePoint 指向最近敌人
        Vector2 dir = nearest.position - firePoint.position;
        firePoint.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        // 3. 检测攻击范围内的敌人（扇形）
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
                    Debug.Log($"Spear hits {enemy.name} for {damage} damage");
                    hitList.Add(enemy.transform);
                }
            }
        }

        return hitList.ToArray();
    }
}
