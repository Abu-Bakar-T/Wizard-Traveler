using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform potionTransform; // Transform of the potion object
    public Transform handTransform; // Transform of the hand (or the position where the potion should be)
    public bool smoothSync = true; // Whether to smoothly sync the potion position and rotation
    public float syncSpeed = 10f; // Speed at which to sync position and rotation when smoothing

    void LateUpdate()
    {
        SyncPotionPosition();
    }

    void SyncPotionPosition()
    {
        if (potionTransform == null || handTransform == null)
        {
          //  Debug.LogWarning("Potion transform or hand transform is not assigned.");
            return;
        }

        if (smoothSync)
        {
            potionTransform.position = Vector3.Lerp(potionTransform.position, handTransform.position, Time.deltaTime * syncSpeed);
            potionTransform.rotation = Quaternion.Slerp(potionTransform.rotation, handTransform.rotation, Time.deltaTime * syncSpeed);
        }
        else
        {
            // Directly sync position and rotation without smoothing
            potionTransform.position = handTransform.position;
            potionTransform.rotation = handTransform.rotation;
        }

       // Debug.Log("Potion synchronized with hand position.");
    }
}
