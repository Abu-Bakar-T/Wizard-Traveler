using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionData
{
    public string potionName;
    public string effectDescription;
    public string applyEffectLine;

    public PotionData(string potionName, string effectDescription, string applyEffectLine)
    {
        this.potionName = potionName;
        this.effectDescription = effectDescription;
        this.applyEffectLine = applyEffectLine;
    }
}

/*
using System.Collections.Generic;
using UnityEngine;

public class PotionDatabase : MonoBehaviour
{
    public static PotionDatabase Instance { get; private set; }

    public List<PotionData> potions = new List<PotionData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scene loads
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }

    void Start()
    {
        potions.Add(new PotionData("Heal", "Restores a certain amount of health to the target.", "health.Heal(50);"));
        potions.Add(new PotionData("Double Damage", "Doubles the damage dealt by the target.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.DoubleDamage, effectDuration));"));
        potions.Add(new PotionData("Halve Damage", "Halves the damage dealt by the target.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.HalveDamage, effectDuration));"));
        potions.Add(new PotionData("Halve Health", "Reduces the target's health by half.", "health.HalveHealth();"));
        potions.Add(new PotionData("Double Stats", "Doubles all the target's stats (health, damage, etc.).", "StartCoroutine(TemporaryEffect(health, Health.EffectType.DoubleStats, effectDuration));"));
        potions.Add(new PotionData("Halve Stats", "Halves all the target's stats (health, damage, etc.).", "StartCoroutine(TemporaryEffect(health, Health.EffectType.HalveStats, effectDuration));"));
        potions.Add(new PotionData("Random Stat Increase", "Increases a random stat of the target.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.RandomStatIncrease, effectDuration));"));
        potions.Add(new PotionData("Giantify", "Increases the target's health, armor, and resistance, but reduces attack speed.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.Giantify, effectDuration));"));
        potions.Add(new PotionData("Speed Boost", "Increases the target's movement speed.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.SpeedBoost, effectDuration));"));
        potions.Add(new PotionData("Shield", "Provides a shield that absorbs a certain amount of damage.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.Shield, effectDuration));"));
        potions.Add(new PotionData("Regeneration", "Gradually restores health over time.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.Regeneration, effectDuration));"));
        potions.Add(new PotionData("Invisibility", "Makes the target invisible to enemies.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.Invisibility, effectDuration));"));
        potions.Add(new PotionData("Stamina Boost", "Increases the target's stamina, allowing for longer or more frequent actions.", "// If needed, handle stamina boost logic here"));
        potions.Add(new PotionData("Night Vision", "Improves the target's vision in dark environments.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.NightVision, effectDuration));"));
        potions.Add(new PotionData("Slow Motion", "Slows down time for the target, making it easier to react.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.SlowMotion, effectDuration));"));
        potions.Add(new PotionData("Reflect Damage", "Reflects a portion of the damage taken back to the attacker.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.ReflectDamage, effectDuration));"));
        potions.Add(new PotionData("Iron Skin", "Increases the target's defense, reducing incoming damage.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.IronSkin, effectDuration));"));
        potions.Add(new PotionData("Levitation", "Allows the target to levitate, avoiding ground-based attacks and obstacles.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.Levitation, effectDuration));"));
        potions.Add(new PotionData("Magnetism", "Attracts nearby items or resources to the target.", "StartCoroutine(TemporaryEffect(health, Health.EffectType.Magnetism, effectDuration));"));
    }
}
*/