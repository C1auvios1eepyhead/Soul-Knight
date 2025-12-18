using UnityEngine;
using System.Collections;

public abstract class Melee : WeaponBase
{
    [Header("Melee Settings")]
    public float attackRange = 1f;           // 攻击范围
    public float attackAngle = 60f;          // 扇形AOE角度（只对AOE类武器有效）
    public float visualOffset = 0.5f;        // VisualAttack偏移距离
    public float visualDuration = 0.2f;      // VisualAttack显示时间

    private GameObject visualInstance;

    // 子类决定是单体攻击还是AOE攻击
    protected abstract Transform[] PerformAttackWithReturnTargets();

    // 修正警告：使用 override
    protected override void Awake()
    {
        base.Awake();
        // 这里可以添加近战武器共有初始化逻辑，如果没有可留空
    }

    public override void Attack()
    {
        if (!CanAttack()) return;

        ResetAttackCD();

        // 隐藏武器贴图
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // 执行子类攻击逻辑并返回被攻击的敌人
        Transform[] targetsHit = PerformAttackWithReturnTargets();

        // 生成VisualAttack
        GameObject visualPrefab = Resources.Load<GameObject>("Melee/VisualAttack");
        if (visualPrefab != null)
        {
            if (targetsHit.Length > 0)
            {
                foreach (Transform t in targetsHit)
                {
                    if (t != null)
                    {
                        GameObject visual = Instantiate(visualPrefab, t.position, Quaternion.identity);
                        visual.transform.SetParent(transform);
                        Destroy(visual, visualDuration);
                    }
                }
            }
            else
            {
                // 场景没有敌人或没打到敌人时，渲染在武器右边一点
                Vector2 offsetPos = (Vector2)firePoint.position + Vector2.right * visualOffset;
                GameObject visual = Instantiate(visualPrefab, offsetPos, Quaternion.identity);
                visual.transform.SetParent(transform);
                Destroy(visual, visualDuration);
            }
        }

        // 恢复武器贴图
        StartCoroutine(RestoreWeaponSprite(visualDuration));
    }

    private IEnumerator RestoreWeaponSprite(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;
    }

    // 单体攻击模板
    protected Transform[] PerformSingleAttackTemplate(Transform target)
    {
        if (target == null) return new Transform[0];

        float distance = Vector2.Distance(firePoint.position, target.position);
        if (distance > attackRange) return new Transform[0];

        //造成伤害
        target.GetComponent<Character>()?.TakeDamage(damage);

        Debug.Log($"Single attack hits {target.name} for {damage} damage");
        return new Transform[] { target };
    }

    // AOE攻击模板
    protected Transform[] PerformAOEAttackTemplate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var hitList = new System.Collections.Generic.List<Transform>();

        foreach (GameObject enemy in enemies)
        {
            Vector2 dirToEnemy = enemy.transform.position - firePoint.position;
            float distance = dirToEnemy.magnitude;
            if (distance <= attackRange)
            {
                float angleToEnemy = Vector2.Angle(Vector2.right, dirToEnemy); // 可改为手持方向
                if (angleToEnemy <= attackAngle / 2f)
                {
                    //造成伤害
                    enemy.GetComponent<Character>()?.TakeDamage(damage);

                    Debug.Log($"AOE attack hits {enemy.name} for {damage} damage");
                    hitList.Add(enemy.transform);
                }
            }
        }

        return hitList.ToArray();
    }

    
    
}

