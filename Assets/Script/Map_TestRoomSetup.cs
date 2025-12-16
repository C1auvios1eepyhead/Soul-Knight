using UnityEngine;

public class Map_TestRoomSetup : MonoBehaviour
{
    public Map_Layout layout;

    void Start()
    {
        if (layout == null)
        {
            layout = GetComponent<Map_Layout>();
        }

        if (layout == null)
        {
            Debug.LogError("[Map_TestRoomSetup] Map_Layout not found!");
            return;
        }

        // Test
        layout.SetupDoors(
            up: true,
            down: true,
            left: true,
            right: true);
    }
}
