using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Transform target;
    public bool hasReachedEnd;
    public GameObject player;
    private NavMeshAgent agent;
    private GameManager gameManager;
    public Animator animator;
    public GameObject smokeparticle;
    private bool hasDeath = false;
    public AudioSource audioSource;
    public AudioSource footstepAudioSource;
    public AudioClip[] audioClip;

    void Start()
    {
        hasReachedEnd = false;
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }

        if (target == null)
        {
            Debug.LogError("Target not assigned!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not assigned!");
        }
    }

    void LateUpdate()
    {
        if (gameManager != null && gameManager.destroyUnits.allEnemiesDead && target != null && !hasReachedEnd)
        {
            MoveTowardsTarget();
        }
        else
        {
            StopMovement();
        }

        if(!gameManager.isLevelCompleted && gameManager.isPlayerDead && !hasDeath)
        {
            hasDeath = true;
            animator.SetBool("Death", true);
            audioSource.PlayOneShot(audioClip[0]);
            StartCoroutine(WaitForTime(3.3f));
        }
    }
    private IEnumerator WaitForTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.SetBool("Death", false);
        Instantiate(smokeparticle, transform.position, Quaternion.identity);
        audioSource.PlayOneShot(audioClip[2]);
        Destroy(gameObject);
    }

    void MoveTowardsTarget()
    {
        agent.SetDestination(target.position);
        animator.SetBool("Running", true);
        footstepAudioSource.PlayOneShot(audioClip[1],0.5f);
        animator.SetBool("Idle", false);
    }

    void StopMovement()
    {
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetBool("Idle", true);
        footstepAudioSource.Stop();
        animator.SetBool("Running", false);
    }
}
