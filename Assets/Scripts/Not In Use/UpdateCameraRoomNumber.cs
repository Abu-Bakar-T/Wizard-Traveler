using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Extra
public class UpdateCameraRoomNumber : MonoBehaviour
{
    public int roomNumber;
    public CameraManager cameraManager;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(cameraManager.roomNumber == 1)
            {
                cameraManager.roomNumber = roomNumber;
            }
            else
                cameraManager.roomNumber = 1;

        }
    }
}
