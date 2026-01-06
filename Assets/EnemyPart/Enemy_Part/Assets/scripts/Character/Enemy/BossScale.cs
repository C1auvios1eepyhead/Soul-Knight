using UnityEngine;

public class BossScale : MonoBehaviour
{
    public float targetScale = 1.5f;    // 目标大小倍数
    public float scaleTime = 1f;        // 缩放时间
    
    public void ScaleUp()
    {
        StartCoroutine(ScaleRoutine());
    }
    
    System.Collections.IEnumerator ScaleRoutine()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * targetScale;
        float timer = 0f;
        
        while (timer < scaleTime)
        {
            timer += Time.deltaTime;
            float t = timer / scaleTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }
}