using UnityEngine;

public class Map_MonsterRoomController : MonoBehaviour
{
    public Map_DoorToggle[] doors;   // ✅ 改成数组
    public GameObject[] monsters;

    bool battleStarted = false;

    void Start()
    {
        // 怪物先全部隐藏
        if (monsters != null)
        {
            foreach (var m in monsters)
            {
                if (m != null)
                    m.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D: " + other.name);

        if (!battleStarted && other.CompareTag("Player"))
        {
            Debug.Log("Player enter room, start battle!");
            StartBattle();
        }
    }

    void StartBattle()
    {
        battleStarted = true;

        // ✅ 关闭所有门
        if (doors != null)
        {
            foreach (var d in doors)
            {
                if (d != null)
                {
                    Debug.Log("CloseDoor from MonsterRoomController");
                    d.CloseDoor();
                }
            }
        }

        // 怪物出现
        if (monsters != null)
        {
            foreach (var m in monsters)
            {
                if (m != null)
                    m.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (!battleStarted) return;

        if (monsters == null || monsters.Length == 0)
        {
            return;
        }

        bool allDead = true;
        foreach (var m in monsters)
        {
            if (m != null && m.activeSelf)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            EndBattle();
        }
    }

    void EndBattle()
    {
        // ✅ 打开所有门
        if (doors != null)
        {
            foreach (var d in doors)
            {
                if (d != null)
                {
                    Debug.Log("OpenDoor from MonsterRoomController");
                    d.OpenDoor();
                }
            }
        }

        battleStarted = false;
    }
}
