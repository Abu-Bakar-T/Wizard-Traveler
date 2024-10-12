using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEditor;
using static Potion;
using static UnityEngine.Rendering.DebugUI;
using static Unit;
//using static UnityEngine.UI.CanvasScaler;

public class GameManager : MonoBehaviour
{
    public Transform allyParent;
    public Transform enemyParent;

    public TextMeshProUGUI allyCountText;
    public TextMeshProUGUI enemyCountText;
    public Slider allyHealthSlider; 
    public Slider enemyHealthSlider; 

    public GameObject spawnManager;

    private List<GameObject> allies;
    private List<GameObject> enemies;

    public DestroyUnits destroyUnits;

    public float alliesCount = 0;
    public float enemiesCount = 0;

    public int level;
    public int maxLevelUnlocked;
    public bool isGameActive = false;
    public bool isLevelCompleted = false;
    public bool isPlayerDead = false;
    public RuntimeAnimatorController deathAnimationController; // Added for death animation
    public static GameManager Instance; // A static reference to the GameManager instance

    public GameObject smokeParticle;
    public bool isGamePaused;

    public GameObject pauseMenu;
    [SerializeField] private AudioSource backgroundMusic;
    public AudioSource menuMusic;
    public AudioClip menuMusicClip;

    public Image[] ImageObjects;
    public Sprite[] spritesForImage;
    public TextMeshProUGUI[] spritesText;
    public TextMeshProUGUI[] countText;

    public Image[] buyImageObjects;
    public TextMeshProUGUI[] buyTexts;
    public TextMeshProUGUI pointsText;
    public int points;


