using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System;
using static Potion;

public class PotionThrower : MonoBehaviour
{
    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;
    public GameObject potionPrefab; // Prefab of the potion to be thrown
    public LineRenderer aimLine; // LineRenderer for aiming
    public float maxThrowForce = 500f; // Maximum force with which the potion can be thrown
    public float radius = 5f; // Radius of the potion's effect
    public Transform handTransform; // Transform where the potion is held in the player's hand
    public Transform initialAimPosition; // position where aim will start from.
    //public LineRenderer radiusLine; // LineRenderer for radius circle
    public Animator animatorThrow; // thrown object 

    private bool isAiming = false; // Flag to check if the player is aiming
    private Vector3 aimPosition; // Position where the aim line points to
    private Vector3 oldTouchLocation;
    private Vector3 initialTouchPosition; // Position where the initial touch is
    public GameObject currentPotion; // The potion currently held in the player's hand
    private bool potionInAir = false; // Flag to check if a potion is already thrown
    private bool buttonPressed = false; // Flag to check if the button was pressed

    public GameObject radiusPrefab; // Prefab for the potion landing radius visualization
    public AudioClip radiusAudioClip;
    public TextMeshProUGUI radiusTextPrefab; // Prefab for the text display using TextMeshProUGUI

    private GameObject thrownPotionParent; // Temporary parent for thrown potions
    public TextMeshProUGUI currentEffectText; // UI text to show the current potion effect

    public Potion.PotionType selectedPotionType = Potion.PotionType.Heal;

    [SerializeField] private GameManager gameManager;
    public Camera cameras; // Array of all your cameras

    // Array to store Audio Clips and Radius Prefabs.
    public GameObject[] radiusPrefabs;
    public AudioSource radiusAudioSource;
    public AudioClip[] radiusAudioClips;
    public AudioClip potionSoundInAir;
    public AudioSource potionInAirAudioSource;

    public AudioSource menuMusic;
    public AudioClip menuMusicClip;

    public Sprite[] potionSprites;

    public Image image;
    public AudioClip noPotionSound; // Sound to play when trying to throw with 0 potions


