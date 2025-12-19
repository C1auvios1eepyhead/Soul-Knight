using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����Ѳ��״̬
/// </summary>
public class EnemyPatrolState : IState
{
    private Enemy enemy;

    private Vector2 direction;

    private float stopThrehold = 3f;
    private float stopTime=0f;

    //���캯��
    public EnemyPatrolState(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()
    {
        GeneratePatrolPoint();//����Ѳ��״̬�������Ѳ�ߵ�
        enemy.animator.Play("Walk");//Ѳ��״̬��������·����
    }
    public void OnUpdate()
    {
        //����Ƿ�����
        if (enemy.isHurt)
        {
            enemy.TransitionState(EnemyStateType.Hurt);
        }

        //��Ѳ�߹����з�����ң��л���׷��״̬
        enemy.GetPlayerTransform();//��ȡ���λ��

        if (enemy.player != null)
        {
            enemy.TransitionState(EnemyStateType.Chase);
        }

        //·�����б�Ϊ��ʱ������·������
        if (enemy.pathPointList == null || enemy.pathPointList.Count <= 0)
        {
            //��������Ѳ�ߵ�
            GeneratePatrolPoint();
        }
        else
        {
            //�����˵��ﵱǰ·����ʱ����������currentIndex������·������
            if (Vector2.Distance(enemy.transform.position, enemy.pathPointList[enemy.currentIndex]) <= 0.1f)
            {
                enemy.currentIndex++;

                //����Ѳ�ߵ�
                if (enemy.currentIndex >= enemy.pathPointList.Count)
                {
                    enemy.TransitionState(EnemyStateType.Idle);//�л�������״̬
                }
                else //δ����Ѳ�ߵ�
                {
                    direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                    enemy.MovementInput = direction;    //�ƶ����򴫸�MovementInput
                }
            }
            else {//��ײ����
                
                //���˸����ٶ�С�ڵ���Ĭ�ϵĵ�ǰ�ٶȣ����ҵ��˻�δ����Ѳ�ߵ�
                if (enemy.rb.velocity.magnitude < enemy.currentSpeed && enemy.currentIndex < enemy.pathPointList.Count)
                {
                    if (enemy.rb.velocity.magnitude <= 0.1f)//��������ٶ�Ϊ0,��Ѱ·��Χ��ĵ���
                    {
                        if (enemy.rb.velocity.magnitude == 0)//��������ٶ�Ϊ0,��Ѱ·��Χ��ĵ���
                        {
                        direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                        enemy.MovementInput = direction;    //�ƶ����򴫸�MovementInput
                        }
                        stopTime+=Time.deltaTime;
                    }
               
                }
            }

            if(stopTime>=stopThrehold)
            {
                enemy.TransitionState(EnemyStateType.Idle);
                stopTime=0;
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

    //������Ѳ�ߵ�
    public void GeneratePatrolPoint()
    {
        while (true)
        {
            //���ѡ��һ��Ѳ�ߵ�����
            int i = Random.Range(0, enemy.patrolPoints.Length);

            //�ų���ǰ����
            if (enemy.targetPointIndex != i)
            {
                enemy.targetPointIndex = i;
                break;//�˳���ѭ��
            }
        }

        //��Ѳ�ߵ������·���㺯��
        enemy.GeneratePath(enemy.patrolPoints[enemy.targetPointIndex].position);

    }
}
