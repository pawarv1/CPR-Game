using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button minigameButton;
    [SerializeField] private Button freePlayButton;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject minigameMenu;
    [SerializeField] private GameObject freePlayMenu;


    private void Start()
    {
        freePlayMenu.SetActive(false);
        minigameMenu.SetActive(false);
        tutorialButton.onClick.AddListener(LoadTutorialScene);
        minigameButton.onClick.AddListener(openMinigameMenu);
        freePlayButton.onClick.AddListener(OpenScenarioSelect);
    }

    private void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void openMinigameMenu()
    {
        startMenu.SetActive(false);
        minigameMenu.SetActive(true);
    }

    private void OpenScenarioSelect()
    {
        startMenu.SetActive(false);
        freePlayMenu.SetActive(true);
    }

    private void OnDestroy()
    {
        tutorialButton.onClick.RemoveListener(LoadTutorialScene);
    }
}