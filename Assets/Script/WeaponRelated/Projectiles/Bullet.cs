using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;       // �����ٶ�
    public float lifeTime = 2f;     // ���ʱ��
    public float damage = 10f;      // �ӵ��˺�ֵ����Gun����

    private void Start()
    {
        // ��ֹ�ӵ����޴��ڣ�ռ���ڴ�
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // �ӵ���������ҷ����ƶ�
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ֻ���С�Enemy����ǩ�ĵ�������˺�
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Character>()?.TakeDamage(damage);
            Destroy(gameObject);
            return;
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
    }
}
