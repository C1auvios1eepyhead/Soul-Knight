using System.Collections.Generic;
using UnityEngine;

public class Map_DungeonGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 5; 
    public int height = 5;

    [Header("Room Settings")]
    public GameObject roomPrefab; 
    public float cellSizeX = 20f;
    public float cellSizeY = 20f;

    [Header("Random Walk Settings")]
    public int walkSteps = 8; 

    private bool[,] hasRoom;

    void Start()
    {
        GenerateLayout();
        BuildDungeon();
    }

    void GenerateLayout()
    {
        hasRoom = new bool[width, height];

        int x = width / 2;
        int y = height / 2;
        hasRoom[x, y] = true;

        System.Random rand = new System.Random();

        for (int i = 0; i < walkSteps; i++)
        {
            int dir = rand.Next(4);

            switch (dir)
            {
                case 0: y += 1; break;
                case 1: y -= 1; break;
                case 2: x -= 1; break;
                case 3: x += 1; break;
            }

            // boundary protection
            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);

            hasRoom[x, y] = true;
        }
    }

    void BuildDungeon()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!hasRoom[x, y]) continue;

                Vector3 pos = new Vector3(
                    x * cellSizeX,
                    y * cellSizeY,
                    0f
                );
                GameObject roomObj = Instantiate(roomPrefab, pos, Quaternion.identity, transform);

                Map_Layout layout = roomObj.GetComponent<Map_Layout>();
                if (layout == null) continue;

                // Check if there are any other rooms in the four directions.
                bool up = IsRoom(x, y + 1);
                bool down = IsRoom(x, y - 1);
                bool left = IsRoom(x - 1, y);
                bool right = IsRoom(x + 1, y);

                // Tell the room which directions are the doors and which directions are the walls.
                layout.SetupDoors(up, down, left, right);
            }
        }
    }

    bool IsRoom(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return hasRoom[x, y];
    }
}
