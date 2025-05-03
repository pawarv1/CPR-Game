using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MiniGameScript : MonoBehaviour
{
    [SerializeField] private GameObject minigame;
    [SerializeField] private GameObject miniGameMenu;

    [SerializeField] private PlayerInput playerInput;
    public Button goBackButton;
    public Button restartButton;
    public TMP_Text timerText;
    public TMP_Text compressionText;
    public TMP_Text feedbackText;

    private int currentStreak = 0;
    private int maxStreak = 0;
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private TMP_Text maxStreakText;

    private int numCompressions;
    private float timer;
    private float lastCompressionTime;
    private float currentCompressionInterval;

    private const float MIN_COMPRESSION_INTERVAL = 0.5f; // Too fast if < 0.5s
    private const float MAX_COMPRESSION_INTERVAL = 1.5f; // Too slow if > 1.5s

    private enum GameState { Idle, Running, Ended }
    private GameState currentState = GameState.Idle;

    void Start()
    {
        numCompressions = 0;
        timer = 60f;
        feedbackText.text = "";
        UpdateUI();

        restartButton.onClick.AddListener(RestartGame);
        goBackButton.onClick.AddListener(GoBack);
    }

    private void Update()
    {
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

        if (currentState == GameState.Running)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 0f;
                EndGame();
            }
        }

        if (playerInput.actions["Back"].WasPressedThisFrame())
        {
            GoBack();
        }

        if (playerInput.actions["Select"].WasPressedThisFrame())
        {
            RestartGame();
        }

        UpdateUI();
    }

    private void StartGame()
    {
        currentState = GameState.Running;
        timer = 60f;
        numCompressions = 0;
        lastCompressionTime = Time.time;
        feedbackText.text = "Game started!";
    }

    private void EndGame()
    {
        currentState = GameState.Ended;
        feedbackText.text = "Time's up!\nMax Streak: " + maxStreak;
        // maxStreakText.text = "Max Streak: " + maxStreak;
    }

    public void RestartGame()
    {
        currentState = GameState.Idle;
        timer = 60f;
        numCompressions = 0;
        feedbackText.text = "Press Action to start.";
        UpdateUI();
    }

    private void RegisterCompression()
    {
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

        // streakText.text = "Streak: " + currentStreak;
    }

    private void GoBack()
    {
        minigame.SetActive(false);
        miniGameMenu.SetActive(true);
    }

    private void UpdateUI()
    {
        timerText.text = "Time Remaining: " + Mathf.CeilToInt(timer);
        compressionText.text = "Number of chest compressions: " + numCompressions;
    }
}
