using UnityEngine;

public class Knife : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Knife";
        damage = 30f;
        attackRate = 0.7f;
        attackRange = 4f;
    }

    // ���幥��
    protected override Transform[] PerformAttackWithReturnTargets()
    {
        Transform target = FindTarget();
        return PerformSingleAttackTemplate(target);
    }
}
