using UnityEngine;

public class Map_PortalInteract : MonoBehaviour
{
    private bool playerInside;

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Map_LevelFlowManager.Instance.EnterNextStage();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) playerInside = false;
    }
}
