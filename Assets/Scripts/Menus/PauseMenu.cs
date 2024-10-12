using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider menuSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] public GameObject Menu;
    [SerializeField] public AudioSource menuMusic;
    [SerializeField] public AudioClip menuMusicClip;

    [SerializeField] private bool isSfxSliderPressed = false;
    [SerializeField] private bool isMasterSliderPressed = false;
    [SerializeField] private bool isMenuSliderPressed = false;
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
 
        ControlMasterVolume();
        ControlSFXVolume();
        ControlMenuVolume(); 
    }

    public void ContinueMenu()
    {
        gameManager.isGamePaused = false;
        Menu.gameObject.SetActive(false);
        backgroundMusic.volume = 0.2f;
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        Time.timeScale = 1.0f;
        gameManager.isGameActive = true;
    }

    public void ControlSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
    }

    public void ControlMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("MV", Mathf.Log10(volume) * 20);
    }
    public void ControlMenuVolume()
    {
        float volume = menuSlider.value;
        audioMixer.SetFloat("BG", Mathf.Log10(volume) * 20);
    }

    public void ExitGame()
    {
        Time.timeScale = 1.0f;
        Application.Quit();
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
    }

    // Method to be called when the slider is pressed
    public void OnSliderPressed()
    {
        if (!isSfxSliderPressed)
        {
            menuMusic.PlayOneShot(menuMusicClip);
            isSfxSliderPressed = true;
        }
    }

    // Method to be called when the slider is released
    public void OnSliderReleased()
    {
        isSfxSliderPressed = false;
        isMasterSliderPressed = false;
        isMenuSliderPressed = false;
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
