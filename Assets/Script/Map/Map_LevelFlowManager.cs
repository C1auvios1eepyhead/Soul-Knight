using UnityEngine;
using Pathfinding;
public class Map_LevelFlowManager : MonoBehaviour
{
    public static Map_LevelFlowManager Instance;

    [SerializeField] private Map_LevelGenerator generator;
    private int stage = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void EnterNextStage()
    {
        stage++;
        generator.Generate(1, stage);
        ScanGraph_1();
  
    }
        public void ScanGraph_1()
    {
        if (AstarPath.active == null)
        {
            Debug.LogError("No AstarPath instance found in the scene!");
            return;
        }

        AstarPath.active.Scan();  // 扫描所有图
        Debug.Log("Pathfinding graph scanned!");
    }
}
