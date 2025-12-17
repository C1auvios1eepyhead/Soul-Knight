using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //单例模式
    public static EnemyManager Instance { get; private set; }

    [Header("敌人刷新点")]
    public Transform[] spawnPoints;

    [Header("敌人巡逻点")]
    public Transform[] patrolPoints;

    [Header("该关卡的敌人")]
    public List<EnemyWave> enemyWaves;

    public int currentWaveIndex = 0; //当前波数的索引

    public int enemyCount = 0;      //敌人数量
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (enemyCount == 0)//当前波数敌人全部死亡，开始下一波
        {
            StartCoroutine(nameof(startNextWaveCoroutine));
        }
    }

    IEnumerator startNextWaveCoroutine()
    {
        if (currentWaveIndex >= enemyWaves.Count)
            yield break;    //已经没有更多波数，直接退出协程

        List<EnemyData> enemies = enemyWaves[currentWaveIndex].enemies;//获取当前波数对应的敌人列表

        foreach (EnemyData enemyData in enemies)
        {
            for (int i = 0; i < enemyData.waveEnemyCount; i++)
            {
                GameObject enemy = Instantiate(enemyData.enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);

                if (patrolPoints != null)//巡逻点列表不为空，就把巡逻点列表赋值给敌人预制体的巡逻点列表
                {
                    enemy.GetComponent<Enemy>().patrolPoints = patrolPoints;
                }

                yield return new WaitForSeconds(enemyData.spawnInterval);
            }
        }

        currentWaveIndex++;
    }

    //从怪物刷新点的位置列表中随机选择一个刷新点
    private Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }
}

//因为没继承MonoBehaviour组件，想要序列化得添加[System.Serializable] 
[System.Serializable]   
public class EnemyData
{
    public GameObject enemyPrefab;  //敌人预制体
    public float spawnInterval;     //怪物生成间隔
    public float waveEnemyCount;    //敌人数量
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemyData> enemies; //每波敌人列表
}
