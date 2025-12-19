using UnityEngine;

public class Sword : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Sword";
        damage = 80f;
        attackRate = 0.7f;
        attackRange = 5f;
    }

    // ���幥����ֻ����ѡĿ�ꡱ
    protected override Transform[] PerformAttackWithReturnTargets()
    {
        Transform target = FindTarget();
        return PerformSingleAttackTemplate(target);
    }
}
