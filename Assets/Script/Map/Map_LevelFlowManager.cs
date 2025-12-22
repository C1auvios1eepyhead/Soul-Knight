using UnityEngine;

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
    }
}
