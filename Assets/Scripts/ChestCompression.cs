using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ChestCompression : MonoBehaviour
{
    public Transform headset;               // Assign VR camera (CenterEyeAnchor)
    public Transform controller; 
    public Collider chestCollider;
    public PlayerInput playerInput;
    public TextMeshProUGUI compressionText;
    public TextMeshProUGUI debugText;
    // public TextMeshProUGUI debugText2;

    public float leanInOffset = 0.3f;       // How far to move down
    public KeyCode leanInKey = KeyCode.Space; // Replace with button mapping if needed
    private bool hasLeanedIn = false;

    public float compressionThreshold = 0.01f;
    public float resetThreshold = 0.02f;

    private float lastY;
    private bool readyForNextCompression = true;
    private int compressionCount = 0;
    private int tempCompressionCount = 0; // Seperate compression count to help with mouth functionality
    private float lastControllerY;

    public TextMeshProUGUI feedbackText;

    public float minCompressionInterval = 0.5f;  // 100 cpm
    public float maxCompressionInterval = 0.6f;  // 120 cpm

    private float lastCompressionTime = -1f;

    public TextMeshProUGUI mouthToMouthText;
    // public string mouthToMouthKey = "Submit"; // Assign to a button in Input Actions

    private bool waitingForMouthToMouth = false;
    public XRBaseController xrController; 
    public static int goodCompressions = 0;
    public static int fastCompressions = 0;
    public static int slowCompressions = 0;
    public static int mouthToMouthSuccesses = 0;

    public TextMeshProUGUI summaryText; // Link in Inspector
    // public float sessionDuration = 60f; // Total session time in seconds
    // private float sessionStartTime;
    private bool sessionEnded = false;

    // public TextMeshProUGUI goodText;
    // public TextMeshProUGUI fastText;
    // public TextMeshProUGUI slowText;
    // public TextMeshProUGUI mouthText;
    // public TextMeshProUGUI percentText;

    [SerializeField] bool gameStart = false;
    [SerializeField] GameObject tutorialMenu;




    void Start()
    {
        if (headset != null)
            lastY = headset.localPosition.y;
        mouthToMouthText.text = "";

        

        
    }

    void Update()
    {
        if (tutorialMenu != null && playerInput.actions["Action"].WasPressedThisFrame()) {
            tutorialMenu.SetActive(false);
            gameStart = true;
        }

        if (!gameStart) return;


        compressionText.text = "Compressions: " + compressionCount.ToString();

        // Lean-in check
        // if (!hasLeanedIn &&
        //     (
        //      playerInput.actions["Action"].WasPressedThisFrame()))
        // {
        //     LeanIn();
        // }

        // if (!hasLeanedIn) return;

        float currentControllerY = controller.position.y;
        float deltaY = lastControllerY - currentControllerY;
        float temp = currentControllerY - lastControllerY;

        // debugText.text = lastControllerY + "\n" + currentControllerY + "\n" + temp; 

        if (waitingForMouthToMouth && playerInput.actions["Action"].WasPressedThisFrame())
        {
            StartCoroutine(MouthToMouthSuccess());
        }

        // Optional: Uncomment if you want to check if the controller is over the chest
        // if (chestCollider.bounds.Contains(controller.position))
        // {
        if (deltaY > compressionThreshold && readyForNextCompression)
        {
            RegisterCompression();
            readyForNextCompression = false;
        }

        if (currentControllerY - lastControllerY > resetThreshold)
        {
            readyForNextCompression = true;
        }
        // }

        

        lastControllerY = currentControllerY;

       


     
    }

    void LeanIn()
    {
        headset.localPosition -= new Vector3(0, leanInOffset, 0);
        hasLeanedIn = true;
        Debug.Log("Leaning in toward patient.");
    }

    // void RegisterCompression()
    // {
    //     compressionCount++;
    //     Debug.Log($"Compression #{compressionCount}");
    //     // Add feedback here (animation, sound, etc.)
    // }
    void RegisterCompression()
    {
        compressionCount++;
        tempCompressionCount++;
        float currentTime = Time.time;

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

        if (tempCompressionCount >= 30) {
            TriggerMouthToMouth();
        }

        Debug.Log($"Compression #{compressionCount}");
    }

    bool IsOverChest(Vector3 position)
    {
        return chestCollider.bounds.Contains(position);
    }

    void TriggerMouthToMouth()
    {
        mouthToMouthText.text = "Give Mouth-to-Mouth! Press [Submit]";
        mouthToMouthText.color = Color.cyan;
        waitingForMouthToMouth = true;
    }

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

    void TriggerHaptic(float amplitude, float duration)
    {
        if (xrController != null)
        {
            xrController.SendHapticImpulse(amplitude, duration);
        }
    }

    public bool GetGameStart() => gameStart;
    public void SetGameStart() => gameStart = false;

    // void ShowScore() {
    //     goodText.text = "Good: " + goodCompressions;
    //     fastText.text = "Fast: " + fastCompressions;
    //     slowText.text = "Slow: " + slowCompressions;
    //     mouthText.text = "Mouth Successes: " + mouthToMouthSuccesses;

    //     // float score = 
    // }

}
