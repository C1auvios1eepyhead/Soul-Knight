using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerConfig runtimeConfig;

    public void Init(PlayerConfig template)
    {
        runtimeConfig = Instantiate(template); // 关键！！
        runtimeConfig.ResetPlayerStats();
    }
}
