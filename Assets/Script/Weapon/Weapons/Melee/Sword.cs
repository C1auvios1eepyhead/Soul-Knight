using UnityEngine;

public class Sword : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Sword";
        damage = 50f;
        attackRate = 0.8f;
        attackRange = 5f;
    }

    // ���幥����ֻ����ѡĿ�ꡱ
    protected override Transform[] PerformAttackWithReturnTargets()
    {
        Transform target = FindTarget();
        return PerformSingleAttackTemplate(target);
    }
}
