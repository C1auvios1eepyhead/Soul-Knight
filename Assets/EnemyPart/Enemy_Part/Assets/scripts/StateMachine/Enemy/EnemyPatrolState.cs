using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人巡逻状态
/// </summary>
public class EnemyPatrolState : IState
{
    private Enemy enemy;

    private Vector2 direction;

    //构造函数
    public EnemyPatrolState(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()
    {
        GeneratePatrolPoint();//进入巡逻状态随机生成巡逻点
        enemy.animator.Play("Walk");//巡逻状态，播放走路动画
    }
    public void OnUpdate()
    {
        //检测是否受伤
        if (enemy.isHurt)
        {
            enemy.TransitionState(EnemyStateType.Hurt);
        }

        //在巡逻过程中发现玩家，切换到追击状态
        enemy.GetPlayerTransform();//获取玩家位置

        if (enemy.player != null)
        {
            enemy.TransitionState(EnemyStateType.Chase);
        }

        //路径点列表为空时，进行路径计算
        if (enemy.pathPointList == null || enemy.pathPointList.Count <= 0)
        {
            //重新生成巡逻点
            GeneratePatrolPoint();
        }
        else
        {
            //当敌人到达当前路径点时，递增索引currentIndex并进行路径计算
            if (Vector2.Distance(enemy.transform.position, enemy.pathPointList[enemy.currentIndex]) <= 0.1f)
            {
                enemy.currentIndex++;

                //到达巡逻点
                if (enemy.currentIndex >= enemy.pathPointList.Count)
                {
                    enemy.TransitionState(EnemyStateType.Idle);//切换到待机状态
                }
                else //未到达巡逻点
                {
                    direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                    enemy.MovementInput = direction;    //移动方向传给MovementInput
                }
            }
            else {//相撞处理
                
                //敌人刚体速度小于敌人默认的当前速度，并且敌人还未到达巡逻点
                if (enemy.rb.velocity.magnitude < enemy.currentSpeed && enemy.currentIndex < enemy.pathPointList.Count)
                {
                    if (enemy.rb.velocity.magnitude == 0)//如果敌人速度为0,在寻路范围外的敌人
                    {
                        direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                        enemy.MovementInput = direction;    //移动方向传给MovementInput
                    }
                    else { //敌人相撞

                        enemy.TransitionState(EnemyStateType.Idle);//切换为待机状态
                    }

                }
            }
         
           

        }

    }


    public void OnFixedUpdate()
    {
        enemy.Move();
    }



    public void OnExit()
    {

    }

    //获得随机巡逻点
    public void GeneratePatrolPoint()
    {
        while (true)
        {
            //随机选择一个巡逻点索引
            int i = Random.Range(0, enemy.patrolPoints.Length);

            //排除当前索引
            if (enemy.targetPointIndex != i)
            {
                enemy.targetPointIndex = i;
                break;//退出死循环
            }
        }

        //把巡逻点给生成路径点函数
        enemy.GeneratePath(enemy.patrolPoints[enemy.targetPointIndex].position);

    }
}
