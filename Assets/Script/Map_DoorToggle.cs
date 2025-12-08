using UnityEngine;

public class Map_DoorToggle : MonoBehaviour
{
    public GameObject doorOpenLayer;   // Tilemap_Door_Open
    public GameObject doorCloseLayer;  // Tilemap_Door_Close

    bool isClosed = false;

    void Start()
    {
        // 默认开门：开门层开，关门层关
        if (doorOpenLayer != null) doorOpenLayer.SetActive(true);
        if (doorCloseLayer != null) doorCloseLayer.SetActive(false);
        isClosed = false;
    }

    void Update()
    {
        // 这里改成按 G 键测试，避免和 Scene 视图的 F 搞混
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("G pressed, toggle door");   // 看看 Console 有没有这条

            if (isClosed)
                OpenDoor();
            else
                CloseDoor();
        }
    }

    public void CloseDoor()
    {
        if (doorOpenLayer != null) doorOpenLayer.SetActive(false);
        if (doorCloseLayer != null) doorCloseLayer.SetActive(true);
        isClosed = true;
    }

    public void OpenDoor()
    {
        if (doorOpenLayer != null) doorOpenLayer.SetActive(true);
        if (doorCloseLayer != null) doorCloseLayer.SetActive(false);
        isClosed = false;
    }
}
