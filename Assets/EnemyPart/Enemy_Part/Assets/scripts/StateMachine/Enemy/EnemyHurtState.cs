using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人受伤状态
/// </summary>

public class EnemyHurtState : IState
{
    private Enemy enemy;

    private Vector2 direction;//击退方向

    private float Timer;//计时器
    //构造函数
    public EnemyHurtState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void OnEnter()
    {
        enemy.animator.Play("Hurt");//播放受伤动画
    }
    public void OnUpdate()
    {
        //判断是否可以击退
        if (enemy.isKnokback)
        {
            if (enemy.player != null)
            {
                direction = (enemy.transform.position - enemy.player.position).normalized;
            }
            else
            {
                //如果敌人在追击范围外打敌人呢，player为null
                Transform player = GameObject.FindWithTag("Player").transform;
                direction = (enemy.transform.position - player.position).normalized;
            }
        }

    }

    public void OnFixedUpdate()
    {
        //施加击退效果
        if (Timer <= enemy.knokbackForceDuration)
        {
            enemy.rb.AddForce(direction * enemy.knokbackForce, ForceMode2D.Impulse);
            Timer += Time.fixedDeltaTime;
        }
        else {
            Timer = 0;
            enemy.isHurt = false;
            //切换到待机状态
            enemy.TransitionState(EnemyStateType.Idle);
        }
    }
   

 
    public void OnExit()
    {
        enemy.isHurt = false;
    }


}
