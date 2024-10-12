using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public PotionType currentPotionType; // Current potion type
    public float effectDuration = 5f; // Duration of potion effects

    public enum PotionType
    {
        DoubleDamage, // sword
        HalveDamage, // sword (Red)
        Heal,  //heart (50)
        HalveHealth, //heart (Red)
        DoubleStats, //star 2x writen)
        HalveStats, // Star with /2 written (Red)
        RandomStatIncrease, // with ?
        Giantify,
        Shield,  // Shield +1000
        Regeneration, // heart with +30
        IronSkin,  // shield with + 100 written
    }

    public void ApplyEffect(GameObject target)
    {
        Unit unit = target.GetComponent<Unit>();
        if (unit != null)
        {
            // Convert potion type to effect type
            Unit.EffectType effectType = Unit.ConvertPotionTypeToEffectType(currentPotionType);
            unit.ApplyTemporaryEffect(effectType);
        }
    }
}






/*
Heal: Restores a certain amount of health to the target.
DoubleDamage: Doubles the damage dealt by the target.
HalveDamage: Halves the damage dealt by the target.
HalveHealth: Reduces the target's health by half.
DoubleStats: Doubles all the target's stats (health, damage, etc.).
HalveStats: Halves all the target's stats (health, damage, etc.).
RandomStatIncrease: Increases a random stat of the target.
Giantify: Increases the target's health, armor, and resistance, but reduces attack speed.
Shield: Provides a shield that absorbs a certain amount of damage.
Regeneration: Gradually restores health over time.
IronSkin: Increases the target's defense, reducing incoming damage.
*/