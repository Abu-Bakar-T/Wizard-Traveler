using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider menuSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject Level;
    [SerializeField] public GameObject Settings;
    [SerializeField] public GameObject PanelHider;
    [SerializeField] public Button continueButton;
    [SerializeField] public AudioSource menuMusic;
    [SerializeField] public AudioClip menuMusicClip;
    [SerializeField] bool isLevelMenuActive = false;

    [SerializeField] private bool isSfxSliderPressed = false;
    [SerializeField] private bool isMasterSliderPressed = false;
    [SerializeField] private bool isMenuSliderPressed = false;

    private int maxLevelUnlocked;
    private bool isFirstTime;

    void Start()
    {
        // Check if it's the first time the game is started
        isFirstTime = PlayerPrefs.GetInt("IsFirstTime", 1) == 1;

        GameObject audioSourceObject = GameObject.FindGameObjectWithTag("Background Music");
        if (audioSourceObject != null)
        {
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
        Menu.gameObject.SetActive(true);

        maxLevelUnlocked = PlayerPrefs.GetInt("MaxLevelUnlocked", 0);

        if (isFirstTime)
        {
            PanelHider.SetActive(true);
            continueButton.enabled = false;
        }
        else
        {
            continueButton.enabled = true;
            PanelHider.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (isFirstTime)
        {

            // Set to false after the first launch
            PlayerPrefs.SetInt("Game Points", 0);

            PlayerPrefs.SetInt("IsFirstTime", 0);
            PlayerPrefs.Save();

            // Perform any first-time setup here, like showing a tutorial
            Debug.Log("First time launching the game.");
        }
        else
        {
            Debug.Log("Game has been launched before.");
        }

        Time.timeScale = 1.0f;
        backgroundMusic.volume = 0.2f;
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);

        SceneManager.LoadScene("Level0"); // Start from Level 0
    }

    public void ContinueGame()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        int lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 0);
        SceneManager.LoadScene("Level" + lastPlayedLevel); // Continue from the last played level
    }

    public void ChangeToSettings()
    {
        isLevelMenuActive = true;
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        Settings.gameObject.SetActive(true);
        Menu.gameObject.SetActive(false);
    }

    public void ChangeToLeveMenu()
    {
        isLevelMenuActive = true;
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        Level.gameObject.SetActive(true);
        Menu.gameObject.SetActive(false);
    }

    public void ChangeToMainMenu()
    {
        isLevelMenuActive = false;
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        Settings.gameObject.SetActive(false);
        Level.gameObject.SetActive(false);
        Menu.gameObject.SetActive(true);
    }    
    public void LoadLevel0()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        // Set to false after the first launch
        PlayerPrefs.SetInt("IsFirstTime", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level0");
    }

    public void LoadLevel1()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        SceneManager.LoadScene("Level2");
    }

    public void LoadLevel3()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        SceneManager.LoadScene("Level3");
    }

    public void LoadLevel4()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        SceneManager.LoadScene("Level4");
    }

    public void LoadLevel5()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        SceneManager.LoadScene("Level5");
    }

    public void LoadLevel6()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        SceneManager.LoadScene("Level6");
    }

    public void LoadLevel7()
    {
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        SceneManager.LoadScene("Level7");
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f; 
        Handheld.Vibrate();
        menuMusic.PlayOneShot(menuMusicClip);
        Application.Quit();
    }

    public void ControlSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
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
}