using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    // Ambulance-related objects
    public GameObject ambulance;
    public GameObject spawnPoint;
    public GameObject stoppingPoint;
    [SerializeField] public Quaternion spawnRotation;
    [SerializeField] public AudioClip ambulanceSirenClip;
    [SerializeField] public AudioClip crowdClip;

    // Bystanders and their animation triggers
    public GameObject firstByStander;
    public GameObject secondByStander;
    public string scared = "Scared";
    public string yell = "Yell";

    // Timer values
    private float timer = 0f;
    private float timeTillEMTS = 10f; // This will be randomized in Start()

    // Ambulance tracking
    private bool ambulanceInstantiated = false;
    private GameObject instantiatedAmbulance;
    private float moveSpeed = 0.1f;

    // Bystander activation
    private float timeToActivateB = 10f;
    private bool BsActivated = false;
    private bool bystandersActive = false;

    // UI elements
    public GameObject button;
    public GameObject performancePanel;
    public Slider emtTimerSlider;

    // VR Input
    private List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();

    // Compression script and player input reference
    [SerializeField] ChestCompression chestCompression;
    [SerializeField] PlayerInput playerInput;

    void Start()
    {
        // Hide performance panel at start
        performancePanel.SetActive(false);

        // Hide bystanders initially
        if (firstByStander != null) firstByStander.SetActive(false);
        if (secondByStander != null) secondByStander.SetActive(false);

        // Log VR controllers found
        InputDevices.GetDevices(inputDevices);
        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                Debug.Log("Found controller: " + device.name);
            }
        }

        // Randomize ambulance arrival time and setup UI slider
        timeTillEMTS = Random.Range(30f, 60f);
        emtTimerSlider.maxValue = timeTillEMTS;
        emtTimerSlider.value = 0f;
        emtTimerSlider.gameObject.SetActive(true);
    }

    void Update()
    {
        // Increase timer only if game has started
        if (timeTillEMTS - timer > 0 && chestCompression.GetGameStart())
        {
            timer += Time.deltaTime;
            emtTimerSlider.value = timer;
        }

        // Handle bystander deletion input
        if (bystandersActive)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.B))
            {
                DeleteBystanders();
            }

            CheckOculusControllerInput(); // VR input support
        }

        // Activate bystanders after a set time
        if (timer >= timeToActivateB && !BsActivated)
        {
            ActivateObjectsWithAnimation();
            bystandersActive = true;
        }

        // Spawn ambulance and show performance UI when timer is up
        if (timer >= timeTillEMTS && !ambulanceInstantiated)
        {
            performancePanel.SetActive(true);

            // Instantiate ambulance and play siren
            instantiatedAmbulance = Instantiate(ambulance, spawnPoint.transform.position, spawnRotation);
            AudioSource audioSource = instantiatedAmbulance.GetComponent<AudioSource>();
            if (audioSource == null) audioSource = instantiatedAmbulance.AddComponent<AudioSource>();
            if (ambulanceSirenClip != null)
            {
                audioSource.clip = ambulanceSirenClip;
                audioSource.loop = true;
                audioSource.playOnAwake = true;
                audioSource.Play();
            }
            ambulanceInstantiated = true;

            // Display compression performance stats
            Transform child = performancePanel.transform.Find("Good");
            TMP_Text goodCompressionText = child.GetComponent<TMP_Text>();
            goodCompressionText.text += chestCompression.GetGoodCompressions();

            child = performancePanel.transform.Find("Fast");
            TMP_Text fastCompressionText = child.GetComponent<TMP_Text>();
            fastCompressionText.text += chestCompression.GetFastCompressions();

            child = performancePanel.transform.Find("Slow");
            TMP_Text slowCompressionText = child.GetComponent<TMP_Text>();
            slowCompressionText.text += chestCompression.GetSlowCompressions();

            child = performancePanel.transform.Find("Mouth");
            TMP_Text mouthSuccessText = child.GetComponent<TMP_Text>();
            mouthSuccessText.text += chestCompression.GetMouthSuccesses();

            // End game logic
            chestCompression.SetGameOver();
            emtTimerSlider.gameObject.SetActive(false);
        }

        // Move ambulance to the stopping point smoothly
        if (ambulanceInstantiated && instantiatedAmbulance != null)
        {
            float step = moveSpeed * Time.deltaTime;
            instantiatedAmbulance.transform.position = Vector3.Lerp(instantiatedAmbulance.transform.position, stoppingPoint.transform.position, step);
        }
    }

    // Check for VR button input to remove bystanders
    private void CheckOculusControllerInput()
    {
        if (inputDevices.Count == 0)
        {
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        }

        foreach (UnityEngine.XR.InputDevice device in inputDevices)
        {
            if (device.characteristics.HasFlag(UnityEngine.XR.InputDeviceCharacteristics.Controller))
            {
                bool primaryButtonPressed = false;
                bool secondaryButtonPressed = false;

                device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryButtonPressed);
                device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out secondaryButtonPressed);

                if (primaryButtonPressed || secondaryButtonPressed)
                {
                    DeleteBystanders();
                    return;
                }
            }
        }
    }

    // Enable bystanders and play animations and crowd audio
    private void ActivateObjectsWithAnimation()
    {
        if (firstByStander != null)
        {
            firstByStander.SetActive(true);
            Animator firstAnimator = firstByStander.GetComponent<Animator>();
            if (firstAnimator != null)
            {
                firstAnimator.SetTrigger(yell);
            }
        }

        if (secondByStander != null)
        {
            secondByStander.SetActive(true);
            Animator secondAnimator = secondByStander.GetComponent<Animator>();
            if (secondAnimator != null)
            {
                secondAnimator.SetTrigger(scared);
            }
        }

        if (button != null)
        {
            button.SetActive(true);
        }

        // Play crowd audio from chestCompression's AudioSource
        AudioSource audioSource = chestCompression.gameObject.GetComponent<AudioSource>();
        if (crowdClip != null)
        {
            audioSource.clip = crowdClip;
            audioSource.loop = true;
            audioSource.playOnAwake = true;
            audioSource.Play();
        }

        BsActivated = true;
    }

    // Destroys bystander game objects and stops crowd audio
    public void DeleteBystanders()
    {
        if (firstByStander != null)
        {
            Destroy(firstByStander);
            firstByStander = null;
        }

        if (secondByStander != null)
        {
            Destroy(secondByStander);
            secondByStander = null;
        }

        AudioSource audioSource = chestCompression.gameObject.GetComponent<AudioSource>();
        if (crowdClip != null)
        {
            audioSource.Stop();
        }

        bystandersActive = false;
    }
}
