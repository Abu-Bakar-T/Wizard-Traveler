using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Tutorial : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject canvas;
    public GameObject Screen1;
    public GameObject Screen2;

    public bool isGamePlayedBefore = false;
    public bool isTutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        // Check if it's the first time the game is started
        isGamePlayedBefore = PlayerPrefs.GetInt("isGamePlayedBefore", 1) == 1;

        if (isGamePlayedBefore)
        {
            isTutorial = true;

            // Set to false after the first launch
            PlayerPrefs.SetInt("isGamePlayedBefore", 0);
            PlayerPrefs.Save();

            // Perform any first-time setup here, like showing a tutorial
            Debug.Log("First time launching the game.");

            // Set to false after the first launch
            PlayerPrefs.SetInt("Game Points", 0);
            PlayerPrefs.Save();

            // Perform any first-time setup here, like showing a tutorial
            Debug.Log("First time launching the game.");
        }
        else
        {
            Debug.Log("Game has been launched before.");
        }

        if(isTutorial)
        {
            canvas.SetActive(true);
            Screen1.SetActive(true);
            Screen2.SetActive(false);
            gameManager.isGameActive = false;
            Time.timeScale = 0.0f;
        }
        else
        {
            canvas.SetActive(false);
            Screen1.SetActive(false);
            Screen2.SetActive(false);
        }
    }

    private void Update()
    {
        if (isTutorial)
        {
            gameManager.isGameActive = false;
        }
        else
        {
            gameManager.isGameActive = true;
        }
    }
}