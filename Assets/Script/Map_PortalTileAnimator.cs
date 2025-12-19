using UnityEngine;
using UnityEngine.Tilemaps;

public class PortalTileAnimator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Tilemap tilemap;

    [Header("Layout (6 cells)")]
    [SerializeField] private Vector3Int originCell = Vector3Int.zero;
    [SerializeField] private Vector3Int[] cells = new Vector3Int[6];

    [Header("Frames (4 x 6 tiles)")]
    [SerializeField] private TileBase[] frame0 = new TileBase[6];
    [SerializeField] private TileBase[] frame1 = new TileBase[6];
    [SerializeField] private TileBase[] frame2 = new TileBase[6];
    [SerializeField] private TileBase[] frame3 = new TileBase[6];

    [Header("Timing")]
    [SerializeField] private float fps = 6f;

    private float timer;
    private int frame;

    private void Reset()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void Update()
    {
        if (tilemap == null) return;
        if (cells == null || cells.Length != 6) return;

        timer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, fps);

        if (timer >= interval)
        {
            timer -= interval;
            frame = (frame + 1) % 4;
            ApplyFrame(frame);
        }
    }

    private void ApplyFrame(int f)
    {
        TileBase[] src = f switch
        {
            0 => frame0,
            1 => frame1,
            2 => frame2,
            _ => frame3
        };

        if (src == null || src.Length != 6) return;

        for (int i = 0; i < 6; i++)
        {
            Vector3Int cellPos = originCell + cells[i];
            tilemap.SetTile(cellPos, src[i]);

            if (i == 0) Debug.Log($"[PortalAnim] Writing at cell {cellPos} on tilemap {tilemap.name}");
        }
    }
}
