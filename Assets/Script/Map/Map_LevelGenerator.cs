using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Map_LevelGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 5;
    [SerializeField] private Vector3 worldOrigin = Vector3.zero;

    [Header("Corridor")]
    [SerializeField] private GameObject corridorPrefab;          // Corridor_H
    [SerializeField] private float corridorRotateZForVertical = 90f;

    [Tooltip("Corridor length in world units. When Tile cellSize = 1, set this to the number of tiles (e.g. 15). If the map or prefab is scaled, multiply by the scale factor (e.g. 15 * 0.16).")]
    [SerializeField] private float corridorLengthWorld = 15f;

    [Header("Room Prefabs")]
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject portalRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private GameObject[] monsterRoomPrefabs;

    [Header("Player Spawn")]
    [SerializeField] private string playerTag = "Player";

    [Header("Generate Options")]
    [SerializeField] private int seed = 0;
    [SerializeField] private bool autoGenerateOnStart = false;

    [Header("Debug")]
    [Range(1, 5)]
    [SerializeField] private int debugStageIndex = 1;

    private readonly int[] roomCounts = { 4, 5, 5, 6, 6 };
    private System.Random rng;

    private enum RoomType { Start, Monster, Portal, Boss }
    private enum Dir4 { Up, Down, Left, Right }

    private class RoomNode
    {
        public Vector2Int cell;
        public RoomType type;
        public GameObject go;

        public bool up, down, left, right;

        public int rotStepsCW;
        public bool placed;
    }

    private Dictionary<Vector2Int, RoomNode> nodes = new();
    private HashSet<(Vector2Int, Vector2Int)> spawnedEdges = new();

    // Monster Room does not repeat the draw.
    private List<int> monsterPickPool = new();
    private int monsterPickIndex = 0;

    private void Start()
    {
        if (!autoGenerateOnStart) return;
        Generate(1, debugStageIndex);
    }

    public void Generate(int worldIndex, int stageIndex)
    {
        Debug.Log($"[Generate] worldIndex={worldIndex}, stageIndex={stageIndex}\nCALLER:\n{Environment.StackTrace}");

        ClearExisting();
        ValidateInputs(stageIndex);

        rng = (seed == 0)
            ? new System.Random(Guid.NewGuid().GetHashCode())
            : new System.Random(seed + stageIndex * 1000);

        int totalRooms = roomCounts[stageIndex - 1];
        bool isFinal = (stageIndex == 5);

        BuildMonsterPickPool();

        BuildTopology(totalRooms, isFinal);

        SpawnAndPlaceRooms();

        SpawnCorridorsByAlign();

        ConfigureAllRoomsConnections();

        MovePlayerToStartRoomCenter();

        Map_LevelIndicator.Instance.Show($"{worldIndex}-{stageIndex}");


    }

    // Monster pick pool
    private void BuildMonsterPickPool()
    {
        monsterPickPool.Clear();
        for (int i = 0; i < monsterRoomPrefabs.Length; i++) monsterPickPool.Add(i);

        for (int i = monsterPickPool.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (monsterPickPool[i], monsterPickPool[j]) = (monsterPickPool[j], monsterPickPool[i]);
        }

        monsterPickIndex = 0;
    }

    private GameObject PickMonsterPrefabNoRepeat()
    {
        if (monsterRoomPrefabs == null || monsterRoomPrefabs.Length == 0)
            throw new Exception("MonsterRoomPrefabs is empty.");

        if (monsterPickIndex < monsterPickPool.Count)
        {
            int idx = monsterPickPool[monsterPickIndex++];
            return monsterRoomPrefabs[idx];
        }

        return monsterRoomPrefabs[rng.Next(monsterRoomPrefabs.Length)];
    }

    // Topology
    private void BuildTopology(int totalRooms, bool isFinal)
    {
        nodes.Clear();

        // Fixed startCell: Left-center position
        Vector2Int startCell = new Vector2Int(1, gridSize / 2);
        RoomNode start = new RoomNode { cell = startCell, type = RoomType.Start };
        nodes[startCell] = start;

        // Start room must only be connected to one: fix the first monster room on the right side.
        Vector2Int firstCell = startCell + Vector2Int.right;
        if (!InBounds(firstCell))
        {
            startCell = new Vector2Int(0, gridSize / 2);
            firstCell = startCell + Vector2Int.right;
            nodes.Clear();
            start = new RoomNode { cell = startCell, type = RoomType.Start };
            nodes[startCell] = start;
        }

        RoomNode first = new RoomNode { cell = firstCell, type = RoomType.Monster };
        nodes[firstCell] = first;
        Link(startCell, firstCell);

        List<Vector2Int> frontier = new List<Vector2Int> { firstCell };
        int guard = 20000;

        while (nodes.Count < totalRooms && guard-- > 0)
        {
            Vector2Int baseCell = frontier[rng.Next(frontier.Count)];
            Vector2Int nextCell = baseCell + RandomDirVec();

            if (!InBounds(nextCell)) continue;
            if (nodes.ContainsKey(nextCell)) continue;

            nodes[nextCell] = new RoomNode { cell = nextCell, type = RoomType.Monster };
            frontier.Add(nextCell);
            Link(baseCell, nextCell);
        }

        if (nodes.Count < totalRooms)
        {
            BuildTopology(totalRooms, isFinal);
            return;
        }

        List<Vector2Int> leaf = new();
        foreach (var kv in nodes)
        {
            var c = kv.Key;
            if (c == startCell) continue;

            int deg = Degree(nodes[c]);
            if (deg != 1) continue;

            int manhattan = Mathf.Abs(c.x - startCell.x) + Mathf.Abs(c.y - startCell.y);
            if (manhattan < 2) continue;

            leaf.Add(c);
        }

        if (leaf.Count == 0)
        {
            BuildTopology(totalRooms, isFinal);
            return;
        }

        leaf.Sort((a, b) =>
        {
            int da = Mathf.Abs(a.x - startCell.x) + Mathf.Abs(a.y - startCell.y);
            int db = Mathf.Abs(b.x - startCell.x) + Mathf.Abs(b.y - startCell.y);
            return db.CompareTo(da);
        });

        Vector2Int specialCell = leaf[0];
        nodes[specialCell].type = isFinal ? RoomType.Boss : RoomType.Portal;

    }

    private void Link(Vector2Int a, Vector2Int b)
    {
        RoomNode A = nodes[a];
        RoomNode B = nodes[b];

        Vector2Int d = b - a;
        if (d == Vector2Int.up) { A.up = true; B.down = true; }
        else if (d == Vector2Int.down) { A.down = true; B.up = true; }
        else if (d == Vector2Int.left) { A.left = true; B.right = true; }
        else if (d == Vector2Int.right) { A.right = true; B.left = true; }
    }

    private int Degree(RoomNode n)
    {
        int deg = 0;
        if (n.up) deg++;
        if (n.down) deg++;
        if (n.left) deg++;
        if (n.right) deg++;
        return deg;
    }

    //Spawn & Place 
    private void SpawnAndPlaceRooms()
    {
        foreach (var kv in nodes)
        {
            RoomNode n = kv.Value;
            GameObject prefab = PickPrefab(n.type);
            n.go = Instantiate(prefab, worldOrigin, Quaternion.identity, transform);
            n.go.name = $"{n.type}_({n.cell.x},{n.cell.y})";
            n.rotStepsCW = 0;
            n.placed = false;
        }

        Vector2Int startCell = FindCellByType(RoomType.Start);

        PlaceRoomAt(nodes[startCell], worldOrigin, 0);
        RotateSingleDoorRoomToMatchConnection(nodes[startCell], forcedConnDir: GetOnlyConnectionDir(nodes[startCell]));
        nodes[startCell].placed = true;

        var q = new Queue<Vector2Int>();
        q.Enqueue(startCell);

        while (q.Count > 0)
        {
            var curCell = q.Dequeue();
            RoomNode cur = nodes[curCell];

            foreach (var (nextCell, dirFromCurToNext) in GetNeighbors(cur))
            {
                if (!nodes.ContainsKey(nextCell)) continue;
                RoomNode next = nodes[nextCell];
                if (next.placed) continue;

                PlaceRoomByAlign(cur, next, dirFromCurToNext);

                next.placed = true;
                q.Enqueue(nextCell);
            }
        }
       
    }

    private GameObject PickPrefab(RoomType type)
    {
        if (type == RoomType.Start) return startRoomPrefab;
        if (type == RoomType.Portal) return portalRoomPrefab;
        if (type == RoomType.Boss) return bossRoomPrefab;
        return PickMonsterPrefabNoRepeat();
    }

    private Vector2Int FindCellByType(RoomType type)
    {
        foreach (var kv in nodes)
            if (kv.Value.type == type) return kv.Key;
        throw new Exception($"RoomType not found: {type}");
    }

    private IEnumerable<(Vector2Int, Dir4)> GetNeighbors(RoomNode n)
    {
        if (n.up) yield return (n.cell + Vector2Int.up, Dir4.Up);
        if (n.down) yield return (n.cell + Vector2Int.down, Dir4.Down);
        if (n.left) yield return (n.cell + Vector2Int.left, Dir4.Left);
        if (n.right) yield return (n.cell + Vector2Int.right, Dir4.Right);
    }

    private Transform GetRotRoot(GameObject room)
    {
        var t = FindAny(room.transform, "RotRoot");
        return t != null ? t : room.transform; // If there is no response, the entire room should be returned.
    }

    private void PlaceRoomAt(RoomNode n, Vector3 worldPos, int rotStepsCW)
    {
        n.rotStepsCW = ((rotStepsCW % 4) + 4) % 4;
        n.go.transform.position = worldPos;

        Transform rotRoot = GetRotRoot(n.go);
        rotRoot.rotation = Quaternion.Euler(0, 0, -90f * n.rotStepsCW);

        // The Portal does not rotate, but its position always follows the anchor point.
        if (n.type == RoomType.Portal)
        {
            Transform anchor = FindAny(n.go.transform, "PortalAnchor");
            Transform portal = FindAny(n.go.transform, "Portal");
            if (anchor != null && portal != null)
            {
                portal.position = anchor.position; 

                portal.rotation = Quaternion.identity;
            }
        }
    }

    /// Use DoorAlign (alignment points) to calculate the inset and ensure that the center line of the corridor is aligned.
    /// Start/Portal：There is only one door (default on the right), which can be rotated to align with the connection direction.
    /// Boss：Rotation is not allowed (rot is set to 0), and the door direction is determined by topology (SetupDoors controls which side of the door to open).
    private void PlaceRoomByAlign(RoomNode from, RoomNode to, Dir4 dirFromTo)
    {
        Vector3 dir = DirToWorldVector(dirFromTo);

        // First, make sure that if "from" is "Start/Portal", then its "right door" is already aligned with its only connection direction.
        if (from.type == RoomType.Start || from.type == RoomType.Portal)
        {
            RotateSingleDoorRoomToMatchConnection(from, forcedConnDir: GetOnlyConnectionDir(from));
        }

        Transform fromCenter = FindAny(from.go.transform, "RoomCenter");
        Transform fromAlign = GetAlign(from.go, dirFromTo);
        if (fromCenter == null || fromAlign == null)
            throw new Exception($"[PlaceRoomByAlign] {from.go.name} missing RoomCenter or DoorAlign (dir={dirFromTo})");

        List<int> rotCandidates = new();

        if (to.type == RoomType.Boss)
        {
            rotCandidates.Add(0); // The boss room does not rotate.
        }
        else if (to.type == RoomType.Start || to.type == RoomType.Portal)
        {
            // There is only one door and it is set to be on the right by default. It must be made to "face towards from".
            Dir4 toConnDir = Opposite(dirFromTo); // to -> from
            int need = RotationStepsToMap(Dir4.Right, toConnDir);
            rotCandidates.Add(need);
        }
        else
        {
            rotCandidates.Add(0);
            rotCandidates.Add(1);
            rotCandidates.Add(2);
            rotCandidates.Add(3);
        }

        float bestErr = float.MaxValue;
        int bestRot = 0;
        Vector3 bestPos = worldOrigin;

        Vector3 basePos = worldOrigin;

        foreach (int rot in rotCandidates)
        {
            PlaceRoomAt(to, basePos, rot);

            Transform toCenter = FindAny(to.go.transform, "RoomCenter");
            Transform toAlign = GetAlign(to.go, Opposite(dirFromTo));
            if (toCenter == null || toAlign == null) continue;

            float insetFrom = Vector3.Dot(fromAlign.position - fromCenter.position, dir);
            float insetTo = Vector3.Dot(toAlign.position - toCenter.position, -dir);

            // center to center = insetFrom + corridor + insetTo
            Vector3 targetToCenter = fromCenter.position + dir * (insetFrom + corridorLengthWorld + insetTo);

            Vector3 deltaCenter = targetToCenter - toCenter.position;
            to.go.transform.position += deltaCenter;

            Vector3 expectedToAlign = fromAlign.position + dir * corridorLengthWorld;
            float err = (expectedToAlign - toAlign.position).sqrMagnitude;

            if (err < bestErr)
            {
                bestErr = err;
                bestRot = rot;
                bestPos = to.go.transform.position;
            }
        }

        PlaceRoomAt(to, bestPos, bestRot);

        // If "to" is "Start/Portal", then force it again (to avoid being overwritten by the "rotCandidates" of the Monster)
        if (to.type == RoomType.Start || to.type == RoomType.Portal)
        {
            RotateSingleDoorRoomToMatchConnection(to, forcedConnDir: GetOnlyConnectionDir(to));
        }
    }

    // Single door rotate rule (Start/Portal only)
    private void RotateSingleDoorRoomToMatchConnection(RoomNode n, Dir4 forcedConnDir)
    {
        // Only the Start/Portal can rotate. The Boss does not rotate.
        if (n.type != RoomType.Start && n.type != RoomType.Portal) return;

        if (Degree(n) != 1) return;

        // Rotate the right door to align with the connection direction.
        int steps = RotationStepsToMap(Dir4.Right, forcedConnDir);
        PlaceRoomAt(n, n.go.transform.position, steps);
    }

    private Dir4 GetOnlyConnectionDir(RoomNode n)
    {
        if (n.up) return Dir4.Up;
        if (n.down) return Dir4.Down;
        if (n.left) return Dir4.Left;
        return Dir4.Right;
    }

    private int RotationStepsToMap(Dir4 from, Dir4 to)
    {
        int a = DirIndex(from);
        int b = DirIndex(to);
        return (b - a + 4) % 4;
    }

    private int DirIndex(Dir4 d) => d switch
    {
        Dir4.Up => 0,
        Dir4.Right => 1,
        Dir4.Down => 2,
        Dir4.Left => 3,
        _ => 0
    };

    private Dir4 Opposite(Dir4 d) => d switch
    {
        Dir4.Up => Dir4.Down,
        Dir4.Down => Dir4.Up,
        Dir4.Left => Dir4.Right,
        Dir4.Right => Dir4.Left,
        _ => Dir4.Up
    };

    private Vector3 DirToWorldVector(Dir4 d) => d switch
    {
        Dir4.Up => Vector3.up,
        Dir4.Down => Vector3.down,
        Dir4.Left => Vector3.left,
        Dir4.Right => Vector3.right,
        _ => Vector3.right
    };

    // Align finder
    private Transform GetAlign(GameObject room, Dir4 logicalDir)
    {
        string alignName = logicalDir switch
        {
            Dir4.Up => "DoorAlign_Up",
            Dir4.Down => "DoorAlign_Down",
            Dir4.Left => "DoorAlign_Left",
            Dir4.Right => "DoorAlign_Right",
            _ => "DoorAlign_Right"
        };

        Transform t = FindAny(room.transform, alignName);
        if (t != null) return t;

        t = FindAny(room.transform, "DoorAlign_Right");
        if (t != null) return t;

        string doorPoint = logicalDir switch
        {
            Dir4.Up => "DoorPoint_Up",
            Dir4.Down => "DoorPoint_Down",
            Dir4.Left => "DoorPoint_Left",
            Dir4.Right => "DoorPoint_Right",
            _ => "DoorPoint_Right"
        };
        t = FindAny(room.transform, doorPoint);
        if (t != null) return t;

        return null;
    }

    private Transform FindAny(Transform root, string childName)
    {
        var t = root.Find(childName);
        if (t != null) return t;

        foreach (Transform tr in root.GetComponentsInChildren<Transform>(true))
            if (tr.name == childName) return tr;

        return null;
    }

    // Corridors (Align-to-Align)
    private void SpawnCorridorsByAlign()
    {
        if (corridorPrefab == null) return;

        spawnedEdges.Clear();

        foreach (var kv in nodes)
        {
            Vector2Int a = kv.Key;
            TrySpawnCorridorByAlign(a, a + Vector2Int.right, Dir4.Right);
            TrySpawnCorridorByAlign(a, a + Vector2Int.up, Dir4.Up);
        }
    }

    private void TrySpawnCorridorByAlign(Vector2Int a, Vector2Int b, Dir4 dirAtoB)
    {
        if (!nodes.ContainsKey(a) || !nodes.ContainsKey(b)) return;

        var edge = NormalizeEdge(a, b);
        if (spawnedEdges.Contains(edge)) return;

        RoomNode A = nodes[a];
        RoomNode B = nodes[b];

        if (dirAtoB == Dir4.Right && !(A.right && B.left)) return;
        if (dirAtoB == Dir4.Up && !(A.up && B.down)) return;

        spawnedEdges.Add(edge);

        Transform alignA = GetAlign(A.go, dirAtoB);
        Transform alignB = GetAlign(B.go, Opposite(dirAtoB));
        if (alignA == null || alignB == null) return;

        Vector3 mid = (alignA.position + alignB.position) * 0.5f;

        bool vertical = (dirAtoB == Dir4.Up);
        Quaternion rot = vertical ? Quaternion.Euler(0, 0, corridorRotateZForVertical) : Quaternion.identity;

        GameObject go = Instantiate(corridorPrefab, mid, rot, transform);
        go.name = $"Corridor_({a.x},{a.y})_to_({b.x},{b.y})";
    }

    private (Vector2Int, Vector2Int) NormalizeEdge(Vector2Int a, Vector2Int b)
    {
        if (a.x < b.x) return (a, b);
        if (a.x > b.x) return (b, a);
        if (a.y < b.y) return (a, b);
        return (b, a);
    }

    // Door setup 
    private void ConfigureAllRoomsConnections()
    {
        foreach (var kv in nodes)
        {
            RoomNode n = kv.Value;
            if (n.go == null) continue;

            var layout = n.go.GetComponent<Map_Layout>();
            if (layout != null)
            {
                layout.SetupDoors(n.up, n.down, n.left, n.right);
            }
        }
    }

    // Player Spawn
    private void MovePlayerToStartRoomCenter()
    {
        StartCoroutine(CoMovePlayerNextFrame());
        ScanGraph();
    }

    private IEnumerator CoMovePlayerNextFrame()
    {
        // 先拿到 player（必须在使用 player 之前声明）
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) yield break;

        // 立刻开启“进房触发保护”，覆盖这一帧（防止生成点重叠触发战斗）
        var guard = player.GetComponent<Map_PlayerRoomEnterGuard>();
        guard?.BlockForSeconds(0.5f);

        // 你原本就在这里等一帧
        yield return null;
        Physics2D.SyncTransforms();

        Vector2Int startCell = FindCellByType(RoomType.Start);
        RoomNode start = nodes[startCell];

        Transform center = FindAny(start.go.transform, "RoomCenter");
        Vector3 targetPos = center != null ? center.position : start.go.transform.position;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            bool oldSim = rb.simulated;
            rb.simulated = false;
            player.transform.position = targetPos;
            rb.velocity = Vector2.zero;
            rb.simulated = oldSim;
        }
        else
        {
            player.transform.position = targetPos;
        }

        // 移动到起点后再续一小段保护，防止 SyncTransforms 后立即触发
        guard?.BlockForSeconds(0.35f);
        ScanGraph();
    }

    //Utility
    private void ValidateInputs(int stageIndex)
    {
        if (startRoomPrefab == null) throw new Exception("StartRoomPrefab not assigned.");
        if (monsterRoomPrefabs == null || monsterRoomPrefabs.Length == 0) throw new Exception("MonsterRoomPrefabs empty.");
        if (stageIndex <= 4 && portalRoomPrefab == null) throw new Exception("PortalRoomPrefab not assigned.");
        if (stageIndex == 5 && bossRoomPrefab == null) throw new Exception("BossRoomPrefab not assigned.");
    }

    private void ClearExisting()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        nodes.Clear();
        spawnedEdges.Clear();
        monsterPickPool.Clear();
        monsterPickIndex = 0;
    }

    private bool InBounds(Vector2Int c)
        => c.x >= 0 && c.y >= 0 && c.x < gridSize && c.y < gridSize;

    private Vector2Int RandomDirVec()
    {
        int r = rng.Next(4);
        return r switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.down,
            2 => Vector2Int.left,
            _ => Vector2Int.right
        };
    }
    public void ScanGraph()
    {
        if (AstarPath.active == null)
        {
            Debug.LogError("No AstarPath instance found in the scene!");
            return;
        }

        AstarPath.active.Scan();  // 扫描所有图
        Debug.Log("Pathfinding graph scanned!");
    }

}


