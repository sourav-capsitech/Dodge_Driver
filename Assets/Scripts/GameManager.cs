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
    public TextMeshProUGUI highScoreText; // Reference for displaying the High Score
    public GameObject gameLogo; // Sprite or GameObject for the game title screen
    public GameObject gameOverLogo; // Sprite or GameObject for the Game Over title
    public GameObject gameOverPanel;
    public Button playButton;
    public Button restartButton;
    public Button exitButton; // NEW: Reference for the Exit/Main Menu Button

    [Header("Audio References")]
    // AudioSources should be attached to the GameManager GameObject
    public AudioSource backgroundMusicSource;
    public AudioSource collisionSoundSource;
    public AudioClip collisionSoundClip; // The actual sound file to play

    // --- Component Reference ---
    private SpawnManager spawnManager; // Reference to the SpawnManager script

    // --- Game State Variables ---
    private int score = 0;
    private int highScore = 0; // Variable to store the highest score
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
        if (gameOverLogo != null) // Hide Game Over Logo
        {
            gameOverLogo.SetActive(false);
        }
        
        // 2. Show Start UI elements (These will be immediately hidden in Start() if it's a direct restart)
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
            // Hide the restart button initially
            restartButton.gameObject.SetActive(false); 
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        // 4. Hook up Exit Button (NEW)
        if (exitButton != null)
        {
            exitButton.gameObject.SetActive(false); // Hide initially
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitToMainMenu); // Set the new main menu function
        }
    }

    private void Start()
    {
        // 1. Check if this is a direct restart (value 1 means yes)
        bool isDirectRestart = PlayerPrefs.GetInt("IsRestart", 0) == 1;

        // 2. IMPORTANT: Clear the flag immediately so the *next* scene reload (e.g., after Game Over) 
        // goes back to the standard title screen flow.
        PlayerPrefs.SetInt("IsRestart", 0);
        
        // Load High Score from PlayerPrefs. Default to 0 if none found. (This always runs)
        highScore = PlayerPrefs.GetInt("HighScore", 0); 
        UpdateScoreText();

        if (isDirectRestart)
        {
            // === DIRECT RESTART LOGIC (From Restart button) ===
            // Skip the title screen and start immediately
            StartGame();
            
            // Explicitly ensure title screen UI elements are hidden
            if (gameLogo != null) gameLogo.SetActive(false);
            if (playButton != null) playButton.gameObject.SetActive(false);
        }
        else
        {
            // === INITIAL GAME START / TITLE SCREEN LOGIC (Standard Load or Exit to Menu) ===
            
            // Pause the game until the player clicks 'Play'
            Time.timeScale = 0f; 

            // Ensure spawner is paused/disabled initially
            if (spawnManager != null)
            {
                spawnManager.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Called when the player presses the initial Play button or immediately on a direct restart.
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

        // Check and Save High Score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save(); // Ensure data is written to disk immediately
            UpdateScoreText(); // Update the display with the new high score
        }

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
        
        // 2. Play collision sound 
        if (collisionSoundSource != null && collisionSoundClip != null)
        {
            collisionSoundSource.PlayOneShot(collisionSoundClip);
        }
        
        // 3. Show Game Over UI (Panel, Logo, and Buttons)
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        if (gameOverLogo != null) 
        {
            gameOverLogo.SetActive(true);
        }
        if (restartButton != null) 
        {
            restartButton.gameObject.SetActive(true);
        }
        if (exitButton != null) // Show Exit Button
        {
            exitButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the player presses the Restart button.
    /// Sets the 'IsRestart' flag and reloads the scene to skip the title screen.
    /// </summary>
    public void RestartGame()
    {
        // 1. Set a flag in PlayerPrefs to tell the next scene instance to skip the title screen
        PlayerPrefs.SetInt("IsRestart", 1);
        PlayerPrefs.Save(); 

        // 2. Reloads the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// MODIFIED: Called when the player presses the Exit button.
    /// Reloads the scene without the 'IsRestart' flag, ensuring a full environment reset
    /// and returning to the paused title screen state.
    /// </summary>
    public void ExitToMainMenu()
    {
        // 1. Ensure the 'IsRestart' flag is set to 0. 
        // This makes the newly loaded scene run the "else" block in Start(), 
        // which pauses the game and shows the title screen UI.
        PlayerPrefs.SetInt("IsRestart", 0);
        PlayerPrefs.Save(); 

        // 2. Reloads the current scene to cleanly reset all objects and scripts.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateScoreText()
    {
        // Update Current Score
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        
        // Update High Score
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }
}
