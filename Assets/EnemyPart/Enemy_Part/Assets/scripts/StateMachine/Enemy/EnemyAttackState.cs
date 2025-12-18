using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人攻击状态
/// </summary>
public class EnemyAttackState : IState
{
    private Enemy enemy;

    private AnimatorStateInfo info;
    //构造函数
    public EnemyAttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void OnEnter()
    {
        if (enemy.isAttack)
        {
            enemy.animator.Play("Attack");//播放攻击动画
            enemy.isAttack = false;
            enemy.AttackColdown();//冷却时间
        }
        
    }
    public void OnUpdate()
    {
        //判断是否受伤
        if (enemy.isHurt)
        {
            enemy.TransitionState(EnemyStateType.Hurt);
        }

        //禁止敌人移动
        enemy.rb.velocity = Vector2.zero;
        //人物翻转
        float x = enemy.player.position.x - enemy.transform.position.x;
        if (x > 0)
        {
            enemy.sr.flipX = true;
        }
        else 
        {
            enemy.sr.flipX = false;
        }
        //获取敌人角色当前播放的动画状态的信息
        info = enemy.animator.GetCurrentAnimatorStateInfo(0);

     
        if (info.normalizedTime >= 1f)//播放完后切换待机动画
        {
            Debug.Log("触发"+info.normalizedTime);
            enemy.TransitionState(EnemyStateType.Idle);
        }
    }

    public void OnFixedUpdate()
    {

    }

 
    public void OnExit()
    {
  
    }

 

}
