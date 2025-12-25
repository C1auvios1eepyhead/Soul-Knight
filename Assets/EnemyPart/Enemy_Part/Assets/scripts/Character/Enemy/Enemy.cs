using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
//敌人状态枚举
public enum EnemyStateType
{ 
    Idle,Patrol,Chase,Attack,Hurt,Death
}


public class Enemy : Character
{
    [Header("目标")]
    public Transform player;
    [Header("待机巡逻")]
    public float IdleDuration; //待机时间
    public Transform[] patrolPoints;//巡逻点
    public int targetPointIndex = 0;//目标点索引

    [Header("移动追击")]
    public float currentSpeed = 0;
    public Vector2 MovementInput { get; set; }

    public float chaseDistance = 3f;//追击距离
    public float attackDistance = 0.8f;//攻击距离

    private Seeker seeker;
    [HideInInspector] public List<Vector3> pathPointList;//路径点列表
    [HideInInspector] public int currentIndex = 0;//路径点的索引
    private float pathGenerateInterval = 0.5f; //每0.5秒生成一次路径
    private float pathGenerateTimer = 0f;//计时器

    [Header("攻击")]
    public float meleeAttackDamage;//攻击伤害
    public bool isAttack = true;
    [HideInInspector] public float distance;
    public LayerMask playerLayer;//表示玩家图层
    public float AttackCooldownDuration = 2f;//冷却时间

    [Header("受伤击退")]
    public bool isHurt;
    public bool isKnokback = true;//击退

    public bool isSuperArmor = false;//霸体
    public float knokbackForce = 10f;
    public float knokbackForceDuration = 0.1f;

    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Collider2D enemyCollider;

    private IState currentState;//当前状态

    //字典Dictionary<键，值>对
    private Dictionary<EnemyStateType, IState> states = new Dictionary<EnemyStateType, IState>();

    private void Awake()
    {
        seeker = GetComponent<Seeker>();//寻路组件
        sr = GetComponent<SpriteRenderer>();//图片组件
        rb = GetComponent<Rigidbody2D>();//刚体组件
        enemyCollider = GetComponent<Collider2D>();//碰撞器组件
        animator = GetComponent<Animator>();//动画控制器组件

        //实例化敌人状态
        states.Add(EnemyStateType.Idle, new EnemyIdleState(this));
        states.Add(EnemyStateType.Chase, new EnemyChaseState(this));
        states.Add(EnemyStateType.Attack, new EnemyAttackState(this));
        states.Add(EnemyStateType.Hurt, new EnemyHurtState(this));
        states.Add(EnemyStateType.Death, new EnemyDeathState(this));
        states.Add(EnemyStateType.Patrol, new EnemyPatrolState(this));

        //设置默认状态为Idle
        TransitionState(EnemyStateType.Idle);
    }

    //用于切换敌人状态的函数
    public void TransitionState(EnemyStateType type)
    {
        //当前状态不为空，让他退出当前状态
        if (currentState != null)
        {
            currentState.OnExit();
        }
        //通过字典的键来找到对应的状态,进入新状态
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

    //判定玩家是否在追击范围内
    public void GetPlayerTransform()
    {
        Collider2D[] chaseColliders = Physics2D.OverlapCircleAll(transform.position, chaseDistance, playerLayer);

        if (chaseColliders.Length > 0)//玩家在追击范围内
        {
            player = chaseColliders[0].transform;//获取玩家的Transform
            distance = Vector2.Distance(player.position, transform.position);
        }
        else
        {
            player = null;//玩家在追击范围外
        }
    }

    #region 自动寻路
    //自动寻路
    public void AutoPath()
    {
        pathGenerateTimer += Time.deltaTime;

        //间隔一定时间来获取路径点
        if (pathGenerateTimer >= pathGenerateInterval)
        {
            GeneratePath(player.position);
            pathGenerateTimer = 0;//重置计时器
        }


        //当路径点列表为空时，进行路径计算
        if (pathPointList == null || pathPointList.Count <= 0)
        {
            GeneratePath(player.position);
        }//当敌人到达当前路径点时，递增索引currentIndex并进行路径计算
        else if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.1f)
        {
            currentIndex++;
            if (currentIndex >= pathPointList.Count)
                GeneratePath(player.position);
        }
    }

    //获取路径点
    public void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        //三个参数：起点、终点、回调函数
        seeker.StartPath(transform.position, target, Path =>
        {
            pathPointList = Path.vectorPath;//Path.vectorPath包含了从起点到终点的完整路径
        });
    }
    #endregion

    #region 移动

    //移动函数
    public void Move()
    {
        if (MovementInput.magnitude > 0.1f && currentSpeed >= 0)
        {
            rb.velocity = MovementInput * currentSpeed;
            //敌人左右翻转
            if (MovementInput.x < 0)//左
            {
                sr.flipX = false;
            }
            if (MovementInput.x > 0)//右
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

    #region 敌人近战攻击帧事件
    //敌人近战攻击
public virtual void Attack()
{
    // 默认近战攻击
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

    //攻击冷却时间
    IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(AttackCooldownDuration);
        isAttack = true;
    }
    #endregion

    #region 受伤
    //动人受伤事件触发的回调函数
    public void EnemyHurt()
    {
        isHurt = true;
    }
    #endregion

    #region 死亡
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
        //显示攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        //显示追击范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

}