using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;       // 飞行速度
    public float lifeTime = 5f;     // 存活时间
    public float damage = 10f;      // 子弹伤害值

    public LayerMask obstacleLayer;

    private void Start()
    {
        // 防止子弹无限存在，占用内存
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 子弹沿自身的右方向移动
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //只对有“Enemy”标签的敌人造成伤害
        if (collision.CompareTag("Player"))
        {
            //受击接口
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);

        }
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            Destroy(gameObject);
        }

    }
}
