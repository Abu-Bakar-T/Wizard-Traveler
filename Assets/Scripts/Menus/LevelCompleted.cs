using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompleted : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] public GameObject Menu;
    [SerializeField] public AudioSource menuMusic;
    [SerializeField] public AudioClip menuMusicClip;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManagerSource = GameObject.FindGameObjectWithTag("Game Manager");
        if (gameManagerSource != null)
        {
            gameManager = gameManagerSource.GetComponent<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("No AudioSource component found on the GameObject with tag 'Game Manager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Game Manager' found in the scene.");
        }
        // Find the GameObject with the tag "BackgroundMusic"
        GameObject audioSourceObject = GameObject.FindGameObjectWithTag("Background Music");
        if (audioSourceObject != null)
        {
            // Get the AudioSource component from the found GameObject
            backgroundMusic = audioSourceObject.GetComponent<AudioSource>();
            backgroundMusic.volume = 0.2f;

            if (backgroundMusic == null)
            {
                Debug.LogError("No AudioSource component found on the GameObject with tag 'Background Music'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Background Music' found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isLevelCompleted)
        {
            backgroundMusic.volume = 0.5f;
            Menu.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Menu.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    // Method to load the next scene
    public void LoadNextScene()
    {
        // Get the build index of the currently active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the index of the next scene
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the valid range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Optionally, handle the case where there is no next scene
            Debug.Log("No more scenes to load.");
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1.0f;
        Application.Quit();
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
    }

    // Method to change the scene to "Main Menu"
    public void ChangeToMainMenu()
    {
        Time.timeScale = 1.0f;
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        // Ensure that "Main Menu" is the exact name of the scene
        SceneManager.LoadScene("Main Menu");

    }
}
