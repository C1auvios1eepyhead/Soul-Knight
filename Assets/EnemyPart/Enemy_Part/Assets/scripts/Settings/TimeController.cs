using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Range(0f,2f)]public float defaultTimeScale=1;//默认游戏时间速度

    [Header("子弹时间")]
    [SerializeField,Range(0f,2f)]float bulletTimeScale;//子弹时间速度
    [SerializeField] private float timeRecoveryDuration;//过渡回默认游戏时间的持续时间

    private GUIStyle labelStyle;
    private void Awake()
    {
        Time.timeScale = defaultTimeScale;

        
    }
    private void Start()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 120;
        labelStyle.normal.textColor = Color.white;
    }
    //用于数据测试
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 80), "ScaleTime = " + Time.timeScale, labelStyle);

    }


    public void BulletTime()
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(nameof(TimeRecoveryCoroutine));
    }

    //恢复默认时间的协程
    IEnumerator TimeRecoveryCoroutine()
    {
        float ratio = 0f;
        while (ratio < 1f)
        {
            ratio += Time.unscaledDeltaTime / timeRecoveryDuration;
            //三个参数：起始值、目标值和插值比例（0~1）
            Time.timeScale = Mathf.Lerp(bulletTimeScale,defaultTimeScale,ratio);

            yield return null;//等待下一帧再继续执行
        }

    }

}
