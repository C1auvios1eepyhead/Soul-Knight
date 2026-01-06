using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���˹���״̬
/// </summary>
public class EnemyAttackState : IState
{
    private Enemy enemy;

    private AnimatorStateInfo info;
    //���캯��
    public EnemyAttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void OnEnter()
    {
        if (enemy.isAttack)
        {
            enemy.animator.Play("Attack");//���Ź�������
            enemy.isAttack = false;
            enemy.AttackColdown();//��ȴʱ��
        }
        
    }
    public void OnUpdate()
    {
        //�ж��Ƿ�����
        if (enemy.isHurt)
        {
            enemy.TransitionState(EnemyStateType.Hurt);
        }

        //��ֹ�����ƶ�
        enemy.rb.velocity = Vector2.zero;
        //���﷭ת
        float x = enemy.player.position.x - enemy.transform.position.x;
        if (x > 0)
        {
            enemy.sr.flipX = true;
        }
        else 
        {
            enemy.sr.flipX = false;
        }
        //��ȡ���˽�ɫ��ǰ���ŵĶ���״̬����Ϣ
        info = enemy.animator.GetCurrentAnimatorStateInfo(0);

     
        if (info.normalizedTime >= 1f)//��������л���������
        {
            Debug.Log("����"+info.normalizedTime);
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