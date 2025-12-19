using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public class EnemyRanged : Enemy
{
    [Header("远程攻击")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;

    public override void Attack()
    {
        if (player == null) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Vector2 dir = (player.position - firePoint.position).normalized;
        bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
    }
}