using System.Collections.Generic;
using UnityEngine;
// no need
public class CameraManager : MonoBehaviour
{
    public List<Camera> battleCameras;
    public Camera explorationCamera;
    public int roomNumber;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateCameras();
    }

    private void Update()
    {
        if (gameManager.isGameActive)
        {
            UpdateCameras();
        }
    }

    public void SetRoomNumber(int newRoomNumber)
    {
        roomNumber = newRoomNumber;
        UpdateCameras();
    }

    private void UpdateCameras()
    {
        DisableAllCameras();

        if (gameManager.enemiesCount > 0)
        {
            if (roomNumber > 0 && roomNumber <= battleCameras.Count)
            {
                battleCameras[roomNumber - 1].gameObject.SetActive(true);
            }
        }
        else
        {
            explorationCamera.gameObject.SetActive(true);
        }
    }

    private void DisableAllCameras()
    {
        if (explorationCamera != null)
        {
            explorationCamera.gameObject.SetActive(false);
        }

        foreach (Camera cam in battleCameras)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }
        }
    }
}
