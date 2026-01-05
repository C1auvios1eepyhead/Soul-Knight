using UnityEngine;

public class Map_PotionPickup : MonoBehaviour
{
    public enum PotionType
    {
        Red_Heal2,
        Green_MaxHPPlus1,
        White_MaxArmorPlus1,
        Blue_SpeedPlus10
    }

    [Header("Potion")]
    public PotionType potionType;

    [Header("Audio")]
    public AudioClip pickupSFX;
    [Range(0f, 1f)] public float volume = 0.8f;

    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;

    private bool canPickup = false;
    private GameObject playerObj;

    private void Update()
    {
        if (!canPickup) return;

        if (Input.GetKeyDown(interactKey))
        {
            TryPickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        canPickup = true;
        playerObj = other.gameObject;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerObj == null) return;
        if (other.gameObject != playerObj) return;

        canPickup = false;
        playerObj = null;

        // 可选：隐藏提示
    }

    private void TryPickup()
    {
        var playerAS = playerObj.GetComponent<AudioSource>();
        if (playerAS == null) playerAS = playerObj.AddComponent<AudioSource>();

        // 关键：强制2D
        playerAS.spatialBlend = 0f;
        playerAS.PlayOneShot(pickupSFX, volume * 3f);

        var health = playerObj.GetComponent<PlayerHealth>();
        var move = playerObj.GetComponent<PlayerMovement>();
        ApplyEffect(health, move);

        Destroy(gameObject);
    }

    private void ApplyEffect(PlayerHealth health, PlayerMovement move)
    {
        switch (potionType)
        {
            case PotionType.Red_Heal2:
                if (health != null) health.RecoverHealth(2f);
                break;

            case PotionType.Green_MaxHPPlus1:
                if (health != null) health.AddMaxHealth(1, refillToNewMax: true);
                break;

            case PotionType.White_MaxArmorPlus1:
                if (health != null) health.AddMaxArmor(1, refillToNewMax: true);
                break;

            case PotionType.Blue_SpeedPlus10:
                if (move != null) move.MultiplySpeed(1.10f);
                break;
        }
    }
}
