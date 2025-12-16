using UnityEngine;
using UnityEngine.Tilemaps;

public class Map_TestAttack : MonoBehaviour
{
    private Collider2D attackCol;

    private void Awake()
    {
        attackCol = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) => TryDestroyTiles(other);
    private void OnTriggerStay2D(Collider2D other) => TryDestroyTiles(other);

    private void TryDestroyTiles(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("DestructibleTile"))
            return;

        Tilemap tilemap = other.GetComponent<Tilemap>();
        if (tilemap == null || attackCol == null) return;

        Bounds a = attackCol.bounds;
        Vector3Int minCell = tilemap.WorldToCell(a.min);
        Vector3Int maxCell = tilemap.WorldToCell(a.max);

        // Iterate through the range of tiles. Delete any tile that has a "boundary intersection".
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (!tilemap.HasTile(cell)) continue;

                Bounds cellBounds = GetCellWorldBounds(tilemap, cell);

                // Even if only a little bit is applied, the bounds will still intersect.
                if (a.Intersects(cellBounds))
                {
                    tilemap.SetTile(cell, null);
                }
            }
        }
    }

    // Calculate the world rectangle bounds of a certain tile cell
    private Bounds GetCellWorldBounds(Tilemap tilemap, Vector3Int cell)
    {
        Vector3 center = tilemap.GetCellCenterWorld(cell);

        // "cellSize" is a unit of the grid. It will be automatically scaled according to the transform of the tilemap.
        Vector3 size = tilemap.cellSize;

        // If Tilemap is scaled, using lossyScale here will be more reliable.
        Vector3 scale = tilemap.transform.lossyScale;
        size = new Vector3(size.x * Mathf.Abs(scale.x), size.y * Mathf.Abs(scale.y), 0.01f);

        return new Bounds(center, size);
    }
}
