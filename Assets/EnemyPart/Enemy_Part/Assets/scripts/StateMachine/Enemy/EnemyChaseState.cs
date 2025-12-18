using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人追击状态
/// </summary>
public class EnemyChaseState : IState
{
    private Enemy enemy;

    //构造函数
    public EnemyChaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void OnEnter()
    {
        enemy.animator.Play("Walk");//没有追击动画，用走路代替
    }
    public void OnUpdate()
    {
        //判断是否受伤
        if (enemy.isHurt)
        {
            enemy.TransitionState(EnemyStateType.Hurt);
        }

        enemy.GetPlayerTransform();//获取玩家位置

        enemy.AutoPath();//自动寻路

        if (enemy.player != null)
        {
            //判定路径点列表是否为空
            if (enemy.pathPointList == null || enemy.pathPointList.Count <= 0)
                return;

            //是否到攻击范围内
            if (enemy.distance <= enemy.attackDistance)//是否处于攻击范围
            {
                enemy.TransitionState(EnemyStateType.Attack);
            }
            else
            {

                //追逐玩家
                Vector2 direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                enemy.MovementInput = direction;//移动方向传给MovementInput

            }
        }
        else
        {
            //范围外就停止追击，回到待机状态
            enemy.TransitionState(EnemyStateType.Idle);
        }
    }

    public void OnFixedUpdate()
    {
        enemy.Move();
    }

    public void OnExit()
    {

    }

  


}
