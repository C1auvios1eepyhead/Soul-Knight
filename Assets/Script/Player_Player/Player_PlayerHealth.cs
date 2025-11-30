using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private playerconfig playerConfig;
    
    private float lastDamageTime;
    private Coroutine armorRegenCoroutine;

    private void Update(){
        if(Input.GetKeyDown(KeyCode.R)){
            TakeDamage(1f);
        }
        if(Input.GetKeyDown(KeyCode.P)){
            RecoverHealth(1f);
        }
        HandleArmorRegen();
    }

    private void HandleArmorRegen(){
        if(armorRegenCoroutine != null){
            return;
        }
        if(Time.time -lastDamageTime >= 5f){
            armorRegenCoroutine = StartCoroutine(AutoRegenArmor());
        }
    }

    private IEnumerator AutoRegenArmor(){
        while(true){
            yield return new WaitForSeconds(2f);

            if(playerConfig.Armor < playerConfig.MaxArmor){
                playerConfig.Armor += 1f;
            }else{
                armorRegenCoroutine = null;
                yield break;
            }
        }
    }

    public void RecoverHealth(float amount){
        playerConfig.CurrentHealth += amount;
        if(playerConfig.CurrentHealth > playerConfig.MaxHealth){
            playerConfig.CurrentHealth = playerConfig.MaxHealth;
        }
    }

    public void TakeDamage(float amount){

        lastDamageTime = Time.time;

        if (armorRegenCoroutine != null)
        {
            StopCoroutine(armorRegenCoroutine);
            armorRegenCoroutine = null;
        }

        if(playerConfig.Armor > 0){
            float remainingDamage = amount - playerConfig.Armor;
            playerConfig.Armor = Mathf.Max(playerConfig.Armor - amount, 0f);
            if(remainingDamage > 0){
                playerConfig.CurrentHealth = Mathf.Max(playerConfig.CurrentHealth - remainingDamage, 0f);
            }
        }else{
            playerConfig.CurrentHealth = Mathf.Max(playerConfig.CurrentHealth - amount, 0f);
        }

        if(playerConfig.CurrentHealth <= 0){
            PlayerDead();
        }
    }

    private void PlayerDead(){
        Destroy(gameObject);
    }
}
