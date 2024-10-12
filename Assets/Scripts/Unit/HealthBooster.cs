using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBooster : MonoBehaviour
{
    public Transform unitsParent; // Assign the parent Transform in the Inspector
    [SerializeField] bool isIncreased;

    void Start()
    {
        isIncreased = false;
    }
    private void Update()
    {
        if (!isIncreased)
            BoostHealth();
    }

    void BoostHealth()
    {
        // Loop through each child under the parent Transform
        foreach (Transform child in unitsParent)
        {
            // Get the Unit component from the child GameObject
            Unit unit = child.GetComponent<Unit>();

            // Check if the Unit component exists and has a UnitStats
            if (unit != null && unit.unitStats != null)
            {
                // Increase the health of the unit by 30
                unit.unitStats.Health += 30;
                unit.unitStats.MaxHealth += 30;
                unit.unitStats.Armour += 2;
                unit.unitStats.AttackDamage += 1;
                Debug.Log($"Increased health of {child.name} to {unit.unitStats.Health}");
            }
        }
        isIncreased=true;
    }
}
