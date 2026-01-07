using UnityEngine;

public class Knife : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Knife";
        damage = 100f;
        attackRate = 0.4f;
        attackRange = 3f;
    }

    // ���幥��
    protected override Transform[] PerformAttackWithReturnTargets()
    {
        Transform target = FindTarget();
        return PerformSingleAttackTemplate(target);
    }
}
