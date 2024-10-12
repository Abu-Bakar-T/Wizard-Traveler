using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy collided with Player. Game Over.");
            gameManager.PlayerDeath();
        }
    }
}
