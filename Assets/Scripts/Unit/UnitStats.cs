using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats
{
    public int Id = 0;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float Armour = 0f;
    public float MagicDamage = 0f;
    public float MagicResistance = 0f;
    public float MeleeAttackResistance = 0f;
    public float RangedAttackResistance = 0f;
    public float AttackSpeed = 1f;
    public float Gravity = 9.81f;
    public float Weight = 0f;
    public float AttackDamage = 10f;
    public float Speed = 5f;
    public float Stamina = 100f;
    public float CriticalChance = 0.05f;
    public float CriticalDamage = 1.5f;
    public float DodgeChance = 0f;
    public float BlockChance = 0f;

    [System.Serializable]
    public class Resistances
    {
        public float Fire = 0f;
        public float Ice = 0f;
        public float Poison = 0f;
        public float Lightning = 0f;
    }
    public Resistances resistances = new Resistances();

    public float Mana = 100f;
    public float ManaRegen = 5f;
    public float HealthRegen = 1f;
    public float Energy = 100f;
    public float EnergyRegen = 5f;
    public float Experience = 0f;
    public float Level = 1f;

    [System.Serializable]
    public class Attributes
    {
        public float Strength = 10f;
        public float Agility = 10f;
        public float Intelligence = 10f;
        public float Luck = 10f;
    }
    public Attributes attributes = new Attributes();
}
