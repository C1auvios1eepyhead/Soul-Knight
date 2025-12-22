using System.Diagnostics;
using UnityEngine;
using Pathfinding;

public class Map_DoorToggle : MonoBehaviour
{
    public GameObject doorOpenLayer;   // Tilemap_Door_Open
    public GameObject doorCloseLayer;  // Tilemap_Door_Close

    private bool isClosed = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorCloseSFX;

    private void Start()
    {
        OpenDoor();

    }

    public void CloseDoor()
    {
        if (isClosed) return; //  防止重复播放

        if (doorOpenLayer != null) doorOpenLayer.SetActive(false);
        if (doorCloseLayer != null) doorCloseLayer.SetActive(true);

        isClosed = true;

        // 播放机关门音效
        if (audioSource != null && doorCloseSFX != null)
        {
            audioSource.PlayOneShot(doorCloseSFX, 1.5f);
        }

        ScanGraph1();
    }


    public void OpenDoor()
    {
        if (!isClosed) return; //  防止重复播放

        if (doorOpenLayer != null) doorOpenLayer.SetActive(true);
        if (doorCloseLayer != null) doorCloseLayer.SetActive(false);

        isClosed = false;

        //  播放开门音效（复用石门音）
        if (audioSource != null && doorCloseSFX != null)
        {
            audioSource.PlayOneShot(doorCloseSFX, 1.5f);
        }
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

