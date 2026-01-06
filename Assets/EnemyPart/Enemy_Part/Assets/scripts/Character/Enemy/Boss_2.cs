using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public class Boss2 : Enemy
{
    [Header("远程攻击")]
    public GameObject bulletPrefab;
    public GameObject bulletPrefab_2;
    public Transform firePoint;

      
  
    public override void Attack()
    {
        if (player == null) return;
        UnityEngine.Debug.Log("ZZZNB1");

        if(phase==1)
        {

            // 计算朝向玩家的中心方向
            Vector2 centerDir = (player.position - firePoint.position).normalized;
            float baseAngle = Mathf.Atan2(centerDir.y, centerDir.x) * Mathf.Rad2Deg;

            // 发射散弹
            for (int i = 0; i < pelletCount; i++)
            {
                // 将散弹分布在 -spreadAngle 到 +spreadAngle 之间
                float angleOffset = Mathf.Lerp(-spreadAngle, spreadAngle, i / (float)(pelletCount - 1));
                float finalAngle = baseAngle + angleOffset;

                Vector2 dir = new Vector2(
                    Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                    Mathf.Sin(finalAngle * Mathf.Deg2Rad)
                ).normalized;

                GameObject bullet = Instantiate(
                    bulletPrefab,
                    firePoint.position,
                    Quaternion.Euler(0f, 0f, finalAngle)
                );

                bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
            }
        }
           if(phase==2)
        {

            // 计算朝向玩家的中心方向
            Vector2 centerDir = (player.position - firePoint.position).normalized;
            float baseAngle = Mathf.Atan2(centerDir.y, centerDir.x) * Mathf.Rad2Deg;

            // 发射散弹
            for (int i = 0; i < pelletCount; i++)
            {
                // 将散弹分布在 -spreadAngle 到 +spreadAngle 之间
                float angleOffset = Mathf.Lerp(-spreadAngle, spreadAngle, i / (float)(pelletCount - 1));
                float finalAngle = baseAngle + angleOffset;

                Vector2 dir = new Vector2(
                    Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                    Mathf.Sin(finalAngle * Mathf.Deg2Rad)
                ).normalized;

                GameObject bullet = Instantiate(
                    bulletPrefab_2,
                    firePoint.position,
                    Quaternion.Euler(0f, 0f, finalAngle)
                );

                bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
            }
        }
    }
}