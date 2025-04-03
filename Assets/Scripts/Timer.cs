using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameObject ambulance;
    private float timer = 0f;
    private float timeTillEMTS;
    
    // Start is called before the first frame update
    void Start()
    {
        float timer = 0f;
        timeTillEMTS = 60f;
        Debug.Log(timeTillEMTS - timer);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(timeTillEMTS - timer);

        if (timer >= timeTillEMTS)
        {
            Instantiate(ambulance, Vector3.zero, Quaternion.identity);
            Debug.Log("Paramedics have arrived!!");
            Application.Quit();
        }
    }
}
