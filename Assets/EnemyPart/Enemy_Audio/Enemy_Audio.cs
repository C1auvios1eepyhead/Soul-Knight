using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip attackClip;
    public AudioClip hurtClip;

    public AudioClip deadClip;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // 动画事件调用
    public void PlayAttackSound(float volume)
    {
        audioSource.PlayOneShot(attackClip,volume);
    }

    public void PlayHurtstepSound(float volume)
    {
        audioSource.PlayOneShot(hurtClip,volume);
    }

     public void PlayDeadstepSound(float volume)
    {
        audioSource.PlayOneShot(deadClip,volume);
    }
}