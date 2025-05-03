using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ChestCompression : MonoBehaviour
{
    [SerializeField] Transform headset;           
    [SerializeField] Transform controller; 
    [SerializeField] PlayerInput playerInput;
    [SerializeField] TextMeshProUGUI compressionText;
    [SerializeField] TextMeshProUGUI feedbackText;
    [SerializeField] TextMeshProUGUI mouthToMouthText;

    [SerializeField] float compressionThreshold = 0.01f;
    [SerializeField] float resetThreshold = 0.02f;

    private float lastY;
    private bool readyForNextCompression = true;
    private int compressionCount = 0;
    private int tempCompressionCount = 0;
    private float lastControllerY;

    [SerializeField]  float minCompressionInterval = 0.5f;  // 100 cpm
    [SerializeField]  float maxCompressionInterval = 0.6f;  // 120 cpm

    private float lastCompressionTime = -1f;


    private bool waitingForMouthToMouth = false;
    [SerializeField] XRBaseController xrController; 
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



        if (gameOver && playerInput.actions["Action"].WasPressedThisFrame()) SceneManager.LoadScene("MenuScreen");

        if (!gameStart) return;


        compressionText.text = "Compressions: " + compressionCount.ToString();


        float currentControllerY = controller.position.y;
        float deltaY = lastControllerY - currentControllerY;
        float temp = currentControllerY - lastControllerY;


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
       

        lastControllerY = currentControllerY;

       


     
    }

 
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

        if (tempCompressionCount >= 35) {
            TriggerFailMouth();
        }

        Debug.Log($"Compression #{compressionCount}");
    }


    void TriggerMouthToMouth()
    {
        mouthToMouthText.text = "Give Mouth-to-Mouth! Press [Submit]";
        mouthToMouthText.color = Color.cyan;
        waitingForMouthToMouth = true;
    }

    void TriggerFailMouth() {
        mouthToMouthText.text = "Failed to give Mouth to Mouth";
        mouthToMouthText.color = Color.red;
        waitingForMouthToMouth = false;
        tempCompressionCount = 0;
        mouthToMouthFails++;
        Invoke("ResetMouthText", 3.0f);
    }

    void ResetMouthText() => mouthToMouthText.text = "";

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
    public void SetGameOver() {
        gameStart = false;
        gameOver = true;
    }

    public int GetGoodCompressions() => goodCompressions;
    public int GetFastCompressions() => fastCompressions;
    public int GetSlowCompressions() => slowCompressions;
    public int GetMouthSuccesses() => mouthToMouthSuccesses;

}
