using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameObject ambulance;
    public GameObject spawnPoint;
    public GameObject stoppingPoint;
    public Quaternion spawnRotation;
    private float timer = 0f;
    private float timeTillEMTS = 10f; 
    private bool ambulanceInstantiated = false;
    private GameObject instantiatedAmbulance;
    private float moveSpeed = 0.1f; // Speed at which the ambulance will move (between 0 and 1)

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Time remaining: " + (timeTillEMTS - timer));
        spawnRotation = Quaternion.Euler(0f, 90f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log("Time remaining: " + (timeTillEMTS - timer));

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
}


