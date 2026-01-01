using UnityEngine;

public class Knife : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Knife";
        damage = 20f;
        attackRate = 0.6f;
        attackRange = 3f;
    }

    // ���幥��
    protected override Transform[] PerformAttackWithReturnTargets()
    {
        Transform target = FindTarget();
        return PerformSingleAttackTemplate(target);
    }
}
