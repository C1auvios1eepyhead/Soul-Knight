using UnityEngine;

public class PlayerHandAnchor : MonoBehaviour
{
    public static Transform HandPoint;

    void Awake()
    {
        
        GameObject hand = new GameObject("HandPoint");
        hand.transform.SetParent(transform);
        hand.transform.localPosition = new Vector3(0.4f, -0.1f, 0); // 玩家手的位置偏移
        hand.transform.localRotation = Quaternion.identity;

        HandPoint = hand.transform;
    }
}
