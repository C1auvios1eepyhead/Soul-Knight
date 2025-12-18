using UnityEngine;

public class Sword : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Sword";
        damage = 15f;
        attackRate = 0.7f;
        attackRange = 5f;
    }

    // 单体攻击：只负责“选目标”
    protected override Transform[] PerformAttackWithReturnTargets()
    {
        Transform target = FindTarget();
        return PerformSingleAttackTemplate(target);
    }
}
