using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;       // 飞行速度
    public float lifeTime = 2f;     // 存活时间
    public float damage = 10f;      // 子弹伤害值，由Gun传入

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
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Character>()?.TakeDamage(damage);
        }

        // 当前版本：碰到任何东西就销毁
        Destroy(gameObject);
    }
}
