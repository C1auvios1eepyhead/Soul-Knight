using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map_MiniMapController : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private GameObject miniMapRoot;      
    [SerializeField] private RectTransform container;     // MiniMapContainer
    [SerializeField] private Image roomIconPrefab;        // RoomIconPrefab（disabled）
    [SerializeField] private Image linkPrefab;            // LinkPrefab（disabled）
    [SerializeField] private RectTransform playerMarker;  // PlayerIcon
    [SerializeField] private RectTransform highlightFrame;// HighlightFrame（enabled）

    [Header("Room Sprites (optional)")]
    [SerializeField] private Sprite startSprite;
    [SerializeField] private Sprite monsterSprite;
    [SerializeField] private Sprite portalSprite;
    [SerializeField] private Sprite bossSprite;

    [Header("Layout")]
    [SerializeField] private float cellSize = 28f;        // 房间间距
    [SerializeField] private Vector2 padding = new Vector2(10, 10);
    [SerializeField] private float linkThickness = 6f;    // 连接线厚度

    [Header("Toggle")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] private bool startVisible = true;

    private readonly Dictionary<Vector2Int, Image> roomIcons = new();
    private readonly Dictionary<Vector2Int, Vector3> roomWorldCenters = new();
    private readonly List<Image> links = new();

    private Vector2Int minCell, maxCell;
    private bool hasData = false;

    private void Awake()
    {
        if (miniMapRoot == null) miniMapRoot = gameObject; 
        miniMapRoot.SetActive(startVisible);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            // 只切 miniMapRoot
            miniMapRoot.SetActive(!miniMapRoot.activeSelf);
        }

        if (!miniMapRoot.activeSelf) return;
        if (!hasData) return;

        UpdatePlayerAndHighlight();
    }

    public void Build(List<Map_LevelGenerator.MiniMapRoomInfo> rooms)
    {
        ClearAll();
        hasData = false;
        if (rooms == null || rooms.Count == 0) return;

        minCell = rooms[0].cell;
        maxCell = rooms[0].cell;

        foreach (var r in rooms)
        {
            minCell = Vector2Int.Min(minCell, r.cell);
            maxCell = Vector2Int.Max(maxCell, r.cell);
        }

        // 生成房间点
        foreach (var r in rooms)
        {
            var img = Instantiate(roomIconPrefab, container);
            img.gameObject.SetActive(true);
            img.raycastTarget = false;

            img.sprite = SpriteByType(r.type);
            img.rectTransform.anchoredPosition = CellToUI(r.cell);

            roomIcons[r.cell] = img;
            roomWorldCenters[r.cell] = r.worldCenter;
        }

        // 生成连线
        foreach (var cell in roomIcons.Keys)
        {
            TryCreateLink(cell, cell + Vector2Int.right);
            TryCreateLink(cell, cell + Vector2Int.up);
        }

        hasData = true;
        UpdatePlayerAndHighlight(); // 立即刷新一次
    }

    private void TryCreateLink(Vector2Int a, Vector2Int b)
    {
        if (!roomIcons.ContainsKey(a) || !roomIcons.ContainsKey(b)) return;

        Vector2 pa = CellToUI(a);
        Vector2 pb = CellToUI(b);
        Vector2 mid = (pa + pb) * 0.5f;

        var img = Instantiate(linkPrefab, container);
        img.gameObject.SetActive(true);
        img.raycastTarget = false;

        RectTransform rt = img.rectTransform;
        rt.anchoredPosition = mid;

        bool horizontal = (b.x != a.x);
        float length = cellSize;

        rt.sizeDelta = horizontal
            ? new Vector2(length, linkThickness)
            : new Vector2(linkThickness, length);

        links.Add(img);
    }

    private void UpdatePlayerAndHighlight()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // 找最近房间
        float best = float.MaxValue;
        Vector2Int bestCell = default;
        bool found = false;

        foreach (var kv in roomWorldCenters)
        {
            float d = (kv.Value - player.transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                bestCell = kv.Key;
                found = true;
            }
        }

        if (!found) return;

        Vector2 uiPos = CellToUI(bestCell);

        if (playerMarker != null)
            playerMarker.anchoredPosition = uiPos;

        if (highlightFrame != null)
            highlightFrame.anchoredPosition = uiPos;
    }

    private Vector2 CellToUI(Vector2Int cell)
    {
        int x = cell.x - minCell.x;
        int y = cell.y - minCell.y;
        return new Vector2(padding.x + x * cellSize, padding.y + y * cellSize);
    }

    private Sprite SpriteByType(Map_LevelGenerator.RoomType type)
    {
        switch (type)
        {
            case Map_LevelGenerator.RoomType.Start: return startSprite ? startSprite : monsterSprite;
            case Map_LevelGenerator.RoomType.Portal: return portalSprite ? portalSprite : monsterSprite;
            case Map_LevelGenerator.RoomType.Boss: return bossSprite ? bossSprite : monsterSprite;
            default: return monsterSprite;
        }
    }

    private void ClearAll()
    {
        foreach (var kv in roomIcons) if (kv.Value) Destroy(kv.Value.gameObject);
        roomIcons.Clear();
        roomWorldCenters.Clear();

        foreach (var l in links) if (l) Destroy(l.gameObject);
        links.Clear();
    }
}
