using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelControl : MonoBehaviour
{
    public float baseMoveSpeed = 5.0f;
    public float armLength = 1.5f;
    public float raycastLength = 2.0f;
    public Transform playerTransform;
    public GameManager gameManager;
    private List<GameObject> allies;
    private List<GameObject> enemies;
    private bool playerAttacked = false;
    private bool footstepSound = false;

    void Start()
    {
        UpdateUnitLists();
    }

    void FixedUpdate()
    {
        if (!gameManager.isGameActive)
        {
            StopAllActions();
            return;
        }
        UpdateUnitLists();
        MoveUnits();
        HandleCombat();
        CheckAllAlliesDead();
    }

    void StopAllActions()
    {
        foreach (var ally in allies)
        {
            if (ally != null)
            {
                StopMovement(ally);
                SetAnimationTrigger(ally, "Attack", false);
                SetAnimationTrigger(ally, "Running", false);
                SetAnimationTrigger(ally, "Idle", true);
            }
        }

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                StopMovement(enemy);
                SetAnimationTrigger(enemy, "Attack", false);
                SetAnimationTrigger(enemy, "Running", false);
                SetAnimationTrigger(enemy, "Idle", true);
            }
        }

        footstepSound = false;
    }

    void UpdateUnitLists()
    {
        if (gameManager == null) return;

        allies = gameManager.GetAllies();
        enemies = gameManager.GetEnemies();
    }

    void MoveUnits()
    {
        MoveGroup(allies, enemies, true);  // Move allies toward enemies
        MoveGroup(enemies, allies, false); // Move enemies toward allies
    }

    void MoveGroup(List<GameObject> movers, List<GameObject> targets, bool isAllyGroup)
    {
        for (int i = 0; i < movers.Count; i++)
        {
            var mover = movers[i];
            if (mover == null) continue;

            var unitComponent = mover.GetComponent<Unit>();
            var closestTarget = FindClosestUnit(mover, targets);

            if (closestTarget != null)
            {
                unitComponent.targetLockedon = closestTarget;
                float distance = Vector3.Distance(mover.transform.position, closestTarget.transform.position);
                float combinedArmLength = armLength * (mover.transform.localScale.magnitude + closestTarget.transform.localScale.magnitude) / 2;

                if (distance >= combinedArmLength)
                {
                    if (!IsUnitInFront(mover))
                    {
                        MoveTowards(mover, closestTarget.transform.position);
                    }
                    else
                    {
                        StopMovement(mover);
                        SetAnimationTrigger(mover, "Idle", true);
                    }
                }
                else
                {
                    FaceTarget(mover, closestTarget);
                    StopMovement(mover);
                    EngageInCombat(mover, closestTarget);
                }
            }
            else
            {
                StopMovement(mover);
                SetAnimationTrigger(mover, "Idle", true);
            }
        }
    }

    bool IsUnitInFront(GameObject mover)
    {
        RaycastHit hit;
        if (Physics.Raycast(mover.transform.position, mover.transform.forward, out hit, raycastLength))
        {
            var hitUnit = hit.transform.GetComponent<Unit>();
            if (hitUnit != null && hitUnit.unitStats.Health > 0)
            {
                return true;
            }
        }
        return false;
    }

    GameObject FindClosestUnit(GameObject mover, List<GameObject> targets)
    {
        GameObject closestUnit = null;
        float closestDistance = Mathf.Infinity;

        foreach (var target in targets)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(mover.transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestUnit = target;
                }
            }
        }

        return closestUnit;
    }

    void MoveTowards(GameObject unit, Vector3 targetPosition)
    {
        NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = baseMoveSpeed / Mathf.Max(1, unit.GetComponent<Unit>().unitStats.Weight);
            agent.SetDestination(targetPosition);
            agent.isStopped = false;

            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                SetAnimationTrigger(unit, "Running", true);
                SetAnimationTrigger(unit, "Idle", false);
            }
            else
            {
                SetAnimationTrigger(unit, "Running", false);
                SetAnimationTrigger(unit, "Idle", true);
            }

            if (!footstepSound)
            {
                var unitScript = unit.GetComponent<Unit>();
                if (unitScript != null)
                {
                    unitScript.PlayFootstepSound();
                    footstepSound = true;
                }
            }
        }
    }

    void StopMovement(GameObject unit)
    {
        if (unit == null) return;

        NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        Rigidbody rb = unit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            var unitScript = unit.GetComponent<Unit>();
            if (unitScript != null && unitScript.footStepSource != null)
            {
                unitScript.footStepSource.Stop();
                footstepSound = false;
            }
        }

        SetAnimationTrigger(unit, "Running", false);
        SetAnimationTrigger(unit, "Idle", true);
    }

    void HandleCombat()
    {
        for (int i = allies.Count - 1; i >= 0; i--)
        {
            var ally = allies[i];
            if (ally == null) continue;

            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                var enemy = enemies[j];
                if (enemy == null) continue;

                float distance = Vector3.Distance(ally.transform.position, enemy.transform.position);
                float combinedArmLength = armLength * (ally.transform.localScale.magnitude + enemy.transform.localScale.magnitude) / 2;

                if (distance < combinedArmLength)
                {
                    if (ally != null && enemy != null)
                    {
                        FaceTarget(ally, enemy);
                        FaceTarget(enemy, ally);
                        EngageInCombat(ally, enemy);

                        var allyUnit = ally.GetComponent<Unit>();
                        if (allyUnit != null && allyUnit.unitStats.Health <= 0 && !allyUnit.isDying)
                        {
                            allyUnit.isDying = true;
                            gameManager.RemoveAlly(ally);
                            ResetUnitState(enemy);
                            break;
                        }

                        var enemyUnit = enemy.GetComponent<Unit>();
                        if (enemyUnit != null && enemyUnit.unitStats.Health <= 0 && !enemyUnit.isDying)
                        {
                            gameManager.points += 200;
                            enemyUnit.isDying = true;
                            gameManager.RemoveEnemy(enemy);
                            ResetUnitState(ally);
                        }
                    }
                }
            }
        }
    }

    void EngageInCombat(GameObject ally, GameObject enemy)
    {
        Unit allyUnit = ally.GetComponent<Unit>();
        Unit enemyUnit = enemy.GetComponent<Unit>();

        if (allyUnit != null && allyUnit.unitStats != null && allyUnit.CanAttack() && allyUnit.targetLockedon == enemy && !allyUnit.isDying)
        {
            enemyUnit.unitStats.Health -= allyUnit.unitStats.AttackDamage;
            allyUnit.ResetAttackCooldown();
            StartCoroutine(PlayAttackAnimation(ally));

            StartCoroutine(PlayParticleSystemWithDelay(enemy, 1f));
        }

        if (enemyUnit != null && enemyUnit.unitStats != null && enemyUnit.CanAttack() && enemyUnit.targetLockedon == ally && !enemyUnit.isDying)
        {
            allyUnit.unitStats.Health -= enemyUnit.unitStats.AttackDamage;
            enemyUnit.ResetAttackCooldown();
            StartCoroutine(PlayAttackAnimation(enemy));

            StartCoroutine(PlayParticleSystemWithDelay(ally, 1f));
        }

        StopMovement(ally);
        StopMovement(enemy);
    }

    IEnumerator PlayAttackAnimation(GameObject unit)
    {
        Animator animator = unit.GetComponent<Animator>();
        Unit unitComponent = unit.GetComponent<Unit>();

        if (animator != null && unitComponent != null)
        {
            unitComponent.isSlashSoundPlayed = false;  // Reset flag before animation
            animator.SetBool("Attack", true);

            if (unit != null)
            {
                StartCoroutine(PlaySlashSoundWithDelay(unitComponent, 0.3f));
            }

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            if (animator != null)
            {
                animator.SetBool("Attack", false);
                animator.SetBool("Idle", true);
            }
        }
    }

    IEnumerator PlaySlashSoundWithDelay(Unit unit, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (unit != null && !unit.isSlashSoundPlayed)
        {
            unit.AudioSource.PlayOneShot(unit.slashsound);
            unit.isSlashSoundPlayed = true;  // Set flag to prevent multiple plays
            Debug.Log("Slash sound played");
        }
    }

    void ResetUnitState(GameObject unit)
    {
        var unitComponent = unit.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.targetLockedon = null;
            SetAnimationTrigger(unit, "Attack", false);
            SetAnimationTrigger(unit, "Idle", true);
        }
    }

    IEnumerator PlayParticleSystemWithDelay(GameObject unit, float delay)
    {
        if (unit != null)
        {
            yield return new WaitForSeconds(delay);
            if (unit != null)
            {
                var particleSystems = unit.GetComponentsInChildren<ParticleSystem>();
                foreach (var particleSystem in particleSystems)
                {
                    if (particleSystem != null)
                        particleSystem.Play();
                    yield return new WaitForSeconds(0.5f);
                    if (particleSystem != null)
                        particleSystem.Stop();
                }
            }
        }
    }

    void FaceTarget(GameObject unit, GameObject target)
    {
        if (unit != null && target != null)
        {
            Vector3 direction = (target.transform.position - unit.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void SetAnimationTrigger(GameObject unit, string trigger, bool state)
    {
        if (unit != null)
        {
            Animator animator = unit.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool(trigger, state);
            }
        }
    }

    void CheckAllAlliesDead()
    {
        if (gameManager.alliesCount == 0 && gameManager.enemiesCount > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    StopMovement(enemy);
                    SetAnimationTrigger(enemy, "Idle", true);
                }
            }
            Debug.LogWarning("Calling Player Death in CheckAllAllies");
            gameManager.PlayerDeath();
        }
    }
}
