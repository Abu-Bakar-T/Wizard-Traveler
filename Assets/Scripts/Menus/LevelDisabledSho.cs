using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDisabledSho : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public int level;
    [SerializeField] public GameObject GameObject;
    [SerializeField] private int maxLevelUnlocked;
    [SerializeField] Button button;

    private void Start()
    {
        maxLevelUnlocked = PlayerPrefs.GetInt("MaxLevelUnlocked", 0);
        button = GetComponent<Button>();
    }
    private void Update()
    {
        ActiveLevel();
    }

    public void ActiveLevel()
    {
        if (level <= maxLevelUnlocked)
        {
            GameObject.SetActive(false);
            button.enabled = true;
            Debug.Log("Level " + level + " is unlocked.");
        }
        else
        {
            GameObject.SetActive(true);
            button.enabled = false;
            Debug.LogWarning("Level " + level + " is not unlocked yet.");
        }
    }
}
