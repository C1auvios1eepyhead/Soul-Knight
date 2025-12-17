using UnityEngine;

public class Knife : Melee
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Knife";
        damage = 5f;       
        attackRate = 0.7f;  
        attackRange = 1f;   
    }

    protected override void PerformAttack()
    {
        // 查找最近的目标
        Transform target = FindTarget();
        // 执行单体攻击
        PerformSingleAttack(target);

        // TODO: 将来这里可以调用敌人的受击接口
        // 例如：target.GetComponent<Enemy>()?.TakeDamage(damage);
    }
}
