using System.Collections.Generic;
using UnityEngine;

public class Map_MonsterRoomController : MonoBehaviour
{
    [Header("Monster Identify")]
    [SerializeField] private string monsterTag = "Monster";

    [Header("Test Spawn (optional)")]
    [Tooltip("If no monster is found in the room, 1 monster will be automatically generated for testing purposes.")]
    [SerializeField] private bool autoSpawnDummyIfNone = true;

    [Tooltip("The monster Prefab used for testing (please set the tag as Monster)")]
    [SerializeField] private GameObject dummyMonsterPrefab;

    [Tooltip("Which layers are considered as barriers?）")]
    [SerializeField] private LayerMask spawnBlockMask;

    [SerializeField] private Vector2 spawnCheckBoxSize = new Vector2(0.6f, 0.6f);

    [Header("Rewards")]
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private Transform rewardSpawnPoint; 
    [SerializeField] private bool spawnChestOnClear = true;

    private bool rewardSpawned = false;

    [Header("Boss Rewards")]
    [SerializeField] private bool spawnPortalOnClear = false;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Transform bossPortalSpawnPoint;
    private GameObject spawnedPortal;

    [Header("Audio")]
    [SerializeField] private bool isBossRoom = false;

    [Header("Wave System (Optional)")]
    [SerializeField] private EnemyManager waveManager;  
    [SerializeField] private bool useWaveSystem = true;  

    [Header("Debug")]
    public bool hasCleared = false;

    private readonly List<Map_DoorToggle> doors = new List<Map_DoorToggle>();
    private readonly List<GameObject> monsters = new List<GameObject>();

    private bool inBattle = false;
    private Transform roomRoot;
    private Collider2D roomBoundsCol;

    private void Awake()
    {
        roomRoot = (transform.parent != null) ? transform.parent : transform;

        foreach (var c in roomRoot.GetComponentsInChildren<Collider2D>(true))
        {
            if (c != null && c.gameObject.name.Contains("RoomBounds"))
            {
                roomBoundsCol = c;
                break;
            }
        }

        CollectMonstersSafe();
    }

    public void SetDoors(Map_DoorToggle[] newDoors)
    {
        doors.Clear();
        if (newDoors == null) return;

        for (int i = 0; i < newDoors.Length; i++)
        {
            if (newDoors[i] != null && !doors.Contains(newDoors[i]))
                doors.Add(newDoors[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasCleared) return;
        if (inBattle) return;

        if (doors.Count == 0)
            CollectDoors();

        // Refresh monsters upon entering
        CollectMonstersSafe();

        // If there are no monsters, one will be generated automatically.
        if (autoSpawnDummyIfNone && monsters.Count == 0 && dummyMonsterPrefab != null)
        {
            SpawnOneDummyMonster();
            CollectMonstersSafe(); 
        }

        StartBattle();
    }

    private void StartBattle()
    {
        inBattle = true;

        // Play Battle/Boss BGM
        if (isBossRoom && Map_AudioManager.Instance != null)
        {
            Map_AudioManager.Instance.PlayBossBGM();
        }

        Debug.Log($"[MonsterRoom] StartBattle room={roomRoot.name} doors={doors.Count} monsters={monsters.Count}");

        for (int i = 0; i < doors.Count; i++)
        {
            if (doors[i] != null) doors[i].CloseDoor();
        }

        if (useWaveSystem && waveManager != null)
        {
            waveManager.StartWaves();
        }
    }

    private void Update()
    {
        if (!inBattle) return;

        // Test: Press P to clear monsters
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("[MonsterRoom] DEBUG: Force clear room (P key)");
            KillAllMonsters();
        }

        if (useWaveSystem && waveManager != null)
        {
            // 所有波次结束并且当前怪清空，才算通关
            if (waveManager.AllWavesCleared())
            {
                EndBattle();
            }
        }
        else
        {
            CollectMonstersSafe();
            if (AreAllMonstersDead())
            {
                EndBattle();
            }
        }

    }

    private void EndBattle()
    {
        inBattle = false;
        hasCleared = true;

        // 1️⃣ 开门
        for (int i = 0; i < doors.Count; i++)
        {
            if (doors[i] != null)
                doors[i].OpenDoor();
        }

        // 2️⃣ 生成宝箱
        if (spawnChestOnClear && !rewardSpawned && chestPrefab != null)
        {
            rewardSpawned = true;

            Vector3 spawnPos;

            if (rewardSpawnPoint != null)
            {
                // Boss 房固定位置
                spawnPos = rewardSpawnPoint.position;
            }
            else
            {
                // 普通房随机
                spawnPos = roomRoot.position;
                if (roomBoundsCol != null)
                {
                    Bounds b = roomBoundsCol.bounds;
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 p = new Vector2(
                            Random.Range(b.min.x, b.max.x),
                            Random.Range(b.min.y, b.max.y)
                        );

                        if (Physics2D.OverlapBox(p, spawnCheckBoxSize, 0f, spawnBlockMask) == null)
                        {
                            spawnPos = p;
                            break;
                        }
                    }
                }
            }

            Instantiate(chestPrefab, spawnPos, Quaternion.identity, roomRoot);
        }

        // 3️⃣ Boss 房生成传送门（你问的第三步就在这里）
        if (spawnPortalOnClear && portalPrefab != null && bossPortalSpawnPoint != null && spawnedPortal == null)
        {
            spawnedPortal = Instantiate(
                portalPrefab,
                bossPortalSpawnPoint.position,
                Quaternion.identity,
                roomRoot
            );
        }

        if (isBossRoom && Map_AudioManager.Instance != null)
        {
            Map_AudioManager.Instance.StopBGM(true);
        }

        Debug.Log($"[MonsterRoom] Cleared room={roomRoot.name}");
    }


    private void CollectDoors()
    {
        var found = roomRoot.GetComponentsInChildren<Map_DoorToggle>(true);
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null && !doors.Contains(found[i]))
                doors.Add(found[i]);
        }
    }

    private void CollectMonstersSafe()
    {
        monsters.RemoveAll(m => m == null);

        foreach (Transform t in roomRoot.GetComponentsInChildren<Transform>(true))
        {
            if (t == null) continue;

            bool isMonster = false;
            try { isMonster = t.CompareTag(monsterTag); }
            catch { isMonster = false; }

            if (isMonster && !monsters.Contains(t.gameObject))
                monsters.Add(t.gameObject);
        }
    }

    private bool AreAllMonstersDead()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            var m = monsters[i];
            if (m != null && m.activeInHierarchy) return false;
        }
        return true;
    }

    private void KillAllMonsters()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] != null) Destroy(monsters[i]);
        }
    }

    private void SpawnOneDummyMonster()
    {
        Vector2 spawnPos = roomRoot.position;

        if (roomBoundsCol != null)
        {
            Bounds b = roomBoundsCol.bounds;

            for (int i = 0; i < 30; i++)
            {
                float x = Random.Range(b.min.x, b.max.x);
                float y = Random.Range(b.min.y, b.max.y);
                Vector2 p = new Vector2(x, y);

                if (Physics2D.OverlapBox(p, spawnCheckBoxSize, 0f, spawnBlockMask) == null)
                {
                    spawnPos = p;
                    break;
                }
            }
        }

        var go = Instantiate(dummyMonsterPrefab, spawnPos, Quaternion.identity, roomRoot);
        try { go.tag = monsterTag; } catch { }
    }
}