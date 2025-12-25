using UnityEngine;

public class Spear : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Spear";
        damage = 60f;
        attackRange = 6f;
        attackAngle = 10f; // С�Ƕ����Σ�ģ�ⳤǹֱ��
        attackRate = 1.5f;
    }

    protected override Transform[] PerformAttackWithReturnTargets()
    {
        // 1. �����������
        Transform nearest = FindTarget();
        if (nearest == null)
        {
            // ������û�е���
            return new Transform[0];
        }

        // 2. �� firePoint ָ���������
        Vector2 dir = nearest.position - firePoint.position;
        firePoint.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        // 3. ��⹥����Χ�ڵĵ��ˣ����Σ�
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
