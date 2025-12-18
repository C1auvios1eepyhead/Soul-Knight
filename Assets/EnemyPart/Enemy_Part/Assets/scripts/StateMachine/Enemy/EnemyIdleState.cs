using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人待机状态
/// </summary>
public class EnemyIdleState : IState
{
    private Enemy enemy;

    private float Timer = 0;//计时器

    //构造函数
    public EnemyIdleState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void OnEnter()
    {
        enemy.animator.Play("Idle");
        enemy.rb.velocity = Vector2.zero;//待机时不要移动
    }
   
    public void OnUpdate()
    {
        //判断是否受伤
        if (enemy.isHurt)
        {
            enemy.TransitionState(EnemyStateType.Hurt);
        }

        enemy.GetPlayerTransform();//获取玩家位置

        if (enemy.player != null)//如果玩家不为空
        {
            if (enemy.distance > enemy.attackDistance)//大于攻击距离，切换为追击状态
            {
                enemy.TransitionState(EnemyStateType.Chase);
            }
            else if (enemy.distance <= enemy.attackDistance)//小于等于攻击距离切换为攻击状态
            {
                enemy.TransitionState(EnemyStateType.Attack);
            }
        }
        else { //如果玩家为空,等待一定时间切换到巡逻状态
            if (Timer <= enemy.IdleDuration)
            {
                Timer += Time.deltaTime;
            }
            else
            {
                Timer = 0;
                enemy.TransitionState(EnemyStateType.Patrol);
            }
        }
    }
    public void OnFixedUpdate()
    {
        
    }

    public void OnExit()
    {
    
    }


}
