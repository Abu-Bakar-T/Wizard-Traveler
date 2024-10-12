using System.Collections;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public GameObject closedSprite;
    public GameObject openSprite;
    public GameManager gameManager;

    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip standardClip;
    public AudioClip travelClip;

    bool openClipPlayed = false;
    bool noMoreSound = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }
        closedSprite.SetActive(true);
        openSprite.SetActive(false);
    }

    void Update()
    {
        if (gameManager != null && gameManager.destroyUnits.allEnemiesDead && !openClipPlayed)
        {
            StartCoroutine(PlayAudioSequence());
            openClipPlayed = true;
        }
    }

    public IEnumerator PlayAudioSequence()
    {
        // Open portal and play openClip
        closedSprite.SetActive(false);
        openSprite.SetActive(true);
        audioSource.pitch = Mathf.Clamp(openClip.length / 1f, 0.1f, 3f); // Adjust pitch to fit 1-2 seconds
        audioSource.PlayOneShot(openClip);
        yield return new WaitForSeconds(openClip.length / audioSource.pitch); // Wait for the adjusted duration

        // Reset pitch to normal for subsequent audio clips
        audioSource.pitch = 1.0f;

        // Play standardClip in loop until the level is completed
        audioSource.clip = standardClip;
        audioSource.loop = true;
        audioSource.Play();

        // Wait until the level is completed
        while (!gameManager.isLevelCompleted)
        {
            yield return null;
        }

        // Stop standardClip and play travelClip
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.PlayOneShot(travelClip);

        noMoreSound = true;
    }
}
