using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MiniGameScript : MonoBehaviour
{
    // UI references and minigame objects
    [SerializeField] private GameObject minigame;
    [SerializeField] private GameObject miniGameMenu;
    [SerializeField] private PlayerInput playerInput;

    public Button goBackButton;
    public Button restartButton;
    public TMP_Text timerText;
    public TMP_Text compressionText;
    public TMP_Text feedbackText;

    // Streak tracking
    private int currentStreak = 0;
    private int maxStreak = 0;
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private TMP_Text maxStreakText;

    // Compression and timing variables
    private int numCompressions;
    private float timer;
    private float lastCompressionTime;
    private float currentCompressionInterval;

    // Compression timing limits
    private const float MIN_COMPRESSION_INTERVAL = 0.5f; // Below this = too fast
    private const float MAX_COMPRESSION_INTERVAL = 1.5f; // Above this = too slow

    // Enum to represent the game state
    private enum GameState { Idle, Running, Ended }
    private GameState currentState = GameState.Idle;

    void Start()
    {
        // Initialize values and set up button listeners
        numCompressions = 0;
        timer = 60f;
        feedbackText.text = "";
        UpdateUI();

        restartButton.onClick.AddListener(RestartGame);
        goBackButton.onClick.AddListener(GoBack);
    }

    private void Update()
    {
        // Start or register compression based on input
        if (playerInput.actions["Action"].WasPressedThisFrame())
        {
            if (currentState == GameState.Idle)
            {
                StartGame();
            }

            if (currentState == GameState.Running)
            {
                RegisterCompression();
            }
        }

        // Timer countdown while game is running
        if (currentState == GameState.Running)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 0f;
                EndGame();
            }
        }

        // Go back to main menu if Back input is pressed
        if (playerInput.actions["Back"].WasPressedThisFrame())
        {
            GoBack();
        }

        // Refresh UI every frame
        UpdateUI();
    }

    private void StartGame()
    {
        // Begin minigame, reset values
        currentState = GameState.Running;
        timer = 60f;
        numCompressions = 0;
        lastCompressionTime = Time.time;
        feedbackText.text = "Game started!";
    }

    private void EndGame()
    {
        // Stop the game and show final feedback
        currentState = GameState.Ended;
        feedbackText.text = "Time's up!\nMax Streak: " + maxStreak;
    }

    public void RestartGame()
    {
        // Reset game to idle state and prepare to start again
        currentState = GameState.Idle;
        timer = 60f;
        numCompressions = 0;
        feedbackText.text = "Press Action to start.";
        UpdateUI();
    }

    private void RegisterCompression()
    {
        // Calculate time between compressions and give feedback
        float now = Time.time;
        currentCompressionInterval = now - lastCompressionTime;
        string feedback = "";

        if (currentCompressionInterval < MIN_COMPRESSION_INTERVAL)
        {
            feedback = "Too fast!";
            currentStreak = 0;
        }
        else if (currentCompressionInterval > MAX_COMPRESSION_INTERVAL)
        {
            feedback = "Too slow!";
            currentStreak = 0;
        }
        else
        {
            feedback = "Good!";
            numCompressions++;
            currentStreak++;
            if (currentStreak > 1)
                feedback += " x" + currentStreak;

            if (currentStreak > maxStreak)
                maxStreak = currentStreak;
        }

        lastCompressionTime = now;
        compressionText.text = "Number of chest compressions: " + numCompressions;
        feedbackText.text = feedback;
    }

    private void GoBack()
    {
        // Exit minigame and return to menu
        minigame.SetActive(false);
        miniGameMenu.SetActive(true);
    }

    private void UpdateUI()
    {
        // Update timer and compression text on screen
        timerText.text = "Time Remaining: " + Mathf.CeilToInt(timer);
        compressionText.text = "Number of chest compressions: " + numCompressions;
    }
}
