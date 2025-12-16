using UnityEngine;

public class Map_SpikeTrapDamage : MonoBehaviour
{
    public Map_SpikeTrapController spikeTrap;
    public float damage = 1f;

    private void Awake()
    {
        if (spikeTrap == null)
            spikeTrap = GetComponentInParent<Map_SpikeTrapController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var hp = other.GetComponent<PlayerHealth>();
        if (hp == null) return;

        // It is uniformly decided by the Controller whether to deduct blood (including the judgment of extension + cooldown).
        spikeTrap.TryDamage(hp, damage);
    }
}
