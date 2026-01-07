using UnityEngine;

public class Sword : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Sword";
        damage = 150f;
        attackRate = 0.6f;
        attackRange = 6f;
    }

    // ���幥����ֻ����ѡĿ�ꡱ
    protected override Transform[] PerformAttackWithReturnTargets()
    {
        Transform target = FindTarget();
        return PerformSingleAttackTemplate(target);
    }
}
