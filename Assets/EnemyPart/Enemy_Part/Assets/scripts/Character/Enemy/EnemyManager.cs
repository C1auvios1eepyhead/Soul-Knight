using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public Transform[] spawnPoints;
    public Transform[] patrolPoints;
    public List<EnemyWave> enemyWaves;

    public int currentWaveIndex = 0;
    public int enemyCount = 0;

    private bool running = false;
    private bool spawningWave = false;

    private void Awake()
    {
        Instance = this;
    }

    // ✅ 由房间调用：开始波次
    public void StartWaves()
    {
        if (running) return;
        running = true;
        currentWaveIndex = 0;
    }

    // ✅ 给房间查询：是否所有波次完成且怪清空
    public bool AllWavesCleared()
    {
        return running && currentWaveIndex >= enemyWaves.Count && enemyCount <= 0 && !spawningWave;
    }

    private void Update()
    {
        if (!running) return;
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        // 当前波怪死光 → 刷下一波（只允许开一次协程）
        if (enemyCount <= 0 && !spawningWave)
        {
            StartCoroutine(StartNextWaveCoroutine());
        }
    }

    IEnumerator StartNextWaveCoroutine()
    {
        if (currentWaveIndex >= enemyWaves.Count)
            yield break;

        spawningWave = true;

        var enemies = enemyWaves[currentWaveIndex].enemies;
        foreach (var enemyData in enemies)
        {
            for (int i = 0; i < enemyData.waveEnemyCount; i++)
            {
                GameObject enemy = Instantiate(enemyData.enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);

                // 刷出来就 +1
                enemyCount++;

                if (patrolPoints != null)
                    enemy.GetComponent<Enemy>().patrolPoints = patrolPoints;

                yield return new WaitForSeconds(enemyData.spawnInterval);
            }
        }

        currentWaveIndex++;
        spawningWave = false;
    }

    private Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }


}

//��Ϊû�̳�MonoBehaviour�������Ҫ���л������[System.Serializable] 
[System.Serializable]   
public class EnemyData
{
    public GameObject enemyPrefab;  //����Ԥ����
    public float spawnInterval;     //�������ɼ��
    public float waveEnemyCount;    //��������
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemyData> enemies; //ÿ�������б�
}
