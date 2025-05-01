using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

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
    private float timeTillEMTS = 30f; 
    private bool ambulanceInstantiated = false;
    private GameObject instantiatedAmbulance;
    private float moveSpeed = 0.1f;
    private float timeToActivateB = 10f;
    private bool BsActivated = false;
    public string scared = "Scared";
    public string yell = "Yell";
    public GameObject button;
    
    private List<InputDevice> inputDevices = new List<InputDevice>();
    private bool bystandersActive = false;

    void Start()
    {
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
    }

    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log("Time remaining: " + (timeTillEMTS - timer));
        
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
            InputDevices.GetDevices(inputDevices);
        }
        
        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                bool primaryButtonPressed = false;
                bool secondaryButtonPressed = false;
                
                device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);
                device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed);
                
                if (primaryButtonPressed)
                {
                    DeleteBystanders();
                    return;
                }
                
                if (secondaryButtonPressed)
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


