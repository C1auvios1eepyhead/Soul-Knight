using System.Collections;
using UnityEngine;

public class Map_SpikeTrapController : MonoBehaviour
{
    [Header("Tilemaps")]
    public GameObject retractTilemap;
    public GameObject extendTilemap;

    [Header("Damage Triggers (4)")]
    public Collider2D[] damageTriggers;

    [Header("Timing")]
    public float cycleSeconds = 3f;
    public float extendSeconds = 1f;

    public bool IsExtended { get; private set; }

    [Header("Damage")]
    public float damageCooldown = 1f;
    private float lastDamageTime = -999f;

    public bool TryDamage(PlayerHealth hp, float damage)
    {
        if (!IsExtended) return false;
        if (hp == null) return false;

        // Global cooldown: Regardless of how many triggers are activated simultaneously, only one amount of health will be deducted.
        if (Time.time - lastDamageTime < damageCooldown) return false;

        hp.TakeDamage(damage);
        lastDamageTime = Time.time;

        Debug.Log($"[SpikeTrap] Damage Player: -{damage}");
        return true;
    }


    void Start()
    {
        SetExtended(false);
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            SetExtended(false);
            yield return new WaitForSeconds(Mathf.Max(0f, cycleSeconds - extendSeconds));

            SetExtended(true);
            yield return new WaitForSeconds(Mathf.Max(0f, extendSeconds));
        }
    }

    void SetExtended(bool extended)
    {
        IsExtended = extended;

        if (retractTilemap) retractTilemap.SetActive(!extended);
        if (extendTilemap) extendTilemap.SetActive(extended);

        // The trigger is only activated when it is extended.
        if (damageTriggers != null)
        {
            foreach (var c in damageTriggers)
            {
                if (c) c.enabled = extended;
            }
        }
    }
}
