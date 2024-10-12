using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    private Dictionary<EffectType, int> activeEffects = new Dictionary<EffectType, int>();
    private Dictionary<EffectType, float> lastEffectAppliedTime = new Dictionary<EffectType, float>();

    public UnitStats unitStats;
    public float attackCooldown;
    private GameObject currentTarget;

    public bool isSlashSoundPlayed = false;
    public float currentHealth;
    private float cumulativeDamage = 0f;
    private float lastAttackTime;
    public float effectDuration = 5f;
    public bool isDying = false;
    public bool animationEnabled = false;
    public GameObject targetLockedon;

    public bool effectActive = false;
    public RuntimeAnimatorController deathAnimationController; // Added for death animation
    [SerializeField] private HealthBar healthBar;
    public GameObject[] prefabs;
    private int prefabIndex = 0;

    //Audio
    public AudioClip spawnSound;
    public AudioClip slashsound;
    public AudioClip deathClip;
    public AudioSource AudioSource;
    public AudioSource footStepSource;
    public AudioSource allyStepSource;
    public AudioSource enemyStepSource;

    public GameObject CurrentTarget
    {
        get { return currentTarget; }
        set { currentTarget = value; }
    }

    public enum EffectType
    {
        DoubleDamage,
        HalveDamage,
        Heal,
        HalveHealth,
        DoubleStats,
        HalveStats,
        RandomStatIncrease,
        Giantify,
        Shield,
        Regeneration,
        IronSkin,
    }

    void Start()
    {
        // Initialize lastEffectAppliedTime with negative values for each effect
        foreach (EffectType effect in System.Enum.GetValues(typeof(EffectType)))
        {
            lastEffectAppliedTime[effect] = -3f; // Initialize so the effect can be applied immediately
        }

        // Assuming this script is attached to the unit with the particle system
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(); // Ensure the particle system is stopped at the start
        }

        if (unitStats == null)
        {
            Debug.LogError("UnitStats not assigned!");
            unitStats = new UnitStats(); // Create a new instance of UnitStats if not assigned
        }

        currentHealth = unitStats.Health;
        attackCooldown = 1 / unitStats.AttackSpeed;
        lastAttackTime = Time.time;
        UpdateHealthUI();

        // Find the Audio Manager GameObject
        GameObject audioManager = GameObject.FindGameObjectWithTag("Audio Manager");

        if (audioManager != null)
        {
            // Find the child GameObject with the tag "AllyFootSteps" under the Audio Manager
            Transform allyFootSteps = audioManager.transform.Find("AllyFootSteps");
            if (allyFootSteps != null)
            {
                allyStepSource = allyFootSteps.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("AllyFootSteps object not found.");
            }

            // Find the child GameObject with the tag "EnemyFootSteps" under the Audio Manager
            Transform enemyFootSteps = audioManager.transform.Find("EnemyFootSteps");
            if (enemyFootSteps != null)
            {
                enemyStepSource = enemyFootSteps.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("EnemyFootSteps object not found.");
            }
        }
        else
        {
            Debug.LogError("Audio Manager object not found.");
        }

        // Assign the appropriate footstep sound based on the unit's tag
        AssignFootstepSound();
    }


    void LateUpdate()
    {
        if (!isDying)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < 0)
            {
                attackCooldown = 0;
            }
        }
        currentHealth = unitStats.Health;
        UpdateHealthUI();
    }

    public bool IsEffectActive(EffectType effectType)
    {
        return activeEffects.ContainsKey(effectType) && activeEffects[effectType] > 0;
    }

    public void ApplyStatMultiplier(float multiplier)
    {
        unitStats.AttackDamage *= multiplier;
        unitStats.Health *= multiplier;
        currentHealth = unitStats.Health;
        unitStats.MaxHealth *= multiplier;
        unitStats.Armour *= multiplier;
        unitStats.Speed *= multiplier;
        unitStats.Stamina *= multiplier;
        unitStats.attributes.Strength *= multiplier;
        unitStats.attributes.Agility *= multiplier;
        unitStats.attributes.Intelligence *= multiplier;
        unitStats.attributes.Luck *= multiplier;
    }

    public void ApplyRandomStatIncrease()
    {
        int statIndex = Random.Range(0, 4); // Assuming 4 stats to choose from
        switch (statIndex)
        {
            case 0:
                unitStats.Health += 20;
                currentHealth = unitStats.Health;
                unitStats.MaxHealth += 20;
                break;
            case 1:
                unitStats.Armour += 5;
                break;
            case 2:
                unitStats.AttackDamage += 10;
                break;
            case 3:
                unitStats.AttackSpeed += 2;
                break;
        }
    }

    public void Heal(float amount)
    {
        unitStats.Health += amount;
        if (unitStats.Health > unitStats.MaxHealth)
        {
            unitStats.Health = unitStats.MaxHealth;
        }
        currentHealth = unitStats.Health;
        effectActive = false;
        UpdateHealthUI();
    }

    public void HalveHealth()
    {
        Debug.Log($"Before HalveHealth: {unitStats.Health}");
        unitStats.Health = Mathf.Max(1, unitStats.Health / 2);
        currentHealth = unitStats.Health;
        effectActive = false;
        Debug.Log($"After HalveHealth: {unitStats.Health}");
        UpdateHealthUI();
    }

    IEnumerator RegenerateHealth()
    {
        for (int i = 0; i < effectDuration; i++)
        {
            if (currentHealth < unitStats.MaxHealth)
            {
                Heal(30); // Heal 30 HP every second
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(unitStats.Health, unitStats.MaxHealth);
        }
    }

    public void ApplyTemporaryEffect(EffectType effectType)
    {
        // Ensure effect can be reapplied after 3 seconds
        float timeSinceLastApplication = Time.time - lastEffectAppliedTime[effectType];
        if (timeSinceLastApplication < 3f)
        {
            Debug.Log($"Effect {effectType} cannot be applied again yet. Time remaining: {3f - timeSinceLastApplication} seconds.");
            return;
        }

        // Reset effect application timer
        lastEffectAppliedTime[effectType] = Time.time;

        // Apply the effect
        if (!activeEffects.ContainsKey(effectType))
        {
            activeEffects[effectType] = 0;
        }

        activeEffects[effectType]++;
        effectActive = true;

        Debug.Log($"Effect {effectType} activated {activeEffects[effectType]} time(s).");

        // Handle the specific effect application
        switch (effectType)
        {
            case EffectType.Heal:
                Heal(100);
                break;
            case EffectType.HalveHealth:
                HalveHealth();
                break;
            case EffectType.DoubleDamage:
                unitStats.AttackDamage *= 2;
                break;
            case EffectType.HalveDamage:
                unitStats.AttackDamage /= 2;
                break;
            case EffectType.DoubleStats:
                ApplyStatMultiplier(2);
                break;
            case EffectType.HalveStats:
                ApplyStatMultiplier(0.5f);
                break;
            case EffectType.RandomStatIncrease:
                ApplyRandomStatIncrease();
                break;
            case EffectType.Giantify: // have error
                unitStats.Health *= 10;
                currentHealth = unitStats.Health;
                unitStats.MaxHealth *= 10;
                unitStats.Armour *= 500;
                unitStats.AttackDamage *= 100;
                unitStats.AttackSpeed -= 10f;


                // Detach audio sources
                //AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
                //foreach (AudioSource audioSource in audioSources)
                //{
                    //audioSource.transform.parent = null;
                //}

                // Scaling the object
                Vector3 scaleIncrement = new Vector3(0.5f, 0.5f, 0.5f); // Amount to increase scale
                Vector3 maxScale = new Vector3(2.5f, 2.5f, 2.5f); // Maximum allowed scale
                float moveUpAmount = 2f; // Amount to move the object up before scaling

                Vector3 newScale = transform.localScale + scaleIncrement;
                newScale = Vector3.Min(newScale, maxScale); // Ensure the new scale doesn't exceed the maximum scale

                // Move the object up before scaling
                Vector3 originalPosition = transform.position;
                transform.position += new Vector3(0, moveUpAmount, 0);
                transform.localScale = newScale;

                // Reattach audio sources
                //foreach (AudioSource audioSource in audioSources)
                //{
                    //audioSource.transform.parent = transform;
                //}
                break;
            case EffectType.Shield:
                unitStats.Armour += 1000;
                break;
            case EffectType.Regeneration:
                StartCoroutine(RegenerateHealth());
                break;
            case EffectType.IronSkin:
                unitStats.Armour += 100;
                break;
        }

        // Start coroutine to remove the effect after the duration
        StartCoroutine(RemoveEffectAfterDuration(effectType, effectDuration));
    }

    IEnumerator RemoveEffectAfterDuration(EffectType effectType, float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Removing " + effectType);

        if (activeEffects.ContainsKey(effectType))
        {
            activeEffects[effectType]--;

            if (activeEffects[effectType] == 0)
            {
                activeEffects.Remove(effectType);
                Debug.Log($"Effect {effectType} has expired.");

                // Reverse the effect (if necessary)
                switch (effectType)
                {
                    case EffectType.DoubleDamage:
                        unitStats.AttackDamage /= 2;
                        break;
                    case EffectType.HalveDamage:
                        unitStats.AttackDamage *= 2;
                        break;
                    case EffectType.DoubleStats:
                        ApplyStatMultiplier(0.5f);
                        break;
                    case EffectType.HalveStats:
                        ApplyStatMultiplier(2);
                        break;
                    case EffectType.Giantify:  // have error
                        unitStats.Health /= 10;
                        currentHealth = unitStats.Health;
                        unitStats.MaxHealth /= 10;
                        unitStats.Armour /= 500;
                        unitStats.AttackDamage /= 100;
                        unitStats.AttackSpeed += 10f;
                        attackCooldown = 0;

                        // Detach audio sources
                        //AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
                        //foreach (AudioSource audioSource in audioSources)
                        //{
                            //audioSource.transform.parent = null;
                        //}

                        // Descaling the object
                        Vector3 scaleDecrement = new Vector3(0.5f, 0.5f, 0.5f); // Amount to increase scale
                        Vector3 minScale = new Vector3(1f, 1f, 1f); // Maximum allowed scale
                        float moveUpAmount = 2f; // Amount to move the object up before scaling

                        Vector3 newScale = transform.localScale - scaleDecrement;
                        newScale = Vector3.Max(newScale, minScale); // Ensure the new scale doesn't exceed the maximum scale

                        // Move the object up before scaling
                        Vector3 originalPosition = transform.position;
                        transform.position += new Vector3(0, moveUpAmount, 0);
                        transform.localScale = newScale;

                        // Reattach audio sources
                        //foreach (AudioSource audioSource in audioSources)
                        //{
                            //audioSource.transform.parent = transform;
                        //}
                        break;
                    case EffectType.Shield:
                        unitStats.Armour -= 200;
                        break;
                    case EffectType.IronSkin:
                        unitStats.Armour -= 100;
                        break;
                }
            }
        }
    }

    public bool CanAttack()
    {
        return attackCooldown <= 0 && IsEnemyInFront();
    }

    public void ResetAttackCooldown()
    {
        attackCooldown = 1 / unitStats.AttackSpeed;
        lastAttackTime = Time.time;
    }

    private bool IsEnemyInFront()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.0f);

        if (hit.collider != null)
        {
            if (tag == "Ally" && hit.collider.CompareTag("Ally"))
            {
                return false; // Don't attack if there's an ally in front
            }
            else if (tag == "Enemy" && hit.collider.CompareTag("Enemy"))
            {
                return false; // Don't attack if there's an enemy in front
            }
        }

        return true; // Attack if no ally/enemy is in front
    }

    // Method to assign the appropriate footstep sound based on the unit's tag
    private void AssignFootstepSound()
    {
        if (CompareTag("Ally"))
        {
            footStepSource = allyStepSource;
        }
        else if (CompareTag("Enemy"))
        {
            footStepSource = enemyStepSource;
        }
    }

    // Call this method in animations or movement functions where the footstep sound should play
    public void PlayFootstepSound()
    {
        if (footStepSource != null)
        {
            footStepSource.Play();
        }
    }

    // Conversion method from Potion.PotionType to Unit.EffectType
    public static EffectType ConvertPotionTypeToEffectType(Potion.PotionType potionType)
    {
        switch (potionType)
        {
            case Potion.PotionType.DoubleDamage:
                return EffectType.DoubleDamage;
            case Potion.PotionType.HalveDamage:
                return EffectType.HalveDamage;
            case Potion.PotionType.Heal:
                return EffectType.Heal;
            case Potion.PotionType.HalveHealth:
                return EffectType.HalveHealth;
            case Potion.PotionType.DoubleStats:
                return EffectType.DoubleStats;
            case Potion.PotionType.HalveStats:
                return EffectType.HalveStats;
            case Potion.PotionType.RandomStatIncrease:
                return EffectType.RandomStatIncrease;
            case Potion.PotionType.Giantify:
                return EffectType.Giantify;
            case Potion.PotionType.Shield:
                return EffectType.Shield;
            case Potion.PotionType.Regeneration:
                return EffectType.Regeneration;
            case Potion.PotionType.IronSkin:
                return EffectType.IronSkin;
            default:
                throw new System.ArgumentException("Unknown potion type");
        }
    }
}