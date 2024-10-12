using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class PotionLanding : MonoBehaviour
{
    public PotionThrower potionThrower; // Reference to the potionThrower
    public GameObject radiusPrefab; // Prefab for the radius visualization
    public AudioClip radiusAudioClip;
    public Animator animator;
    public AudioSource radiusAudioSource;
    public TextMeshProUGUI radiusText; // TextMeshProUGUI element in the Canvas
    public Potion.PotionType potionType; // Potion type reference

    private GameObject radiusInstance;
    public AudioSource potioninAirAudiosource;
    private bool effectApplied = false;

    void LateUpdate()
    {
        if (transform.position.y < -2.5f && !effectApplied)
        {
            ApplyEffectAndCleanup();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !effectApplied)
        {
            ApplyEffectAndCleanup();
        }
    }

    void ApplyEffectAndCleanup()
    {
        if (effectApplied) return; // Prevent multiple applications of the effect
        effectApplied = true; // Mark effect as applied

        if (potionThrower == null)
        {
            Debug.LogWarning("PotionThrower is not assigned.");
            return;
        }

        CreateRadiusVisual();
        ApplyPotionEffect();

        // Start cleanup process before notifying the PotionThrower
        StartCleanupProcess();
    }

    void CreateRadiusVisual()
    {
        if (radiusPrefab != null)
        {
            radiusInstance = Instantiate(radiusPrefab, transform.position, Quaternion.identity);
            potioninAirAudiosource.Stop();
            radiusAudioSource.PlayOneShot(radiusAudioClip);
            Handheld.Vibrate();
            //radiusInstance.transform.localScale = new Vector3(potionThrower.radius * 2, 1, potionThrower.radius * 2);
        }

        if (radiusText != null)
        {
            radiusText.text = $"{potionType}";
            radiusText.gameObject.SetActive(true);
        }

        if (animator != null)
        {
            //animator.SetBool("Throw", false);
            //animator.SetBool("Idle", true);
        }
    }

    void ApplyPotionEffect()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, potionThrower.radius);
        Potion potion = GetComponent<Potion>();

        if (potion != null)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Ally") )//|| hitCollider.CompareTag("Enemy"))
                {
                    Unit unit = hitCollider.GetComponent<Unit>();
                    if (unit != null && unit)
                    {
                        Unit.EffectType effectType = Unit.ConvertPotionTypeToEffectType(potion.currentPotionType);
                        if (!unit.IsEffectActive(effectType))
                        {
                            potion.ApplyEffect(hitCollider.gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Potion component is not attached to the game object.");
        }
    }

    void StartCleanupProcess()
    {
        // Start the cleanup process
        StartCoroutine(CleanupVisuals());
    }
    void DisableChildRenderers()
    {
        // Get all child MeshRenderers
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();

        // Disable each child's MeshRenderer
        foreach (MeshRenderer renderer in childRenderers)
        {
            renderer.enabled = false;
        }
    }
    IEnumerator CleanupVisuals()
    {
        // Disable MeshRenderer on all child objects
        DisableChildRenderers();
        yield return new WaitForSeconds(1); // Wait for 1 second

        // Disable the radius visual and text
        if (radiusInstance != null)
        {
            Destroy(radiusInstance);
        }

        if (radiusText != null)
        {
            radiusText.gameObject.SetActive(false);
        }

        // Notify the PotionThrower script
        potionThrower.PotionLanded(gameObject);
    }
}