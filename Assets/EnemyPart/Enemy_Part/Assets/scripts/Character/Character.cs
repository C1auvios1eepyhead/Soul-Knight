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

    
    public float damagemutipler = 1;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    [Header("无敌时间或霸体")]
  

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
  
        if (currentHealth -damage > 0f)
        {
                if (!isSuperArmor)
                {
                    currentHealth -= damage*damagemutipler;
                    OnHurt?.Invoke();
                }
                else
                {
                    currentHealth -= damage*damagemutipler;
                    GetComponent<EnemyHurtFlash>().FlashRed();
                }
            
                if(currentHealth<=maxHealth*0.5)//阶段判断
                {
                    if(isboss1==true&&(!hastriggered))
                    {
                        hastriggered=true;
                        damagemutipler=0.8f;
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
                        damagemutipler=0.5f;
                        AnimationSound animSound = GetComponent<AnimationSound>();
                                if (animSound != null)
                        {
                            animSound.PlayPhaseSound(1.0f);
                        }
                        GetComponent<BossScale>()?.ScaleUp();
                        hastriggered=true;
                        AttackCooldownDuration *= 0.7f;
                        pelletCount += 4;
                        spreadAngle += 20f;
                        bulletSpeed+=8f;
                        phase+=1;
                    }
                }
                if(isboss1&&phase==2)
            {
                
                GetComponent<HurtRandomTeleport>()?.OnHurt();
                
            }
        }
        else 
        {
           
            Die();
        }
    }

    protected virtual void Die()
    {
        
        //ִ�н�ɫ��������
        OnDie?.Invoke();
    }

    //�޵�


 

}
