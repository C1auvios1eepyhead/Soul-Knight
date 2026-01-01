using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{   

    [Header("PlayerUI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image armorBar;
    [SerializeField] private TextMeshProUGUI armorText;
    
    void Update()
    {
        UpdatePlayerUI();
    }

    private void UpdatePlayerUI(){
        PlayerConfig playerConfig = PlayerSelectionManager.Instance.Player;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, playerConfig.CurrentHealth / playerConfig.MaxHealth,  10f*Time.deltaTime);
        armorBar.fillAmount = Mathf.Lerp(armorBar.fillAmount, playerConfig.Armor / playerConfig.MaxArmor,  10f*Time.deltaTime);

        healthText.text = $"{playerConfig.CurrentHealth}/{playerConfig.MaxHealth}";
        armorText.text = $"{playerConfig.Armor}/{playerConfig.MaxArmor}";
    }
}