    // Array to store different potions
    public GameObject[] potionPrefabs;
    void DisableRotation()
    {
        Rigidbody potionRb = potionPrefab.GetComponent<Rigidbody>();
        potionRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Start()
    {
        // Initialize the LineRenderer for aiming
        aimLine.positionCount = 0;
        aimLine.enabled = false;

        selectedPotionType = gameManager.availablePotions[0];
        // Initialize the LineRenderer for radius
        //radiusLine.positionCount = 0;
        //radiusLine.enabled = false;
        SelectionOfRadiusPrefabnAudio();
        // Initialize the potion in the player's hand
        if (potionPrefab != null && handTransform != null)
        {
            currentPotion = Instantiate(potionPrefab, handTransform.position, handTransform.rotation, handTransform);
            Rigidbody rb = currentPotion.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // Disable gravity when the potion is in the player's hand
            }
        }

        // Create the temporary parent for thrown potions
        thrownPotionParent = new GameObject("ThrownPotionsParent");
        UpdateCurrentEffectText();
        
        DisableRotation();

        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            //Debug.LogError("GameManager not found!");
        }
    }

    void Update()
    {
        if(potionInAir)
        {
            animatorThrow.SetBool("Throw", false);
            animatorThrow.SetBool("Idle", true);
        }
        if (gameManager.isGameActive)
        {
#if UNITY_EDITOR
            HandleMouseInput();
#else
        HandleTouchInput();
#endif
        }
    }

    void UpdateCurrentEffectText()
    {
        if (currentEffectText != null)
        {
            currentEffectText.text = selectedPotionType.ToString();
        }
    }

    private IEnumerator WaitAndThrowPotion(float waitTime, Vector3 aimPosition)
    {
        animatorThrow.SetBool("Throw", true);
        yield return new WaitForSeconds(waitTime); // Wait for the specified time
        ThrowPotion(aimPosition); // Throw the potion after the wait
    }

    void HandleMouseInput()
    {
        // Prevent input if potion is already in the air, no enemies, or the mouse is over a UI element
        if (potionInAir || gameManager.GetEnemies().Count <= 0 || IsPointerOverUIElement(Input.mousePosition))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isAiming = true;
            aimPosition = initialAimPosition.position; // Start aiming from the initialAimPosition's position
            initialTouchPosition = GetWorldPosition(Input.mousePosition); // Store the initial touch position
        }
        else if (Input.GetMouseButton(0) && isAiming)
        {
            Vector3 currentTouchPosition = GetWorldPosition(Input.mousePosition);
            Vector3 direction = initialTouchPosition - currentTouchPosition; // Calculate direction based on touch movement
            aimPosition = handTransform.position + direction; // Adjust aim position based on direction
            UpdateAimLine(aimPosition); // Update the aim line based on the adjusted aim position
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isAiming)
            {
                if (gameManager.GetPotionCount(selectedPotionType) > 0) // Check if the potion is available
                {
                    StartCoroutine(WaitAndThrowPotion(1.73f, aimPosition));
                    gameManager.availablePotionCounts[selectedPotionType]--;
                }
                else
                {
                    PlayNoPotionSound(); // Play sound if potion count is 0
                }
                isAiming = false;
                aimLine.enabled = false;
                //radiusLine.enabled = false;
            }
        }
    }

    void HandleTouchInput()
    {
        // Prevent input if potion is already in the air, no enemies, or the touch is over a UI element
        if (potionInAir || gameManager.GetEnemies().Count <= 0)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Check if the touch is over a UI element
                if (IsPointerOverUIElement(touch.position))
                    return;

                isAiming = true;
                aimPosition = initialAimPosition.position; // Start aiming from the initialAimPosition's position
                initialTouchPosition = GetWorldPosition(touch.position); // Store the initial touch position
            }
            else if (touch.phase == TouchPhase.Moved && isAiming)
            {
                Vector3 currentTouchPosition = GetWorldPosition(touch.position);
                Vector3 direction = initialTouchPosition - currentTouchPosition; // Calculate direction based on touch movement
                aimPosition = handTransform.position + direction; // Adjust aim position based on direction
                UpdateAimLine(aimPosition); // Update the aim line based on the adjusted aim position
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isAiming)
                {
                    // Check if the target is within the clear path before allowing the throw
                    if (IsClearPath(aimPosition))
                    {
                        if (gameManager.GetPotionCount(selectedPotionType) > 0) // Check if the potion is available
                        {
                            gameManager.availablePotionCounts[selectedPotionType]--;
                            StartCoroutine(WaitAndThrowPotion(1.73f, aimPosition));
                        }
                        else
                        {
                            PlayNoPotionSound(); // Play sound if potion count is 0
                        }
                    }
                    else
                    {
                        Debug.Log("Throw canceled: target is not in front of the player.");
                    }

                    isAiming = false;
                    aimLine.enabled = false;
                    //radiusLine.enabled = false;
                }
            }
        }
    }

    bool IsPointerOverUIElement(Vector2 position)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventData, results);
        return results.Count > 0;
    }

    void PlayNoPotionSound()
    {
        if (noPotionSound != null)
        {
            potionInAirAudioSource.PlayOneShot(noPotionSound);
        }
    }

    Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Camera activeCamera = Camera.main;

        Ray ray = activeCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return ray.GetPoint(10); // Default to some distance if no collider was hit
    }

    void UpdateAimLine(Vector3 aimPos)
    {
        if (!IsClearPath(aimPos))
        {
            aimLine.enabled = false;
            //radiusLine.enabled = false;
            return;
        }

        aimLine.enabled = true;
        Vector3 startPos = handTransform.position; // Use handTransform position for accuracy

        Vector3[] allPoints = CalculateTrajectoryPoints(startPos, aimPos, 30); // Calculate all points for the trajectory
        int pointCount = Mathf.CeilToInt(allPoints.Length * 1.5f / 3f); // Calculate 2/3 of the points

        Vector3[] limitedPoints = new Vector3[pointCount];
        System.Array.Copy(allPoints, limitedPoints, pointCount); // Copy the first 2/3 points to a new array

        aimLine.positionCount = limitedPoints.Length;
        aimLine.SetPositions(limitedPoints);

        // Visualize the radius of the potion's effect
        //DrawRadiusCircle(aimPos);
    }
    Vector3[] CalculateTrajectoryPoints(Vector3 start, Vector3 target, int pointsCount)
    {
        List<Vector3> points = new List<Vector3>();
        float initialVelocity = CalculateLaunchVelocity(start, target);
        Vector3 direction = (target - start).normalized;
        float angle = 45f * Mathf.Deg2Rad;
        Vector3 initialVelocityVector = direction * initialVelocity * Mathf.Cos(angle) + Vector3.up * initialVelocity * Mathf.Sin(angle);

        for (int i = 0; i < pointsCount; i++)
        {
            float t = i * 0.1f; // Adjust time step as necessary
            Vector3 point = start + initialVelocityVector * t + 0.5f * Physics.gravity * t * t;
            points.Add(point);

            if (Vector3.Distance(point, target) <= 0.5f) // Stop if the trajectory reaches the target
                break;
        }

        points.Add(target); // Ensure the last point is the target position

        return points.ToArray();
    }

    float CalculateLaunchVelocity(Vector3 start, Vector3 target)
    {
        float distance = Vector3.Distance(start, target);
        float angle = 45f * Mathf.Deg2Rad; // Launch angle of 45 degrees
        float gravity = Physics.gravity.magnitude;
        float velocity = Mathf.Sqrt(distance * gravity / Mathf.Sin(2 * angle));
        return Mathf.Min(velocity, maxThrowForce); // Ensure the velocity doesn't exceed maxThrowForce
    }

    // Excluded
    /*
    void DrawRadiusCircle(Vector3 center)
    {
        radiusLine.enabled = true;
        int segments = 50;
        float angle = 0f;
        float angleStep = 360f / segments;

        radiusLine.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float x = center.x + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = center.z + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            radiusLine.SetPosition(i, new Vector3(x, center.y, z));
            angle += angleStep;
        }
    }*/

    bool IsClearPath(Vector3 targetPosition)
    {
        Vector3 startPos = handTransform.position;
        Vector3 direction = (targetPosition - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPosition);

        // Check if the target is within the forward 180-degree arc
        Vector3 playerForward = transform.forward;
        float dotProduct = Vector3.Dot(playerForward, direction);

        if (dotProduct <= 0)
        {
            // Target is behind the player, cancel the throw
            return false;
        }

        Ray ray = new Ray(startPos, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            // If the ray hits the player, cancel the throw
            if (hit.collider.gameObject == gameObject)
            {
                return false;
            }
        }
        return true;
    }

    void ThrowPotion(Vector3 targetPosition)
    {
        if (currentPotion != null && !potionInAir && !buttonPressed)
        {
            currentPotion.SetActive(false);
            potionInAirAudioSource.PlayOneShot(potionSoundInAir);
            GameObject thrownPotion = Instantiate(potionPrefab, handTransform.position, Quaternion.identity);

            // Reduce the potion count after throwing

            Rigidbody rb = thrownPotion.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;

            if (rb != null)
            {
                rb.useGravity = true;
                Vector3 throwDirection = CalculateLaunchDirection(handTransform.position, targetPosition);
                float velocity = CalculateLaunchVelocity(handTransform.position, targetPosition);
                rb.velocity = throwDirection * velocity;
                potionInAir = true;

                thrownPotion.transform.parent = thrownPotionParent.transform;
                Potion potionComponent = thrownPotion.GetComponent<Potion>();
                if (potionComponent != null)
                {
                    potionComponent.currentPotionType = selectedPotionType;
                }
                else
                {
                   // Debug.LogWarning("Potion script not found on the potion prefab.");
                }

                // Assign the references to the PotionLanding script
                PotionLanding potionLanding = thrownPotion.GetComponent<PotionLanding>();
                if (potionLanding != null)
                {
                    potionLanding.potionThrower = this;
                    potionLanding.radiusPrefab = radiusPrefab;
                    potionLanding.radiusAudioClip = radiusAudioClip;
                    potionLanding.radiusAudioSource = radiusAudioSource;
                    potionLanding.radiusText = radiusTextPrefab;
                    potionLanding.potionType = selectedPotionType;
                    potionLanding.animator = animatorThrow;
                    potionLanding.potioninAirAudiosource = potionInAirAudioSource;
                }
                else
                {
                   // Debug.LogWarning("PotionLanding script not found on the potion prefab.");
                }
            }
        }
    }

    Vector3 CalculateLaunchDirection(Vector3 start, Vector3 target)
    {
        Vector3 direction = target - start;
        float distance = direction.magnitude;
        direction.y += distance * Mathf.Tan(45f * Mathf.Deg2Rad);
        return direction.normalized;
    }

    public void PotionLanded(GameObject landedPotion)
    {
        // Ensure that cleanup is complete before destroying the potion
        Destroy(landedPotion);

        // Re-enable the potion in the player's hand
        if (currentPotion != null)
        {
            currentPotion.SetActive(true);
            potionInAir = false;
        }
    }

    public void BuyPotion(Potion.PotionType potionType, int cost)
    {
        if (gameManager.points >= cost)
        {
            gameManager.points = gameManager.points - cost;
            menuMusic.PlayOneShot(menuMusicClip);
            buttonPressed = true;

            selectedPotionType = potionType;
            gameManager.availablePotionCounts[selectedPotionType]++;
            Debug.Log(potionType.ToString());

            SelectionOfRadiusPrefabnAudio();
            DisableRotation();

            UpdateCurrentEffectText();
            buttonPressed = false;
        }
        else
        {
            menuMusic.PlayOneShot(noPotionSound);
        }
    }

    public void SelectPotion(Potion.PotionType potionType)
    {
        menuMusic.PlayOneShot(menuMusicClip);
        buttonPressed = true;

        selectedPotionType = potionType;
        Debug.Log(potionType.ToString());

        SelectionOfRadiusPrefabnAudio();
        DisableRotation();

        UpdateCurrentEffectText();
        buttonPressed = false;
    }

    // DoubleDamage
    public void BuyDoubleDamage() => BuyPotion(Potion.PotionType.DoubleDamage, 300);
    // HalveDamage
    public void BuyHalveDamage() => BuyPotion(Potion.PotionType.HalveDamage, 300);
    // Heal
    public void BuyHeal() => BuyPotion(Potion.PotionType.Heal, 300);
    // HalveHealth
    public void BuyHalveHealth() => BuyPotion(Potion.PotionType.HalveHealth, 300);
    // DoubleStats
    public void BuyDoubleStats() => BuyPotion(Potion.PotionType.DoubleStats, 300);
    // HalveStats
    public void BuyHalveStats() => BuyPotion(Potion.PotionType.HalveStats, 300);
    // RandomStatIncrease
    public void BuyRandomStatIncrease() => BuyPotion(Potion.PotionType.RandomStatIncrease, 300);
    // Giantify
    public void BuyGiantify() => BuyPotion(Potion.PotionType.Giantify, 300);
    // Shield
    public void BuyShield() => BuyPotion(Potion.PotionType.Shield, 300);
    // Regeneration
    public void BuyRegeneration() => BuyPotion(Potion.PotionType.Regeneration, 300);
    // Iron Skin
    public void BuyIronSkin() => BuyPotion(Potion.PotionType.IronSkin, 300);

    // DoubleDamage
    public void DoubleDamage() => SelectPotion(Potion.PotionType.DoubleDamage);
    // HalveDamage
    public void HalveDamage() => SelectPotion(Potion.PotionType.HalveDamage);
    // Heal
    public void Heal() => SelectPotion(Potion.PotionType.Heal);
    // HalveHealth
    public void HalveHealth() => SelectPotion(Potion.PotionType.HalveHealth);
    // DoubleStats
    public void DoubleStats() => SelectPotion(Potion.PotionType.DoubleStats);
    // HalveStats
    public void HalveStats() => SelectPotion(Potion.PotionType.HalveStats);
    // RandomStatIncrease
    public void RandomStatIncrease() => SelectPotion(Potion.PotionType.RandomStatIncrease);
    // Giantify
    public void Giantify() => SelectPotion(Potion.PotionType.Giantify);
    // Shield
    public void Shield() => SelectPotion(Potion.PotionType.Shield);
    // Regeneration
    public void Regeneration() => SelectPotion(Potion.PotionType.Regeneration);
    // Iron Skin
    public void IronSkin() => SelectPotion(Potion.PotionType.IronSkin);

    public void CyclePotionType()
    {
        menuMusic.PlayOneShot(menuMusicClip);
        buttonPressed = true;
        // Get all values of the PotionType enum
        Potion.PotionType[] potionTypes = (Potion.PotionType[])System.Enum.GetValues(typeof(Potion.PotionType));
        // Find the index of the current selected potion type
        int currentIndex = System.Array.IndexOf(potionTypes, selectedPotionType);
        // Cycle to the next potion type
        selectedPotionType = potionTypes[(currentIndex + 1) % potionTypes.Length];        

        SelectionOfRadiusPrefabnAudio();
        DisableRotation();

        // Update the UI text
        UpdateCurrentEffectText();
        buttonPressed = false;
    }

    public void SelectionOfRadiusPrefabnAudio()
    {
        if (selectedPotionType.ToString() == "Heal" || selectedPotionType.ToString() == "Regeneration")
        {
            image.sprite = potionSprites[0];
            radiusPrefab = radiusPrefabs[0];
            potionPrefab = potionPrefabs[0];
            radiusAudioClip = radiusAudioClips[0];
        }
        else if (selectedPotionType.ToString() == "DoubleDamage" || selectedPotionType.ToString() == "DoubleStats" || selectedPotionType.ToString() == "RandomStatIncrease"
            || selectedPotionType.ToString() == "Giantify" || selectedPotionType.ToString() == "IronSkin" || selectedPotionType.ToString() == "Shield")
        {
            image.sprite = potionSprites[1];
            radiusPrefab = radiusPrefabs[1];
            potionPrefab = potionPrefabs[1];
            radiusAudioClip = radiusAudioClips[0];
        }
        else if (selectedPotionType.ToString() == "HalveDamage" || selectedPotionType.ToString() == "HalveHealth" || selectedPotionType.ToString() == "HalveStats")
        {
            image.sprite = potionSprites[2];
            radiusPrefab = radiusPrefabs[2];
            potionPrefab = potionPrefabs[2];
            radiusAudioClip = radiusAudioClips[1];
        }
    }
}
