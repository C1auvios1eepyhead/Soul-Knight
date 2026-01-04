using UnityEngine;

public class Map_MiniMapBinder : MonoBehaviour
{
    [SerializeField] private Map_LevelGenerator generator;   // 关卡生成器
    [SerializeField] private Map_MiniMapController miniMap;  // 小地图控制器

    private void Awake()
    {
        if (generator != null && miniMap != null)
            generator.OnMiniMapBuilt += miniMap.Build;
    }

    private void OnDestroy()
    {
        if (generator != null && miniMap != null)
            generator.OnMiniMapBuilt -= miniMap.Build;
    }
}
