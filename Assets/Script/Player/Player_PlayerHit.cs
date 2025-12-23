using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private SpriteRenderer[] srs;
    private Color originalColor;

    void Awake()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();

        if (srs.Length > 0)
        {
            originalColor = srs[0].color;
        }
    }

    public void FlashRed()
    {
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        foreach (var sr in srs)
            sr.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        foreach (var sr in srs)
            sr.color = originalColor;
    }
}
