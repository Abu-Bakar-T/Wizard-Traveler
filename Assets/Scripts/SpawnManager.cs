using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform enemiesParent;
    public Transform alliesParent;
    public List<Transform> enemySpawnLocations;
    public List<Transform> allySpawnLocations;
    public List<GameObject> enemyUnitPrefabs;
    public List<GameObject> allyUnitPrefabs;
    public StatsManager statsManager;

    public int numberOfEnemies = 10;
    public int numberOfAllies = 5;
    public float spacing = 2f; // Distance between units
    public GameObject smokeParticle;

    void Start()
    {
        SpawnAllies();
        SpawnEnemies();
    }

    public void SpawnAllies()
    {
        ClearUnits(alliesParent);
        statsManager.ReinitializeStats();
        Debug.Log($"Spawning {numberOfAllies} allies.");

        SpawnUnits(numberOfAllies, allyUnitPrefabs, allySpawnLocations, alliesParent, true, Quaternion.Euler(0,180,0));
    }

    public void SpawnEnemies()
    {
        ClearUnits(enemiesParent);
        statsManager.ReinitializeStats();
        Debug.Log($"Spawning {numberOfEnemies} enemies.");

        SpawnUnits(numberOfEnemies, enemyUnitPrefabs, enemySpawnLocations, enemiesParent, false,Quaternion.identity);
    }

    void ClearUnits(Transform parent)
    {
        Debug.Log($"Clearing units in parent: {parent.name}");

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    void SpawnUnits(int numberOfUnits, List<GameObject> unitPrefabs, List<Transform> spawnLocations, Transform parent, bool isAlly, Quaternion rotation)
    {
        int unitIndex = 0;

        for (int prefabIndex = unitPrefabs.Count - 1; prefabIndex >= 0; prefabIndex--)
        {
            int unitsToSpawn = numberOfUnits / (int)Mathf.Pow(10, prefabIndex);

            for (int i = 0; i < unitsToSpawn; i++)
            {
                Vector3 position = spawnLocations[unitIndex % spawnLocations.Count].position;
                Instantiate(smokeParticle,position, rotation);
                GameObject unit = Instantiate(unitPrefabs[prefabIndex], position, rotation, parent);
                PlaySpawnSound(unit);
                InitializeUnitStats(unit, isAlly, prefabIndex);
                unitIndex++;
            }

            numberOfUnits %= (int)Mathf.Pow(10, prefabIndex);
        }
    }

    void PlaySpawnSound(GameObject unit)
    {
        Unit unitScript = unit.GetComponent<Unit>();
        unitScript.AudioSource.PlayOneShot(unitScript.spawnSound);
    }

    void InitializeUnitStats(GameObject unit, bool isAlly, int prefabIndex)
    {
        Unit unitScript = unit.GetComponent<Unit>();
        if (unitScript != null)
        {
            Debug.Log($"Assigning stats to {(isAlly ? "Ally" : "Enemy")}.");

            // Base stats assignment
            statsManager.AssignUnitStats(unitScript, isAlly);

            // Scale stats based on the prefab index
            if (prefabIndex > 0)
            {
                float multiplier = Mathf.Pow(10, prefabIndex);
                unitScript.unitStats.Health *= multiplier;
                unitScript.unitStats.MaxHealth *= multiplier;
                unitScript.unitStats.AttackDamage *= multiplier;
                unitScript.unitStats.AttackSpeed /= multiplier; 
                Debug.Log($"Increased stats for {(isAlly ? "Ally" : "Enemy")} due to prefab index {prefabIndex}: Health={unitScript.unitStats.Health}, Armour={unitScript.unitStats.Armour}, Multiplier={multiplier}");
            }
        }
        else
        {
            Debug.LogError("Unit script not found on the instantiated unit!");
        }
    }
}
