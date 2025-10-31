using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Button
using UnityEngine.SceneManagement; // Required for reloading the scene
using TMPro; // Required for TextMeshPro UI components

public class GameManager : MonoBehaviour
{
    // --- Public References (Must be set in Inspector) ---

    [Header("UI References")]
    // Uses TextMeshProUGUI for modern text components
    public TextMeshProUGUI scoreText; 
    public GameObject gameLogo; // Sprite or GameObject for the game title screen
    public GameObject gameOverLogo; // NEW: Sprite or GameObject for the Game Over title
    public GameObject gameOverPanel;
    public Button playButton;
    public Button restartButton;

    [Header("Audio References")]
    // AudioSources should be attached to the GameManager GameObject
    public AudioSource backgroundMusicSource;
    public AudioSource collisionSoundSource;
    public AudioClip collisionSoundClip; // The actual sound file to play

    // --- Component Reference ---
    private SpawnManager spawnManager; // Reference to the SpawnManager script

    // --- Game State Variables ---
    private int score = 0;
    private bool isGameOver = false;
    private bool isGameActive = false;

    // --- Singleton Pattern ---
    public static GameManager Instance;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            // Ensure the GameManager persists across scenes if needed, though for a simple restart we don't need DontDestroyOnLoad
        }
        else
        {
            Destroy(gameObject);
        }

        // Get reference to the SpawnManager
        spawnManager = FindAnyObjectByType<SpawnManager>();
        
        // --- INITIAL UI SETUP (Game Start State) ---
        
        // 1. Hide Game Over UI elements
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (gameOverLogo != null) // NEW: Hide Game Over Logo
        {
            gameOverLogo.SetActive(false);
        }
        
        // 2. Show Start UI elements
        if (gameLogo != null)
        {
            gameLogo.SetActive(true);
        }
        if (playButton != null)
        {
            playButton.gameObject.SetActive(true);
            // Hook up the click event for the initial Play button
            playButton.onClick.RemoveAllListeners(); // Prevent duplicate listeners
            playButton.onClick.AddListener(StartGame);
        }

        // 3. Hook up Restart Button (control individually if outside the panel)
        if (restartButton != null)
        {
            // If the restart button is outside the panel, hide it here.
            // If it's inside gameOverPanel, this line is optional but harmless.
            restartButton.gameObject.SetActive(false); 
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void Start()
    {
        // Pause the game until the player clicks 'Play'
        Time.timeScale = 0f; 
        UpdateScoreText();

        // Ensure spawner is paused/disabled initially
        if (spawnManager != null)
        {
            spawnManager.enabled = false;
        }
    }
    
    /// <summary>
    /// Called when the player presses the initial Play button.
    /// </summary>
    public void StartGame()
    {
        // 1. Hide Start UI elements
        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
        }
        if (gameLogo != null)
        {
            gameLogo.SetActive(false);
        }
        
        // 2. Start Game Logic
        isGameActive = true;
        Time.timeScale = 1f; // Resume normal time flow

        // Enable the spawner when the game starts
        if (spawnManager != null)
        {
            spawnManager.enabled = true;
        }
        
        // 3. Start background music
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Play();
        }
    }

    /// <summary>
    /// Called by player or object scripts when a collision with a coin occurs.
    /// </summary>
    /// <param name="coin">The coin GameObject to be destroyed.</param>
    public void CollectCoin(GameObject coin)
    {
        if (isGameActive && !isGameOver)
        {
            score++;
            UpdateScoreText();
            Destroy(coin); // Destroy the coin after collection
            
            // Optional: Play coin collection sound here if you add a reference for it
        }
    }

    /// <summary>
    /// Called by player scripts when a collision with a car occurs.
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return; // Prevent multiple calls

        isGameOver = true;
        isGameActive = false;
        Time.timeScale = 0f; // Pause the game

        // DISABLE the spawner when the game ends
        if (spawnManager != null)
        {
            spawnManager.enabled = false;
        }
        
        // 1. Stop background music
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }
        
        // 2. Play collision sound (using PlayOneShot for a non-looping sound effect)
        if (collisionSoundSource != null && collisionSoundClip != null)
        {
            collisionSoundSource.PlayOneShot(collisionSoundClip);
        }
        
        // 3. Show Game Over UI (Panel, Logo, and Button)
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        if (gameOverLogo != null) // NEW: Show Game Over Logo
        {
            gameOverLogo.SetActive(true);
        }
        if (restartButton != null) // NEW: Show Restart Button
        {
            restartButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the player presses the Restart button.
    /// </summary>
    public void RestartGame()
    {
        // Reloads the current scene. Make sure the scene is added to Build Settings!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
