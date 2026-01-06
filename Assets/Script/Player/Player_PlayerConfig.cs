using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType{
    knight,
    dwarf,
    nun,
    ghost,
    savage,
    wizard
}

[CreateAssetMenu]
public class PlayerConfig : ScriptableObject
{
	[Header("Data")]
	public PlayerType playerType;
	public int Level;
	public string Name;
	public Sprite Icon;
	[Header("Values")]
	public float CurrentHealth;
	public float MaxHealth;
	public float cfgHealth;
	public float Armor;
	public float MaxArmor;
	public float cfgArmor;
	public float Energy;
	public float MaxEnergy;
	public float CriticalChance; 
	public float CriticalDamage;
	public string describe;
	[Range(0, 100f)]
	public int UpgradeMultiplier;

	public void ResetPlayerStats(){
		MaxHealth = cfgHealth;
		MaxArmor = cfgArmor;
		CurrentHealth = MaxHealth;
		Armor = MaxArmor;
	}
	
}
