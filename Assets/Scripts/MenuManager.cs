using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button freePlayButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject freePlayMenu;


    private void Start()
    {
        tutorialButton.onClick.AddListener(LoadTutorialScene);
        freePlayButton.onClick.AddListener(OpenScenarioSelect);
        exitButton.onClick.AddListener(ExitGame);
    }
    private void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void OpenScenarioSelect()
    {
        startMenu.SetActive(false);
        freePlayMenu.SetActive(true);
    }

    private void ExitGame()
    {
        // UnityEditor.EditorApplication.isPlaying = false;
    }

    private void OnDestroy()
    {
        tutorialButton.onClick.RemoveListener(LoadTutorialScene);
    }
}