using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static AudioSource backgroundMusicInstance;
    // Start is called before the first frame update
    void Start()
    {
        Awake();
    }

    private void Awake()
    {
        backgroundMusicInstance = GetComponent<AudioSource>(); // Get the AudioSource component
        if (backgroundMusicInstance == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
        }
        else
        {
            DontDestroyOnLoad(gameObject); // Don't destroy this GameObject on load
        }
    }
}
