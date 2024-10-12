using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyUnits : MonoBehaviour
{
    public Transform alliesParent;
    public Transform enemiesParent;
    public RuntimeAnimatorController deathAnimationController;
    public GameObject smokeParticle;
    public GameObject target;

    public bool allEnemiesDead = false;
    private HashSet<GameObject> unitsDying = new HashSet<GameObject>();

    void LateUpdate()
    {
        CheckAndDestroyUnits(alliesParent);
        CheckAndDestroyUnits(enemiesParent);
        CheckAllEnemiesDead();
        if (allEnemiesDead)
            DestroyAllies(alliesParent);
    }

    void CheckAndDestroyUnits(Transform parent)
    {
        List<GameObject> unitsToDestroy = new List<GameObject>();

        foreach (Transform child in parent)
        {
            Unit unit = child.GetComponent<Unit>();
            if (unit != null && unit.unitStats.Health <= 0)
            {
                unitsToDestroy.Add(child.gameObject);
            }
        }

        foreach (GameObject unit in unitsToDestroy)
        {
            PlayDeathAnimationAndDestroy(unit);
        }
    }

    public void DestroyAllies(Transform parent)
    {
        List<GameObject> unitsToDestroy = new List<GameObject>();

        foreach (Transform child in parent)
        {
            Unit unit = child.GetComponent<Unit>();
            unitsToDestroy.Add(child.gameObject);
        }

        foreach (GameObject unit in unitsToDestroy)
        {
            PlayDeathAnimationAndDestroy(unit);
        }
    }

    void CheckAllEnemiesDead()
    {
        allEnemiesDead = true;
        foreach (Transform enemy in enemiesParent)
        {
            Unit unit = enemy.GetComponent<Unit>();
            if (unit != null && unit.unitStats.Health > 0)
            {
                allEnemiesDead = false;
                break;
            }
        }
    }

    public void PlayDeathAnimationAndDestroy(GameObject unit)
    {
        if (unitsDying.Contains(unit)) return; // Ensure this is triggered only once
        unitsDying.Add(unit);
        StartCoroutine(PlayDeathAnimationAndDestroyCoroutine(unit));
    }

    private IEnumerator PlayDeathAnimationAndDestroyCoroutine(GameObject unit)
    {
        Unit unitScript = unit.GetComponent<Unit>();
        if (unitScript != null)
        {
            AudioSource deathSource = unitScript.AudioSource;
            AudioClip deathClip = unitScript.deathClip;
            if (deathSource != null)
            {
                deathSource.PlayOneShot(deathClip);
            }
        }

        // Ensure the death sound plays immediately
        yield return new WaitForSeconds(0.1f);

        target = GameObject.FindGameObjectWithTag("Target");
        if (target != null)
        {
            Debug.Log("Target GameObject found: " + target.name);
            if (unit != null)
            {
                Animator animator = unit.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Dead", true);
                    Debug.Log("Playing Dead Animation");
                    // Wait for the animation to complete plus an additional delay
                    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 0.5f);
                }
                if (target != null)
                {
                    Instantiate(smokeParticle, target.transform.position, Quaternion.identity);
                }
                unitsDying.Remove(unit);
                Destroy(unit);
            }
        }
        else
        {
            Debug.Log("Target GameObject not found");
        }
    }
}