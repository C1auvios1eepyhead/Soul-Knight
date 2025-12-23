//using UnityEngine;

//public class Bullet_Boss : MonoBehaviour
//{
//    [Header("Bullet Settings")]
//    public float speed = 10f;       // 飞行速度
//    public float lifeTime = 5f;     // 存活时间
//    public float damage = 10f;      // 子弹伤害值

//    public LayerMask obstacleLayer;

//    private void Start_1()
//    {
//        // 防止子弹无限存在，占用内存
//        Destroy(gameObject, lifeTime);
//    }

//    private void Update_1()
//    {
//        // 子弹沿自身的右方向移动
//        transform.Translate(Vector3.right * speed * Time.deltaTime);
//    }

//    private void OnTriggerEnter2D_1(Collider2D collision)
//    {
//        //只对有“Enemy”标签的敌人造成伤害
//        if (collision.CompareTag("Player"))
//        {
//            //受击接口
//            collision.GetComponent<PlayerHealth>()?.TakeDamage(damage);
//            Destroy(gameObject);

//        }
//        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
//        {
//            Destroy(gameObject);
//        }

//    }
//}
using UnityEngine;
using UnityEngine.Tilemaps;
public class Bullet_Boss : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;       // 飞行速度
    public float lifeTime = 5f;     // 存活时间
    public float damage = 10f;      // 子弹伤害值，由Gun传入

    private void Start()
    {
        // 防止子弹无限存在，占用内存
        Destroy(gameObject, lifeTime);
    }

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //只对有“Enemy”标签的敌人造成伤害
        if (collision.CompareTag("Player"))
        {
            //敌人受击接口
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);

        }
          if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("DestructibleTile"))
        {
            // 1) 拿到 Tilemap
            Tilemap tilemap = collision.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                // 2) 用子弹当前位置决定打碎哪一格
                Vector2 hitPoint = collision.ClosestPoint(transform.position);
                Vector3Int cell = tilemap.WorldToCell(hitPoint);

                if (tilemap.HasTile(cell))
                    tilemap.SetTile(cell, null);
            }

            Destroy(gameObject);
            return;
        }

        // 当前版本：碰到任何东西就销毁
    }
}
