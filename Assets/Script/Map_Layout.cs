using System.Collections.Generic;
using UnityEngine;

public class Map_Layout : MonoBehaviour
{
    [Header("Door Points")]
    public Transform doorPointUp;
    public Transform doorPointDown;
    public Transform doorPointLeft;
    public Transform doorPointRight;

    [Header("Gate Prefab（四个方向共用一个）")]
    public Map_DoorToggle gatePrefab; 

    [Header("Wall Prefabs（四个方向各一份，因为砖块不同）")]
    public GameObject wallUpPrefab;
    public GameObject wallDownPrefab; 
    public GameObject wallLeftPrefab; 
    public GameObject wallRightPrefab; 

    [Header("MonsterRoom 关联")]
    public Map_MonsterRoomController monsterRoomController;

    /// Called from the outside (by the map generator / test script):
    /// If there is a connection in that direction, a door will be generated at that DoorPoint; otherwise, a wall will be generated.
    public void SetupDoors(bool up, bool down, bool left, bool right)
    {
        Debug.Log("[Map_Layout] SetupDoors: "
                  + $"U={up} D={down} L={left} R={right}");

        List<Map_DoorToggle> doorList = new List<Map_DoorToggle>();

        // Up
        SetupOneDirection(
            hasConnection: up,
            point: doorPointUp,
            gatePrefab: gatePrefab,
            wallPrefab: wallUpPrefab,
            doorList: doorList);

        // Down
        SetupOneDirection(
            hasConnection: down,
            point: doorPointDown,
            gatePrefab: gatePrefab,
            wallPrefab: wallDownPrefab,
            doorList: doorList);

        // Left
        SetupOneDirection(
            hasConnection: left,
            point: doorPointLeft,
            gatePrefab: gatePrefab,
            wallPrefab: wallLeftPrefab,
            doorList: doorList);

        // Right
        SetupOneDirection(
            hasConnection: right,
            point: doorPointRight,
            gatePrefab: gatePrefab,
            wallPrefab: wallRightPrefab,
            doorList: doorList);

        // Pass the generated door to the monster room controller so that it can uniformly control the opening and closing later.
        if (monsterRoomController != null)
        {
            monsterRoomController.SetDoors(doorList.ToArray());
        }
        else
        {
            Debug.LogWarning("[Map_Layout] monsterRoomController is NULL");
        }
    }

    private void SetupOneDirection(
    bool hasConnection,
    Transform point,
    Map_DoorToggle gatePrefab,
    GameObject wallPrefab,
    List<Map_DoorToggle> doorList)
    {
        if (point == null)
        {
            Debug.LogWarning("[Map_Layout] DoorPoint is NULL, skip.");
            return;
        }

        Vector3 pos = point.position;

        if (hasConnection)
        {
            if (gatePrefab == null)
            {
                Debug.LogWarning("[Map_Layout] gatePrefab is NULL, cannot spawn gate.");
                return;
            }

            bool isUpDown = (point == doorPointUp || point == doorPointDown);
            Quaternion rot = isUpDown ? Quaternion.Euler(0, 0, 90f) : Quaternion.identity;

            Map_DoorToggle gate = Instantiate(gatePrefab, pos, rot, transform);

            doorList.Add(gate);
            Debug.Log("[Map_Layout] Spawn Gate at " + point.name + " rot=" + rot.eulerAngles);
        }
        else
        {
            if (wallPrefab == null)
            {
                Debug.LogWarning("[Map_Layout] wallPrefab is NULL for " + point.name);
                return;
            }

            Instantiate(wallPrefab, pos, Quaternion.identity, transform);

            Debug.Log("[Map_Layout] Spawn Wall at " + point.name);
        }
    }


}
