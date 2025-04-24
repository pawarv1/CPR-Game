using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public GameObject ambulance;
    public GameObject spawnPoint;
    public GameObject stoppingPoint;
    public Quaternion spawnRotation;
    public GameObject firstByStander;
    public GameObject secondByStander;
    private float timer = 0f;
    private float timeTillEMTS = 30f; 
    private bool ambulanceInstantiated = false;
    private GameObject instantiatedAmbulance;
    private float moveSpeed = 0.1f; // Speed at which the ambulance will move (between 0 and 1)
     private float timeToActivateB = 10f;
    private bool BsActivated = false;
    public string scared = "Scared";
    public string yell = "Yell";
    public GameObject button;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Time remaining: " + (timeTillEMTS - timer));
        spawnRotation = Quaternion.Euler(0f, 90f, 0f);
        if (firstByStander != null) firstByStander.SetActive(false);
        if (secondByStander != null) secondByStander.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log("Time remaining: " + (timeTillEMTS - timer));
        if (timer >= timeToActivateB && !BsActivated)
        {
            ActivateObjectsWithAnimation();
        }
        if (timer >= timeTillEMTS && !ambulanceInstantiated)
        {
            instantiatedAmbulance = Instantiate(ambulance, spawnPoint.transform.position, spawnRotation);
            Debug.Log("Paramedics have arrived!!");
            ambulanceInstantiated = true;
        }

        if (ambulanceInstantiated && instantiatedAmbulance != null)
        {
            float step = moveSpeed * Time.deltaTime;
            instantiatedAmbulance.transform.position = Vector3.Lerp(instantiatedAmbulance.transform.position, stoppingPoint.transform.position, step);
            if (Vector3.Distance(instantiatedAmbulance.transform.position, stoppingPoint.transform.position) < 0.1f)
            {
                Debug.Log("Ambulance reached the destination!");
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
    }
        
}


