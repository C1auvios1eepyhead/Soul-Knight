using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("����")]
    [SerializeField]protected float maxHealth;

    [SerializeField] protected float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    [Header("�޵�")]
    public bool invulnerable;
    public float invulnerableDuration;//�޵�ʱ��

    public UnityEvent OnHurt;
    public UnityEvent OnDie;
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (invulnerable)
            return;
        if (currentHealth -damage > 0f)
        {
            currentHealth -= damage;
            StartCoroutine(nameof(InvulnerableCoroutine));//����޵�ʱ��Э��
            //ִ�н�ɫ���˶���
            OnHurt?.Invoke();
        }
        else
        {
            //����
            Die();
        }
    }

    protected virtual void Die()
    {
        currentHealth = 0f;
        
        //ִ�н�ɫ��������
        OnDie?.Invoke();
    }

    //�޵�
    protected virtual IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;

        //�ȴ��޵�ʱ��
        yield return new WaitForSeconds(invulnerableDuration);

        invulnerable = false;
    }

 
 

}
