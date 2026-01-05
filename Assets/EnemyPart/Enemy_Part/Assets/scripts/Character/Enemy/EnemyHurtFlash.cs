using System.Collections;
using UnityEngine;

public class EnemyHurtFlash : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void FlashRed(float duration = 0.1f)
    {
        StopAllCoroutines(); // 防止多次受击叠加
        StartCoroutine(FlashCoroutine(duration));
    }

    IEnumerator FlashCoroutine(float duration)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(duration);
        sr.color = originalColor;
    }
}
