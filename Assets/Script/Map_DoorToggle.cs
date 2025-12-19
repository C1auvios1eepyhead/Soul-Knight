using System.Diagnostics;
using UnityEngine;
using Pathfinding;

public class Map_DoorToggle : MonoBehaviour
{
    public GameObject doorOpenLayer;   // Tilemap_Door_Open
    public GameObject doorCloseLayer;  // Tilemap_Door_Close

    private bool isClosed = false;


    private void Start()
    {
        OpenDoor();

    }

    public void CloseDoor()
    {
        if (doorOpenLayer != null) doorOpenLayer.SetActive(false);
        if (doorCloseLayer != null) doorCloseLayer.SetActive(true);
        isClosed = true;
        ScanGraph1();
    


    }

    public void OpenDoor()
    {
        if (doorOpenLayer != null) doorOpenLayer.SetActive(true);
        if (doorCloseLayer != null) doorCloseLayer.SetActive(false);
        isClosed = false;
    }

    public bool IsClosed() => isClosed;

    public void ScanGraph1()
    {
        if (AstarPath.active == null)
        {
            UnityEngine.Debug.LogError("No AstarPath instance found in the scene!");
            return;
        }

        AstarPath.active.Scan();  // 扫描所有图
        UnityEngine.Debug.Log("Pathfinding graph scanned!");
    }
}

