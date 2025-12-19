using UnityEngine;
using System.Collections;

public abstract class Melee : WeaponBase
{
    [Header("Melee Settings")]
    public float attackRange = 1f;           // ������Χ
    public float attackAngle = 60f;          // ����AOE�Ƕȣ�ֻ��AOE��������Ч��
    public float visualOffset = 0.5f;        // VisualAttackƫ�ƾ���
    public float visualDuration = 0.2f;      // VisualAttack��ʾʱ��

    private GameObject visualInstance;

    // ��������ǵ��幥������AOE����
    protected abstract Transform[] PerformAttackWithReturnTargets();

    // �������棺ʹ�� override
    protected override void Awake()
    {
        base.Awake();
        // ���������ӽ�ս�������г�ʼ���߼������û�п����
    }

    public override void Attack()
    {
        if (!CanAttack()) return;

        ResetAttackCD();

        // ����������ͼ
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // ִ�����๥���߼������ر������ĵ���
        Transform[] targetsHit = PerformAttackWithReturnTargets();

        // ����VisualAttack
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
                // ����û�е��˻�û�򵽵���ʱ����Ⱦ�������ұ�һ��
                Vector2 offsetPos = (Vector2)firePoint.position + Vector2.right * visualOffset;
                GameObject visual = Instantiate(visualPrefab, offsetPos, Quaternion.identity);
                visual.transform.SetParent(transform);
                Destroy(visual, visualDuration);
            }
        }

        // �ָ�������ͼ
        StartCoroutine(RestoreWeaponSprite(visualDuration));
    }

    private IEnumerator RestoreWeaponSprite(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;
    }

    // ���幥��ģ��
    protected Transform[] PerformSingleAttackTemplate(Transform target)
    {
        if (target == null) return new Transform[0];

        float distance = Vector2.Distance(firePoint.position, target.position);
        if (distance > attackRange) return new Transform[0];

        //����˺�
        target.GetComponent<Character>()?.TakeDamage(damage);

        Debug.Log($"Single attack hits {target.name} for {damage} damage");
        return new Transform[] { target };
    }

    // AOE����ģ��
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
                float angleToEnemy = Vector2.Angle(Vector2.right, dirToEnemy); // �ɸ�Ϊ�ֳַ���
                if (angleToEnemy <= attackAngle / 2f)
                {
                    //����˺�
                    enemy.GetComponent<Character>()?.TakeDamage(damage);

                    Debug.Log($"AOE attack hits {enemy.name} for {damage} damage");
                    hitList.Add(enemy.transform);
                }
            }
        }

        return hitList.ToArray();
    }

    
    
}

