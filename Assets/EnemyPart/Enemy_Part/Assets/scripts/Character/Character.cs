using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("属性")]
    [SerializeField]protected float maxHealth;

    [SerializeField] protected float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    [Header("无敌时间或霸体")]
    public bool invulnerable;
    public float invulnerableDuration;//�޵�ʱ��

     public bool isSuperArmor = false;//霸体

     public int phase = 1;

     public bool isboss1;

     public bool isboss2;

     [Header("远程参数")]

     public float bulletSpeed = 8f;

     public int pelletCount = 3;  

     public float spreadAngle = 15f;

     public float AttackCooldownDuration = 2f;//攻击冷却时间

     bool hastriggered =false;




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
            if (!isSuperArmor)
            {
                currentHealth -= damage;
                StartCoroutine(nameof(InvulnerableCoroutine));
             
                OnHurt?.Invoke();
            }
            else
            {
                currentHealth -= damage;
                GetComponent<EnemyHurtFlash>().FlashRed();
            }
        }
        if(currentHealth<=maxHealth*0.5)
        {
            if(isboss1==true&&(!hastriggered))
            {
                hastriggered=true;
                AnimationSound animSound = GetComponent<AnimationSound>();
                        if (animSound != null)
                {
                    animSound.PlayPhaseSound(1.0f);
                }
                AttackCooldownDuration *= 0.8f;
                pelletCount += 2;
                spreadAngle += 10f;
                bulletSpeed+=4f;
                phase+=1;
            }
            if(isboss2==true&&(!hastriggered))
            {
                AnimationSound animSound = GetComponent<AnimationSound>();
                        if (animSound != null)
                {
                    animSound.PlayPhaseSound(1.0f);
                }
                hastriggered=true;
                AttackCooldownDuration *= 0.8f;
                pelletCount += 2;
                spreadAngle += 10f;
                bulletSpeed+=4f;
                phase+=1;

            }
        }
        else
        {
           
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
