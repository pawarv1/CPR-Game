using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ChestCompression : MonoBehaviour
{
    public Transform headset;               // Assign VR camera (CenterEyeAnchor)
     public Transform controller; 
    public Collider chestCollider;
    public PlayerInput playerInput;
    public TextMeshProUGUI compressionText;
    public TextMeshProUGUI debugText;
    public TextMeshProUGUI debugText2;

    public float leanInOffset = 0.3f;       // How far to move down
    public KeyCode leanInKey = KeyCode.Space; // Replace with button mapping if needed
    private bool hasLeanedIn = false;

    public float compressionThreshold = 0.01f;
    public float resetThreshold = 0.02f;

    private float lastY;
    private bool readyForNextCompression = true;
    private int compressionCount = 0;
    private float lastControllerY;

    public TextMeshProUGUI feedbackText;

    public float minCompressionInterval = 0.5f;  // 100 cpm
    public float maxCompressionInterval = 0.6f;  // 120 cpm

    private float lastCompressionTime = -1f;

    void Start()
    {
        if (headset != null)
            lastY = headset.localPosition.y;
    }

    void Update()
    {

        Debug.Log("Controller Y: " + controller.position.y);
        debugText.text = "Controller Y: " + controller.position.y.ToString("F2");

        compressionText.text = compressionCount.ToString();

        // Lean-in check
        if (!hasLeanedIn &&
            (
             playerInput.actions["Action"].WasPressedThisFrame()))
        {
            LeanIn();
        }

        if (!hasLeanedIn) return;

        float currentControllerY = controller.position.y;
        float deltaY = lastControllerY - currentControllerY;

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

        Debug.Log("Controller Y: " + controller.position.y + " Delt " + deltaY);
        // debugText.text = "Controller Y: " + controller.position.y;//.ToString("F2");
        debugText2.text = "Delt " + deltaY.ToString("F2");
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
        float currentTime = Time.time;

        if (lastCompressionTime > 0f)
        {
            float interval = currentTime - lastCompressionTime;
            if (interval >= minCompressionInterval && interval <= maxCompressionInterval)
            {
                feedbackText.text = "Good!";
                feedbackText.color = Color.green;
            }
            else if (interval < minCompressionInterval)
            {
                feedbackText.text = "Too Fast!";
                feedbackText.color = Color.yellow;
            }
            else
            {
                feedbackText.text = "Too Slow!";
                feedbackText.color = Color.red;
            }
        }
        else
        {
            feedbackText.text = "Start!";
            feedbackText.color = Color.white;
        }

        lastCompressionTime = currentTime;

        Debug.Log($"Compression #{compressionCount}");
    }

    bool IsOverChest(Vector3 position)
    {
        return chestCollider.bounds.Contains(position);
    }
}
