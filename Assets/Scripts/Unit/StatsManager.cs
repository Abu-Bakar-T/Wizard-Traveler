using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public List<UnitStats> unitStatsList = new List<UnitStats>();
    public List<UnitStats> allyStatsList = new List<UnitStats>();
    public List<UnitStats> enemyStatsList = new List<UnitStats>();

    void Awake()
    {
        InitializePlayerStats();
    }

    void InitializePlayerStats()
    {
        // Initialize a sample list of stats
        for (int i = 1; i <= 40; i++)
        {
            unitStatsList.Add(new UnitStats
            {
                Id = i,
                Health = 200,
                MaxHealth = 200,
                Armour = 50,//Random.Range(50, 150),
                MagicDamage = 0,//Random.Range(0, 30),
                MagicResistance = 0,//Random.Range(0, 20),
                MeleeAttackResistance = 10,
                //MeleeAttackResistance = Random.Range(0, 20),
                RangedAttackResistance = 0,//Random.Range(0, 20),
                AttackSpeed = 1f,//Random.Range(0.5f, 2.0f),
                Gravity = 9.81f,
                Weight = 1f,// Random.Range(0.5f, 2.0f),
                AttackDamage = 20,
                //AttackDamage = Random.Range(5, 30),
                Speed = 5, //Random.Range(3, 10),
                Stamina = 80, //Random.Range(80, 150),
                CriticalChance = 0.1f,//Random.Range(0.05f, 0.25f),
                CriticalDamage = 1.5f,//Random.Range(1.2f, 2.0f),
                DodgeChance = 0.1f,//Random.Range(0.05f, 0.25f),
                BlockChance = 0.1f,//Random.Range(0.05f, 0.25f),
                resistances = new UnitStats.Resistances
                {
                    Fire = 0,//Random.Range(0, 10),
                    Ice = 0,//Random.Range(0, 10),
                    Poison = 0,//Random.Range(0, 10),
                    Lightning = 0,//Random.Range(0, 10)
                },
                Mana = 0,//Random.Range(80, 150),
                ManaRegen = 0,//Random.Range(2, 10),
                HealthRegen = 2,//Random.Range(1, 5),
                Energy = 0,//Random.Range(80, 150),
                EnergyRegen = 0,//Random.Range(2, 10),
                Experience = 0,
                Level = 1,
                attributes = new UnitStats.Attributes
                {
                    Strength = 10,//Random.Range(5, 20),
                    Agility = 10,//Random.Range(5, 20),
                    Intelligence = 10,//Random.Range(5, 20),
                    Luck = 10,//Random.Range(5, 20)
                }
            });
        }
    }

    public void ReinitializeStats()
    {
        unitStatsList.Clear();
        allyStatsList.Clear();
        enemyStatsList.Clear();
        InitializePlayerStats();
    }

    public UnitStats GetUnitStats(bool isAlly)
    {
        if (isAlly && allyStatsList.Count > 0)
        {
            return allyStatsList[Random.Range(0, allyStatsList.Count)];
        }
        else if (!isAlly && enemyStatsList.Count > 0)
        {
            return enemyStatsList[Random.Range(0, enemyStatsList.Count)];
        }

        return unitStatsList[Random.Range(0, unitStatsList.Count)];
    }

    public void AssignUnitStats(Unit unit, bool isAlly)
    {
        UnitStats stats = GetUnitStats(isAlly);
        unit.unitStats = new UnitStats()
        {
            Id = stats.Id,
            Health = stats.Health,
            MaxHealth = stats.MaxHealth,
            Armour = stats.Armour,
            MagicDamage = stats.MagicDamage,
            MagicResistance = stats.MagicResistance,
            MeleeAttackResistance = stats.MeleeAttackResistance,
            RangedAttackResistance = stats.RangedAttackResistance,
            AttackSpeed = stats.AttackSpeed,
            Gravity = stats.Gravity,
            Weight = stats.Weight,
            AttackDamage = stats.AttackDamage,
            Speed = stats.Speed,
            Stamina = stats.Stamina,
            CriticalChance = stats.CriticalChance,
            CriticalDamage = stats.CriticalDamage,
            DodgeChance = stats.DodgeChance,
            BlockChance = stats.BlockChance,
            resistances = new UnitStats.Resistances
            {
                Fire = stats.resistances.Fire,
                Ice = stats.resistances.Ice,
                Poison = stats.resistances.Poison,
                Lightning = stats.resistances.Lightning
            },
            Mana = stats.Mana,
            ManaRegen = stats.ManaRegen,
            HealthRegen = stats.HealthRegen,
            Energy = stats.Energy,
            EnergyRegen = stats.EnergyRegen,
            Experience = stats.Experience,
            Level = stats.Level,
            attributes = new UnitStats.Attributes
            {
                Strength = stats.attributes.Strength,
                Agility = stats.attributes.Agility,
                Intelligence = stats.attributes.Intelligence,
                Luck = stats.attributes.Luck
            }
        };

        Debug.Log($"Assigned Stats to {(isAlly ? "Ally" : "Enemy")}: Health={unit.unitStats.Health}, Armour={unit.unitStats.Armour}");
    }
}
