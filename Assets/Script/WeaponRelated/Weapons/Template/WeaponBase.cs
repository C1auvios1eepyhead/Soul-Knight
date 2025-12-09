using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName = "Weapon";
    public float damage = 10f;
    public float attackRate = 1f;

    [Header("Fire Point")]
    public Transform firePoint;

    protected float nextAttackTime = 0f;
    public System.Action OnAttackEvent;

    protected virtual void Awake()
    {
        // 强制查找 FirePoint 子物体
        if (firePoint == null || firePoint.gameObject == null)
        {
            firePoint = transform.Find("FirePoint");
        }

        // 如果找不到，就创建
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.name = "FirePoint";
            fp.transform.SetParent(transform);
            fp.transform.localPosition = Vector3.zero;
            firePoint = fp.transform;
        }
    }

    // 子类必须实现攻击逻辑
    public abstract void Attack();


    // 攻击冷却判断
    public bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    protected void ResetAttackCD()
    {
        nextAttackTime = Time.time + attackRate;
    }

    private void Update()
    {
        // 按空格攻击
        if (Input.GetKey(KeyCode.Space))
        {
            Attack();
        }
    }

    // 查找目标（默认最近敌人，可被子类重写）
    protected virtual Transform FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(firePoint.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e.transform;
            }
        }
        return closest;
    }

    // 旋转 firePoint 朝向目标
    protected void AimAtTarget(Transform target)
    {
        if (target == null) return;

        Vector2 dir = target.position - firePoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }


    // 预留给装备系统
    public virtual void OnEquip(Transform holder)
    {
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void OnUnequip()
    {
        transform.SetParent(null);
    }
}
