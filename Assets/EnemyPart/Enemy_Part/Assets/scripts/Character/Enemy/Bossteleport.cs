using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HurtRandomTeleport : MonoBehaviour
{
    [Header("瞬移设置")]
    public string teleportTag = "BossTeleport";
    public float cooldown = 3f;
    public float preTeleportDelay = 0.5f;
    
    [Header("特效设置")]
    public Sprite[] preTeleportFrames;  // 把你的特效帧拖到这里
    public Sprite[] teleportFrames;     // 把你的特效帧拖到这里
    
    [Header("特效属性")]
    public Color preTeleportColor = Color.white;
    public Color teleportColor = Color.white;
    public float preTeleportDuration = 0.5f;
    public float teleportDuration = 0.3f;
    public float preTeleportSize = 3f;  // 设置大一点确保可见
    public float teleportSize = 3f;
    
    private bool canTeleport = true;
    private List<Transform> teleportPoints = new List<Transform>();
    private int lastTeleportIndex = -1;
    private AnimationSound animSound;
    
    void Awake()
    {
        animSound = GetComponent<AnimationSound>();
    }
    
    void Start()
    {
        FindTeleportPoints();
    }
    
    void FindTeleportPoints()
    {
        teleportPoints.Clear();
        GameObject[] pointObjects = GameObject.FindGameObjectsWithTag(teleportTag);
        foreach (GameObject point in pointObjects)
        {
            if (point != null)
            {
                teleportPoints.Add(point.transform);
            }
        }
    }
    
    public void OnHurt()
    {
        if (canTeleport && teleportPoints.Count > 0)
        {
            StartCoroutine(TeleportSequence());
        }
    }
    
    IEnumerator TeleportSequence()
    {
        canTeleport = false;
        
        // 1. 显示传送前特效
        PlayEffect(transform.position, preTeleportFrames, preTeleportColor, preTeleportDuration, preTeleportSize);
        
        // 2. 等待延迟
        yield return new WaitForSeconds(preTeleportDelay);
        
        // 3. 选择瞬移点
        int randomIndex = GetRandomTeleportIndex();
        
        if (randomIndex >= 0 && randomIndex < teleportPoints.Count)
        {
            Transform targetPoint = teleportPoints[randomIndex];
            if (targetPoint != null)
            {
                lastTeleportIndex = randomIndex;
                
                // 4. 显示离开特效
                PlayEffect(transform.position, teleportFrames, teleportColor, teleportDuration, teleportSize);
                
                if (animSound != null) animSound.PlayTeleSound(1.0f);
                
                // 5. 执行传送
                transform.position = targetPoint.position;
                
                // 6. 显示到达特效
                PlayEffect(transform.position, teleportFrames, teleportColor, teleportDuration, teleportSize);
            }
        }
        
        // 7. 冷却
        yield return new WaitForSeconds(cooldown);
        canTeleport = true;
    }
    
    // ★ 这就是原来的PlayEffect方法，我保持名称一致
    void PlayEffect(Vector3 position, Sprite[] frames, Color color, float duration, float size = 1f)
    {
        if (frames == null || frames.Length == 0)
        {
            Debug.LogError("特效帧数组为空！请检查Inspector设置");
            return;
        }
        
        // 检查第一帧是否为空
        if (frames[0] == null)
        {
            Debug.LogError("特效第一帧为null！");
            return;
        }
        
        // 创建特效对象
        GameObject effect = new GameObject("TeleportEffect");
        effect.transform.position = position;
        effect.transform.localScale = Vector3.one * size;
        
        // 添加SpriteRenderer
        SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Character";
        sr.sortingOrder = 8; // 设置渲染层级
        sr.color = color;
        sr.sprite = frames[0]; // 设置第一帧
        
        // 播放帧动画
        StartCoroutine(PlayFrameAnimation(sr, frames, duration));
    }
    
    IEnumerator PlayFrameAnimation(SpriteRenderer sr, Sprite[] frames, float duration)
    {
        float frameTime = duration / frames.Length;
        
        // 从第二帧开始播放（第一帧已经在PlayEffect中设置了）
        for (int i = 1; i < frames.Length; i++)
        {
            if (frames[i] != null)
            {
                sr.sprite = frames[i];
            }
            yield return new WaitForSeconds(frameTime);
        }
        
        // 动画结束后销毁
        Destroy(sr.gameObject);
    }
    
    int GetRandomTeleportIndex()
    {
        if (teleportPoints.Count == 0) return -1;
        if (teleportPoints.Count == 1) return 0;
        
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, teleportPoints.Count);
        } 
        while (randomIndex == lastTeleportIndex);
        
        return randomIndex;
    }
    
    public bool CanTeleport()
    {
        return canTeleport;
    }
}