    public PotionType[] availablePotions;/* ={
            PotionType.DoubleDamage,
            PotionType.HalveDamage,
            PotionType.Heal,
            PotionType.HalveHealth,
            PotionType.DoubleStats,
            PotionType.HalveStats,
            PotionType.RandomStatIncrease,
            PotionType.Giantify,
            PotionType.Shield,
            PotionType.Regeneration,
            PotionType.IronSkin
        };*/
    public Dictionary<Potion.PotionType, int> availablePotionCounts = new Dictionary<Potion.PotionType, int>();
    void Start()
    {
        isGamePaused = false;
        isGameActive = true;
        destroyUnits = gameObject.AddComponent<DestroyUnits>();
        destroyUnits.alliesParent = allyParent;
        destroyUnits.enemiesParent = enemyParent;
        destroyUnits.deathAnimationController = deathAnimationController;
        destroyUnits.smokeParticle = smokeParticle;

        // Find the GameObject with the tag "BackgroundMusic"
        GameObject audioSourceObject = GameObject.FindGameObjectWithTag("Background Music");

        if (audioSourceObject != null)
        {
            // Get the AudioSource component from the found GameObject
            backgroundMusic = audioSourceObject.GetComponent<AudioSource>();
            backgroundMusic.volume = 0.2f;

            if (backgroundMusic == null)
            {
                Debug.LogError("No AudioSource component found on the GameObject with tag 'BackgroundMusic'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject with tag 'BackgroundMusic' found in the scene.");
        }

        Awake();
    }
    
    // Method to set available potions based on the level
    void SetAvailablePotions(int level)
    {
        switch (level)
        {
            case 0:
                availablePotions = new PotionType[]
                {
                    PotionType.Heal,
                    PotionType.DoubleDamage,
                    PotionType.Shield
                };
                break;
            case 1:
                availablePotions = new PotionType[]
                {
                    PotionType.Giantify,
                    PotionType.Shield,
                    PotionType.HalveHealth
                };
                break;
            case 2:
                availablePotions = new PotionType[]
                {
                    PotionType.Shield,
                    PotionType.Giantify,
                    PotionType.Regeneration
                };
                break;
            case 3:
                availablePotions = new PotionType[]
                {
                    PotionType.HalveHealth,
                    PotionType.DoubleStats,
                    PotionType.Regeneration
                };
                break;
            case 4:
                availablePotions = new PotionType[]
                {
                    PotionType.Giantify,
                    PotionType.Regeneration,
                    PotionType.IronSkin
                };
                break;
            case 5:
                availablePotions = new PotionType[]
                {
                    PotionType.HalveStats,
                    PotionType.RandomStatIncrease,
                    PotionType.Giantify,
                };
                break;
            case 6:
                availablePotions = new PotionType[]
                {
                    PotionType.HalveStats,
                    PotionType.Heal,
                    PotionType.Giantify,
                };
                break;
            case 7:
                availablePotions = new PotionType[]
                {
                    PotionType.Regeneration,
                    PotionType.Heal,
                    PotionType.Giantify,
                };
                break;
            // Add more cases for additional levels
            default:
                availablePotions = new PotionType[]
                {
                    PotionType.Heal,
                    PotionType.Giantify,
                    PotionType.HalveHealth
                };
                break;
        }
        // Reset potion counts based on the selected availablePotions
        availablePotionCounts.Clear();
        foreach (Potion.PotionType potion in availablePotions)
        {
            availablePotionCounts[potion] = 1;  // Set initial count to 1
        }
    }

    void UpdateCount()
    {
        if (availablePotions.Length != ImageObjects.Length || availablePotions.Length != spritesText.Length)
        {
            Debug.LogError("Mismatch in lengths of availablePotions, ImageObjects, or spritesText arrays.");
            return;
        }

        for (int i = 0; i < availablePotions.Length; i++)
        {
            PotionType potion = availablePotions[i];

            Debug.Log($"Setting sprite and text for potion: {potion}");
            countText[i].text = availablePotionCounts[potion].ToString();
        }
        pointsText.text = points.ToString();
    }
    void SetSprites()
    {
        if (availablePotions.Length != ImageObjects.Length || availablePotions.Length != spritesText.Length)
        {
            Debug.LogError("Mismatch in lengths of availablePotions, ImageObjects, or spritesText arrays.");
            return;
        }

        for (int i = 0; i < availablePotions.Length; i++)
        {
            PotionType potion = availablePotions[i];

            Debug.Log($"Setting sprite and text for potion: {potion}");
            countText[i].text = availablePotionCounts[potion].ToString();
            switch (potion)
            {
                case PotionType.Heal:
                    ImageObjects[i].sprite = spritesForImage[2];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "Heal";

                    buyImageObjects[i].sprite = spritesForImage[2];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "Heal";
                    break;
                case PotionType.HalveHealth:
                    ImageObjects[i].sprite = spritesForImage[6];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "/2";

                    buyImageObjects[i].sprite = spritesForImage[6];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "/2";
                    break;
                case PotionType.DoubleDamage:
                    ImageObjects[i].sprite = spritesForImage[1];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "x2";

                    buyImageObjects[i].sprite = spritesForImage[1];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "x2";
                    break;
                case PotionType.HalveDamage:
                    ImageObjects[i].sprite = spritesForImage[5];
                    ImageObjects[i].color = Color.red;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "/2";

                    buyImageObjects[i].sprite = spritesForImage[5];
                    buyImageObjects[i].color = Color.red;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "/2";
                    break;
                case PotionType.DoubleStats:
                    ImageObjects[i].sprite = spritesForImage[3];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "x2";

                    buyImageObjects[i].sprite = spritesForImage[3];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "x2";
                    break;
                case PotionType.HalveStats:
                    ImageObjects[i].sprite = spritesForImage[4];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "/2";

                    buyImageObjects[i].sprite = spritesForImage[4];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "/2";
                    break;
                case PotionType.RandomStatIncrease:
                    ImageObjects[i].sprite = spritesForImage[8];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "?";

                    buyImageObjects[i].sprite = spritesForImage[8];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "?";
                    break;
                case PotionType.Giantify:
                    ImageObjects[i].sprite = spritesForImage[7];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.white;
                    spritesText[i].text = "Giga";

                    buyImageObjects[i].sprite = spritesForImage[7];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.white;
                    buyTexts[i].text = "Giga";
                    break;
                case PotionType.Shield:
                    ImageObjects[i].sprite = spritesForImage[0];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "+1000";

                    buyImageObjects[i].sprite = spritesForImage[0];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "+1000";
                    break;
                case PotionType.Regeneration:
                    ImageObjects[i].sprite = spritesForImage[2];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "Regen";

                    buyImageObjects[i].sprite = spritesForImage[2];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "Regen";
                    break;
                case PotionType.IronSkin:
                    ImageObjects[i].sprite = spritesForImage[0];
                    ImageObjects[i].color = Color.white;
                    spritesText[i].color = Color.black;
                    spritesText[i].text = "+100";

                    buyImageObjects[i].sprite = spritesForImage[0];
                    buyImageObjects[i].color = Color.white;
                    buyTexts[i].color = Color.black;
                    buyTexts[i].text = "+100";
                    break;
                default:
                    Debug.LogWarning($"No sprite found for potion: {potion}");
                    break;
            }
        }
    }

    
    public int GetPotionCount(Potion.PotionType potionType)
    {
        if (availablePotionCounts.ContainsKey(potionType))
        {
            return availablePotionCounts[potionType];
        }
        return 0; // Return 0 if the potion type is not found
    }

    void Awake()
    {
        Instance = this;
        SetAvailablePotions(level);
        availablePotionCounts = new Dictionary<Potion.PotionType, int>();
        foreach (Potion.PotionType potion in availablePotions)
        {
            availablePotionCounts[potion] = 1;  // Start with 1 instance of each potion
        }

        // Check if it's the first time the game is started
        points = PlayerPrefs.GetInt("Game Points");
                
        SetSprites();
        UpdateUnitLists();
        UpdateDisplay(); // Initial display update

        // Save the last played level
        PlayerPrefs.SetInt("LastPlayedLevel", level);

        // Ensure the data is saved
        PlayerPrefs.Save();
    }

    void LateUpdate()
    {
        if (isGamePaused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
            if (isGameActive)
            {
                UpdateUnitLists();
                UpdateDisplay(); // Update display each frame or as needed
                UpdateCount();
            }
        }
    }

    // No longer Required
    /*public void PauseMenu()
    {
        menuMusic.PlayOneShot(menuMusicClip);
        isGamePaused = true;
        backgroundMusic.volume = 0.5f;
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
        isGameActive = false;
    }*/

        public void UpdateUnitLists()
    {
        allies = new List<GameObject>();
        enemies = new List<GameObject>();

        foreach (Transform ally in allyParent)
        {
            if (ally.GetComponent<Unit>() != null)
            {
                allies.Add(ally.gameObject);
            }
        }

        foreach (Transform enemy in enemyParent)
        {
            if (enemy.GetComponent<Unit>() != null)
            {
                enemies.Add(enemy.gameObject);
            }
        }
    }

    public void RemoveAlly(GameObject ally)
    {
        if (allies.Contains(ally))
        {
            destroyUnits.PlayDeathAnimationAndDestroy(ally);
            allies.Remove(ally);
        }
        UpdateDisplay(); // Update display after removal
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            destroyUnits.PlayDeathAnimationAndDestroy(enemy);
            enemies.Remove(enemy);
        }
        UpdateDisplay(); // Update display after removal
    }

