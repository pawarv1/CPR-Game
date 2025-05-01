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
    public GameObject ambulance;
    public GameObject spawnPoint;
    public GameObject stoppingPoint;
    [SerializeField] public Quaternion spawnRotation;
    [SerializeField] public AudioClip ambulanceSirenClip;
    public GameObject firstByStander;
    public GameObject secondByStander;
    private float timer = 0f;
    private float timeTillEMTS = 30f; //Random.Range(30f, 60f);
    private bool ambulanceInstantiated = false;
    private GameObject instantiatedAmbulance;
    private float moveSpeed = 0.1f;
    private float timeToActivateB = 10f;
    private bool BsActivated = false;
    public string scared = "Scared";
    public string yell = "Yell";
    public GameObject button;
    public GameObject performancePanel;
    private List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();

    private bool bystandersActive = false;
    public Slider emtTimerSlider;
    [SerializeField] ChestCompression chestCompression;
    [SerializeField] PlayerInput playerInput;

    void Start()
    {
        performancePanel.SetActive(false);
        Debug.Log("Time remaining: " + (timeTillEMTS - timer));
        if (firstByStander != null) firstByStander.SetActive(false);
        if (secondByStander != null) secondByStander.SetActive(false);
        
        InputDevices.GetDevices(inputDevices);
        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                Debug.Log("Found controller: " + device.name);
            }
        }

        emtTimerSlider.maxValue = timeTillEMTS;
        emtTimerSlider.value = 0f;
    }

    void Update()
    {
        
        if (timeTillEMTS - timer > 0 && chestCompression.GetGameStart())
        {
            timer += Time.deltaTime;
            emtTimerSlider.value = timer;
            Debug.Log("Time remaining: " + (timeTillEMTS - timer));
        }
        
        if (bystandersActive)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                DeleteBystanders();
            }
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                DeleteBystanders();
            }
            
            CheckOculusControllerInput();
        }
        
        if (timer >= timeToActivateB && !BsActivated)
        {
            ActivateObjectsWithAnimation();
            bystandersActive = true;
        }
        
        if (timer >= timeTillEMTS && !ambulanceInstantiated)
        {

            instantiatedAmbulance = Instantiate(ambulance, spawnPoint.transform.position, spawnRotation);
            AudioSource audioSource = instantiatedAmbulance.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = instantiatedAmbulance.AddComponent<AudioSource>();
            }
            if (ambulanceSirenClip != null)
            {
                audioSource.clip = ambulanceSirenClip;
                audioSource.loop = true;
                audioSource.playOnAwake = true;
                audioSource.Play();
            }
            Debug.Log("Paramedics have arrived!!");
            ambulanceInstantiated = true;

            Transform child = performancePanel.transform.Find("Good");
            TMP_Text goodCompressionText = child.GetComponent<TMP_Text>();
            goodCompressionText.text += ChestCompression.goodCompressions;

            child = performancePanel.transform.Find("Fast");
            TMP_Text fastCompressionText = child.GetComponent<TMP_Text>();
            fastCompressionText.text += ChestCompression.fastCompressions;

            child = performancePanel.transform.Find("Slow");
            TMP_Text slowCompressionText = child.GetComponent<TMP_Text>();
            slowCompressionText.text += ChestCompression.slowCompressions;

            child = performancePanel.transform.Find("Mouth");
            TMP_Text mouthSuccessText = child.GetComponent<TMP_Text>();
            mouthSuccessText.text += ChestCompression.mouthToMouthSuccesses;

            performancePanel.SetActive(true);
            chestCompression.SetGameStart();
            if (playerInput.actions["Action"].WasPressedThisFrame()) {
                SceneManager.LoadScene("MenuScreen");
            }
        }

        if (ambulanceInstantiated && instantiatedAmbulance != null)
        {
            float step = moveSpeed * Time.deltaTime;
            instantiatedAmbulance.transform.position = Vector3.Lerp(instantiatedAmbulance.transform.position, stoppingPoint.transform.position, step);
        }
    }
    
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
        
        BsActivated = true;
    }

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
        
        bystandersActive = false;
    }
}


