using UnityEngine;

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
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("DestructibleTile"))
        {
            
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
        
        
    }
}
