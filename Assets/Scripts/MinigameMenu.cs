using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button goBackButton;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject minigameMenu;
    [SerializeField] private GameObject minigame;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(openMiniGame);
        goBackButton.onClick.AddListener(goBack);
    }

    void openMiniGame()
    {
        minigameMenu.SetActive(false);
        minigame.SetActive(true);
    }

    void goBack()
    {
        minigameMenu.SetActive(false);
        startMenu.SetActive(true);
    }
}
