using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ChestCompression : MonoBehaviour
{
    // References to VR player components and UI
    [SerializeField] Transform headset;
    [SerializeField] Transform controller;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] TextMeshProUGUI compressionText;
    [SerializeField] TextMeshProUGUI feedbackText;
    [SerializeField] TextMeshProUGUI mouthToMouthText;

    // Thresholds for detecting compression and reset movement
    [SerializeField] float compressionThreshold = 0.01f;
    [SerializeField] float resetThreshold = 0.02f;

    private float lastY;
    private bool readyForNextCompression = true;
    private int compressionCount = 0;
    private int tempCompressionCount = 0;
    private float lastControllerY;

    // Ideal compression rate range (CPM)
    [SerializeField] float minCompressionInterval = 0.5f;  // 100 cpm
    [SerializeField] float maxCompressionInterval = 0.6f;  // 120 cpm

    private float lastCompressionTime = -1f;

    private bool waitingForMouthToMouth = false;

    [SerializeField] XRController xrController; // For haptic feedback

    // Compression stats
    private int goodCompressions = 0;
    private int fastCompressions = 0;
    private int slowCompressions = 0;
    private int mouthToMouthSuccesses = 0;
    private int mouthToMouthFails = 0;

    private bool sessionEnded = false;

    [SerializeField] bool gameStart = false;
    [SerializeField] GameObject tutorialMenu;
    private bool gameOver = false;

    void Start()
    {
        // Store initial headset height
        if (headset != null)
            lastY = headset.localPosition.y;

        mouthToMouthText.text = "";
    }

    void Update()
    {
        // Dismiss tutorial menu and start game
        if (tutorialMenu != null && playerInput.actions["Action"].WasPressedThisFrame()) {
            tutorialMenu.SetActive(false);
            gameStart = true;
        }

        // Return to menu if game is over and action is pressed
        if (gameOver && playerInput.actions["Action"].WasPressedThisFrame()) SceneManager.LoadScene("MenuScreen");

        // Don't proceed if the game hasn't started
        if (!gameStart) return;

        // Update compression count text
        compressionText.text = "Compressions: " + compressionCount.ToString();

        float currentControllerY = controller.position.y;
        float deltaY = lastControllerY - currentControllerY;
        float temp = currentControllerY - lastControllerY;

        // Check for mouth-to-mouth interaction
        if (waitingForMouthToMouth && playerInput.actions["Action"].WasPressedThisFrame())
        {
            StartCoroutine(MouthToMouthSuccess());
        }

        // Check for downward compression movement
        if (deltaY > compressionThreshold && readyForNextCompression)
        {
            RegisterCompression();
            readyForNextCompression = false;
        }

        // Allow new compression after controller moves up past threshold
        if (currentControllerY - lastControllerY > resetThreshold)
        {
            readyForNextCompression = true;
        }

        lastControllerY = currentControllerY;
    }

    // Handles compression count and feedback logic
    void RegisterCompression()
    {
        compressionCount++;
        tempCompressionCount++;
        float currentTime = Time.time;

        // Check interval timing between compressions
        if (lastCompressionTime > 0f)
        {
            float interval = currentTime - lastCompressionTime;
            if (interval >= minCompressionInterval && interval <= maxCompressionInterval)
            {
                feedbackText.text = "Good!";
                feedbackText.color = Color.green;
                TriggerHaptic(0.5f, 0.1f);
                goodCompressions++;
            }
            else if (interval < minCompressionInterval)
            {
                feedbackText.text = "Too Fast!";
                feedbackText.color = Color.yellow;
                TriggerHaptic(0.1f, 0.1f);
                fastCompressions++;
            }
            else
            {
                feedbackText.text = "Too Slow!";
                feedbackText.color = Color.red;
                TriggerHaptic(0.1f, 0.1f);
                slowCompressions++;
            }
        }
        else
        {
            feedbackText.text = "Start!";
            feedbackText.color = Color.white;
        }

        lastCompressionTime = currentTime;

        // Trigger mouth-to-mouth prompt after 30 compressions
        if (tempCompressionCount >= 30) {
            TriggerMouthToMouth();
        }

        // If no mouth-to-mouth after 35 compressions, fail
        if (tempCompressionCount >= 35) {
            TriggerFailMouth();
        }

        Debug.Log($"Compression #{compressionCount}");
    }

    // Prompts the user for mouth-to-mouth
    void TriggerMouthToMouth()
    {
        mouthToMouthText.text = "Give Mouth-to-Mouth! Press [Submit]";
        mouthToMouthText.color = Color.cyan;
        waitingForMouthToMouth = true;
    }

    // Handles failure to perform mouth-to-mouth
    void TriggerFailMouth() {
        mouthToMouthText.text = "Failed to give Mouth to Mouth";
        mouthToMouthText.color = Color.red;
        waitingForMouthToMouth = false;
        tempCompressionCount = 0;
        mouthToMouthFails++;
        Invoke("ResetMouthText", 3.0f);
    }

    // Clears mouth-to-mouth instruction text
    void ResetMouthText() => mouthToMouthText.text = "";

    // Coroutine for successful mouth-to-mouth
    IEnumerator MouthToMouthSuccess()
    {
        tempCompressionCount = 0;
        mouthToMouthSuccesses++;
        TriggerHaptic(1f, 0.2f);
        mouthToMouthText.text = "Success!";
        mouthToMouthText.color = Color.green;
        waitingForMouthToMouth = false;

        yield return new WaitForSeconds(3f);

        mouthToMouthText.text = "";
    }

    // Sends haptic feedback to controller
    void TriggerHaptic(float amplitude, float duration)
    {
        if (xrController != null)
        {
            xrController.SendHapticImpulse(amplitude, duration);
        }
        
    }

    // Returns if the game has started
    public bool GetGameStart() => gameStart;

    // Ends the game session
    public void SetGameOver() {
        gameStart = false;
        gameOver = true;
    }

    // Stat getters
    public int GetGoodCompressions() => goodCompressions;
    public int GetFastCompressions() => fastCompressions;
    public int GetSlowCompressions() => slowCompressions;
    public int GetMouthSuccesses() => mouthToMouthSuccesses;

}
