using UnityEngine;
using System.Collections.Generic;

public enum WeaponSoundType
{
    GunFire,
    MeleeAttack,
    Reload,
    WeaponSwitch,
    WeaponPickup
}

public class Weapon_SoundManager : MonoBehaviour
{
    public static Weapon_SoundManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Global Volume")]
    [Range(0f, 1f)]
    public float masterVolume = 1f; // 全局音量

    [Header("Weapon Sounds")]
    public AudioClip gunFireClip;
    public AudioClip meleeAttackClip;
    public AudioClip reloadClip;
    public AudioClip weaponSwitchClip;
    public AudioClip weaponPickupClip;

    private Dictionary<WeaponSoundType, AudioClip> soundMap;

    private void Awake()
    {
        // 单例
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        soundMap = new Dictionary<WeaponSoundType, AudioClip>
        {
            { WeaponSoundType.GunFire, gunFireClip },
            { WeaponSoundType.MeleeAttack, meleeAttackClip },
            { WeaponSoundType.Reload, reloadClip },
            { WeaponSoundType.WeaponSwitch, weaponSwitchClip },
            { WeaponSoundType.WeaponPickup, weaponPickupClip }
        };
    }

    /// <summary>
    /// 播放音效，可单独指定音量（0~1），同时受 masterVolume 影响
    /// </summary>
    public void PlaySound(WeaponSoundType type, float volumeScale = 1f)
    {
        if (!soundMap.ContainsKey(type) || soundMap[type] == null) return;

        // 最终音量 = masterVolume * volumeScale
        float finalVolume = Mathf.Clamp01(masterVolume * volumeScale);
        audioSource.PlayOneShot(soundMap[type], finalVolume);
    }
}
