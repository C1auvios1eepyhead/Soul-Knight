using UnityEngine;

public class Map_MonsterRoomController : MonoBehaviour
{
    public Map_DoorToggle door;        // 房间的门控制脚本（挂在 MonsterRoom_00 上）
    public GameObject[] monsters;      // 这个房间里的怪物（可以先留空）

    bool battleStarted = false;

    void Start()
    {
        // 怪物数组为 null 就直接略过，避免报错
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
        Debug.Log("OnTriggerEnter2D: " + other.name);  // 调试用，看有没有进来

        if (!battleStarted && other.CompareTag("Player"))
        {
            Debug.Log("Player enter room, start battle!");
            StartBattle();
        }
    }

    void StartBattle()
    {
        battleStarted = true;

        if (door != null)
        {
            Debug.Log("CloseDoor from MonsterRoomController");
            door.CloseDoor();
        }
        else
        {
            Debug.LogWarning("MonsterRoomController: door is NULL!");
        }

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
            // 没有怪物也当作战斗结束
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
        if (door != null)
        {
            Debug.Log("OpenDoor from MonsterRoomController");
            door.OpenDoor();
        }

        battleStarted = false;
    }
}
