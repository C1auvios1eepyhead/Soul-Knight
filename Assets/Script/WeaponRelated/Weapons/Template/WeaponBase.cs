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

    // ����Ƿ�������ϳ���
    [HideInInspector] public bool isEquipped = false;

    protected virtual void Awake()
    {
        // ǿ�Ʋ��� FirePoint ������
        if (firePoint == null || firePoint.gameObject == null)
            firePoint = transform.Find("FirePoint");

        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = Vector3.zero;
            firePoint = fp.transform;
        }
    }

    // �������ʵ�ֹ����߼�
    public abstract void Attack();

    // ������ȴ�ж�
    public bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    protected void ResetAttackCD()
    {
        nextAttackTime = Time.time + attackRate;
    }

    // �����������
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

    protected void AimAtTarget(Transform target)
    {
        if (target == null) return;

        Vector2 dir = target.position - firePoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    // װ��
    public virtual void OnEquip(Transform holder)
    {
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isEquipped = true;
    }

    // ȡ��װ��
    public virtual void OnUnequip(bool drop = false)
    {
        transform.SetParent(null);
        isEquipped = false;
    }
}
