using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MiniGameScript : MonoBehaviour
{
    [SerializeField] private GameObject minigame;
    [SerializeField] private GameObject miniGameMenu;
    //public Button chestCompression;
    public Button goBackButton;
    public Button restartButton;
    public TMP_Text timerText;
    public TMP_Text compressionText;
    private int numCompressions;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        numCompressions = 0;
        timer = 60f;
        timerText.text = "Time Remaining: " + timer;
        compressionText.text = "Number of chest compressions: " + numCompressions;
        //chestCompression.onClick.AddListener(updateCounter);
        restartButton.onClick.AddListener(restartTimer);
        goBackButton.onClick.AddListener(goBack);
    }

    private void updateCounter()
    {
        numCompressions++;
        compressionText.text = "Number of chest compressions: " + numCompressions;
    }

    private void restartTimer()
    {
        timer = 60f;
    }

    private void goBack()
    {
        minigame.SetActive(false);
        miniGameMenu.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            updateCounter();
        }

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }

        timerText.text = "Time Remaining: " + timer;
    }
}