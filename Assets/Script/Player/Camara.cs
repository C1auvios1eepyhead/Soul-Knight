using UnityEngine;

public class CanvasCameraBinder : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();

        Camera playerCamera = FindObjectOfType<Camera>();

        if (canvas != null && playerCamera != null)
        {
            canvas.worldCamera = playerCamera;
        }
        else
        {
            Debug.LogError("Canvas or Player Camera not found!");
        }
    }
}
