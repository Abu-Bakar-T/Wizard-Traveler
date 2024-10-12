using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonScript : MonoBehaviour
{
    public GameObject canvas;
    public GameObject Screen1;
    public GameObject Screen2;
    public AudioSource audioSource;
    public AudioClip clip;
    public Tutorial Tutorial;

    public void Next()
    {
        audioSource.PlayOneShot(clip);
        Handheld.Vibrate();
        Screen2.SetActive(true);
        Screen1.SetActive(false);
    }

    public void SkipTutorial()
    {
        audioSource.PlayOneShot(clip);
        Handheld.Vibrate();
        canvas.SetActive(false);
        Time.timeScale = 1.0f;
        Tutorial.isTutorial = false;
        Tutorial.gameManager.isGameActive = true;
    }

    public void Continue()
    {
        audioSource.PlayOneShot(clip);
        Handheld.Vibrate();
        canvas.SetActive(false);
        Time.timeScale = 1.0f;
        Tutorial.isTutorial = false;
        Tutorial.gameManager.isGameActive = true;
    }
}
