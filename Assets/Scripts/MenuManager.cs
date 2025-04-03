using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button tutorialButton;

    private void Start()
    {
        tutorialButton.onClick.AddListener(LoadTutorialScene);
    }
    private void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void OnDestroy()
    {
        tutorialButton.onClick.RemoveListener(LoadTutorialScene);
    }
}