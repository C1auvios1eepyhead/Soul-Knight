using UnityEngine;

public class Map_AcceleratingZone : MonoBehaviour
{
    [SerializeField] private float multiplier = 1.5f; 
    [SerializeField] private float duration = 3f;

    private void OnTriggerEnter2D(Collider2D other) => Apply(other);

    private void OnTriggerStay2D(Collider2D other)
    {
        Apply(other);
    }

    private void Apply(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var buff = other.GetComponent<Map_PlayerSpeedBoost>();
        if (buff == null) buff = other.gameObject.AddComponent<Map_PlayerSpeedBoost>();

        buff.Refresh(multiplier, duration);
    }
}
