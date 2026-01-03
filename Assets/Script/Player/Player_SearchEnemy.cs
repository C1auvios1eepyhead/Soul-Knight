using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public float lockRadius = 6f;
    public Transform currentTarget;

    void Update()
    {
        if (currentTarget != null)
        {
            float dist = Vector2.Distance(transform.position, currentTarget.position);
            if (dist <= lockRadius) return;
        }

        FindTarget();
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float minDist = float.MaxValue;
        Transform best = null;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist && dist <= lockRadius)
            {
                minDist = dist;
                best = enemy.transform;
            }
        }

        currentTarget = best;
    }
}
