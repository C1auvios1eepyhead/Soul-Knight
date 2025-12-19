using System.Collections;
using UnityEngine;

public class Map_PlayerRoomEnterGuard : MonoBehaviour
{
    public bool BlockRoomTrigger { get; private set; }

    public void BlockForSeconds(float seconds)
    {
        StopAllCoroutines();
        StartCoroutine(CoBlock(seconds));
    }

    private IEnumerator CoBlock(float seconds)
    {
        BlockRoomTrigger = true;
        yield return new WaitForSeconds(seconds);
        BlockRoomTrigger = false;
    }
}
