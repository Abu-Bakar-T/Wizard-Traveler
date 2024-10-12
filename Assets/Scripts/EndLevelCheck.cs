using UnityEngine;
using UnityEngine.AI;

public class EndLevelCheck : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    public PlayerMovement playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called with tag: {other.tag}");
        Debug.Log($"OnTriggerEnter called with tag: {other.gameObject.tag}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected. Level completed.");
            StopPlayerMovement();
            gameManager.isLevelCompleted = true;
        }
    }

    private void StopPlayerMovement()
    {
        var rb = player.GetComponent<Rigidbody>();
        var agent = player.GetComponent<NavMeshAgent>();
        playerMovement.hasReachedEnd = true;

        if (rb != null)
        {
            Debug.Log("Rigidbody found. Stopping player.");
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;  // Disable physics interactions
        }
        else
        {
            Debug.LogError("Rigidbody not found on the player.");
        }
    }
}