    public List<GameObject> GetAllies()
    {
        return allies;
    }

    public List<GameObject> GetEnemies()
    {
        return enemies;
    }

    void UpdateDisplay()
    {
        if (allyCountText != null)
        {
            allyCountText.text = $"{allies.Count}";
            alliesCount = allies.Count;
        }

        if (enemyCountText != null)
        {
            enemyCountText.text = $"{enemies.Count}";
            enemiesCount = enemies.Count;
        }

        if (allyHealthSlider != null)
        {
            float totalAllyHealth = 0;
            float totalMaxAllyHealth = 0;

            foreach (var ally in allies)
            {
                var unit = ally.GetComponent<Unit>();
                if (unit != null && unit.unitStats != null)
                {
                    totalAllyHealth += unit.unitStats.Health;
                    totalMaxAllyHealth += unit.unitStats.MaxHealth;
                    Debug.Log($"Ally {ally.name} Health: {unit.unitStats.Health} / {unit.unitStats.MaxHealth}");
                }
                else
                {
                    Debug.LogWarning($"Ally {ally.name} does not have a Unit component or UnitStats is not assigned.");
                }
            }
            allyHealthSlider.value = totalAllyHealth / totalMaxAllyHealth;
        }

        if (enemyHealthSlider != null)
        {
            float totalEnemyHealth = 0;
            float totalMaxEnemyHealth = 0;

            foreach (var enemy in enemies)
            {
                var unit = enemy.GetComponent<Unit>();
                if (unit != null && unit.unitStats != null)
                {
                    totalEnemyHealth += unit.unitStats.Health;
                    totalMaxEnemyHealth += unit.unitStats.MaxHealth;
                    Debug.Log($"Enemy {enemy.name} Health: {unit.unitStats.Health} / {unit.unitStats.MaxHealth}");
                }
                else
                {
                    Debug.LogWarning($"Enemy {enemy.name} does not have a Unit component or UnitStats is not assigned.");
                }
            }
            enemyHealthSlider.value = totalEnemyHealth / totalMaxEnemyHealth;
        }

        if (isLevelCompleted)
        {
            backgroundMusic.volume = 0.5f; 
            CompleteLevel(level);
            isGameActive = false;
        }
    }

    public void PlayerDeath()
    {
        isGameActive = false;
        isLevelCompleted = false;
        isPlayerDead = true;
    }

    public void CompleteLevel(int level)
    {
        // Check if this is the highest level unlocked so far
         maxLevelUnlocked = PlayerPrefs.GetInt("MaxLevelUnlocked", 0);
        if (level >= maxLevelUnlocked)
        {
            PlayerPrefs.SetInt("MaxLevelUnlocked", level + 1);  // Unlock the next level
        }

        PlayerPrefs.SetInt("Game Points", points);

        // Save the last played level
        PlayerPrefs.SetInt("LastPlayedLevel", level);

        // Ensure the data is saved
        PlayerPrefs.Save();
    }

}
