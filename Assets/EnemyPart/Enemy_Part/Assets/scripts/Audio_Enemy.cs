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
    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackClip);
    }

    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtClip);
    }
    public void PlayDeadSound()
    {
        audioSource.PlayOneShot(deadClip);
    }
}
