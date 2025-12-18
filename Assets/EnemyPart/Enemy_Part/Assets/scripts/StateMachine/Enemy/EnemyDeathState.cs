using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人死亡状态
/// </summary>
public class EnemyDeathState : IState
{
    private Enemy enemy;

    //构造函数
    public EnemyDeathState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void OnEnter()
    {
        enemy.animator.Play("Die");//播放死亡动画
        enemy.rb.velocity = Vector2.zero;//禁用刚体移动
        enemy.enemyCollider.enabled = false;//禁用碰撞体
    }
    public void OnUpdate()
    {

    }

    public void OnFixedUpdate()
    {

    }

  
    public void OnExit()
    {
      
    }

   

}
