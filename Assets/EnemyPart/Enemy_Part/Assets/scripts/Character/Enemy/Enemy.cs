using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
//����״̬ö��
public enum EnemyStateType
{ 
    Idle,Patrol,Chase,Attack,Hurt,Death
}


public class Enemy : Character
{
    [Header("Ŀ��")]
    public Transform player;
    [Header("����Ѳ��")]
    public float IdleDuration; //����ʱ��
    public Transform[] patrolPoints;//Ѳ�ߵ�
    public int targetPointIndex = 0;//Ŀ�������

    [Header("�ƶ�׷��")]
    public float currentSpeed = 0;
    public Vector2 MovementInput { get; set; }

    public float chaseDistance = 3f;//׷������
    public float attackDistance = 0.8f;//��������

    private Seeker seeker;
    [HideInInspector] public List<Vector3> pathPointList;//·�����б�
    [HideInInspector] public int currentIndex = 0;//·���������
    private float pathGenerateInterval = 0.5f; //ÿ0.5������һ��·��
    private float pathGenerateTimer = 0f;//��ʱ��

    [Header("����")]
    public float meleeAttackDamage;//�����˺�
    public bool isAttack = true;
    [HideInInspector] public float distance;
    public LayerMask playerLayer;//��ʾ���ͼ��
    public float AttackCooldownDuration = 2f;//��ȴʱ��

    [Header("���˻���")]
    public bool isHurt;
    public bool isKnokback = true;
    public float knokbackForce = 10f;
    public float knokbackForceDuration = 0.1f;

    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Collider2D enemyCollider;

    private IState currentState;//��ǰ״̬

    //�ֵ�Dictionary<����ֵ>��
    private Dictionary<EnemyStateType, IState> states = new Dictionary<EnemyStateType, IState>();

    private void Awake()
    {
        seeker = GetComponent<Seeker>();//Ѱ·���
        sr = GetComponent<SpriteRenderer>();//ͼƬ���
        rb = GetComponent<Rigidbody2D>();//�������
        enemyCollider = GetComponent<Collider2D>();//��ײ�����
        animator = GetComponent<Animator>();//�������������

        //ʵ��������״̬
        states.Add(EnemyStateType.Idle, new EnemyIdleState(this));
        states.Add(EnemyStateType.Chase, new EnemyChaseState(this));
        states.Add(EnemyStateType.Attack, new EnemyAttackState(this));
        states.Add(EnemyStateType.Hurt, new EnemyHurtState(this));
        states.Add(EnemyStateType.Death, new EnemyDeathState(this));
        states.Add(EnemyStateType.Patrol, new EnemyPatrolState(this));

        //����Ĭ��״̬ΪIdle
        TransitionState(EnemyStateType.Idle);
    }

    //�����л�����״̬�ĺ���
    public void TransitionState(EnemyStateType type)
    {
        //��ǰ״̬��Ϊ�գ������˳���ǰ״̬
        if (currentState != null)
        {
            currentState.OnExit();
        }
        //ͨ���ֵ�ļ����ҵ���Ӧ��״̬,������״̬
        currentState = states[type];
        currentState.OnEnter();

    }

    private void Start()
    {
        EnemyManager.Instance.enemyCount++;
    }
    private void OnDestroy()
    {
        EnemyManager.Instance.enemyCount--;
    }

    private void Update()
    {
        currentState.OnUpdate();
    }
    private void FixedUpdate()
    {
        currentState.OnFixedUpdate();
    }

    //�ж�����Ƿ���׷����Χ��
    public void GetPlayerTransform()
    {
        Collider2D[] chaseColliders = Physics2D.OverlapCircleAll(transform.position, chaseDistance, playerLayer);

        if (chaseColliders.Length > 0)//�����׷����Χ��
        {
            player = chaseColliders[0].transform;//��ȡ��ҵ�Transform
            distance = Vector2.Distance(player.position, transform.position);
        }
        else
        {
            player = null;//�����׷����Χ��
        }
    }

    #region �Զ�Ѱ·
    //�Զ�Ѱ·
    public void AutoPath()
    {
        pathGenerateTimer += Time.deltaTime;

        //���һ��ʱ������ȡ·����
        if (pathGenerateTimer >= pathGenerateInterval)
        {
            GeneratePath(player.position);
            pathGenerateTimer = 0;//���ü�ʱ��
        }


        //��·�����б�Ϊ��ʱ������·������
        if (pathPointList == null || pathPointList.Count <= 0)
        {
            GeneratePath(player.position);
        }//�����˵��ﵱǰ·����ʱ����������currentIndex������·������
        else if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.1f)
        {
            currentIndex++;
            if (currentIndex >= pathPointList.Count)
                GeneratePath(player.position);
        }
    }

    //��ȡ·����
    public void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        //������������㡢�յ㡢�ص�����
        seeker.StartPath(transform.position, target, Path =>
        {
            pathPointList = Path.vectorPath;//Path.vectorPath�����˴���㵽�յ������·��
        });
    }
    #endregion

    #region �ƶ�

    //�ƶ�����
    public void Move()
    {
        if (MovementInput.magnitude > 0.1f && currentSpeed >= 0)
        {
            rb.velocity = MovementInput * currentSpeed;
            //�������ҷ�ת
            if (MovementInput.x < 0)//��
            {
                sr.flipX = false;
            }
            if (MovementInput.x > 0)//��
            {
                sr.flipX = true;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    #endregion

    #region ���˽�ս����֡�¼�
    //���˽�ս����
public virtual void Attack()
{
    // Ĭ�Ͻ�ս����
    Collider2D[] hitColliders =
        Physics2D.OverlapCircleAll(transform.position, attackDistance, playerLayer);

    foreach (Collider2D hitCollider in hitColliders)
    {
        hitCollider.GetComponent<PlayerHealth>()
            ?.TakeDamage(meleeAttackDamage);
    }
}
    public void AttackColdown()
    {
        StartCoroutine(nameof(AttackCooldownCoroutine));
    }

    //������ȴʱ��
    IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(AttackCooldownDuration);
        isAttack = true;
    }
    #endregion

    #region ����
    //���������¼������Ļص�����
    public void EnemyHurt()
    {
        isHurt = true;
    }
    #endregion

    #region ����
    public void EnemyDie()
    {
        TransitionState(EnemyStateType.Death);
    }
    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        //��ʾ������Χ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        //��ʾ׷����Χ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

}