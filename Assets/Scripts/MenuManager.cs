using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button minigameButton;
    [SerializeField] private Button freePlayButton;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject minigameMenu;
    [SerializeField] private GameObject freePlayMenu;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference selectAction;
    [SerializeField] private float stickDeadzone = 0.5f;
    
    private Button[] menuButtons;
    private int currentButtonIndex = 0;
    private bool inputProcessed = false;

    private void Start()
    {
        freePlayMenu.SetActive(false);
        minigameMenu.SetActive(false);
        
        tutorialButton.onClick.AddListener(LoadTutorialScene);
        minigameButton.onClick.AddListener(openMinigameMenu);
        freePlayButton.onClick.AddListener(OpenScenarioSelect);
        
        menuButtons = new Button[] { tutorialButton, minigameButton, freePlayButton };
        
        if (menuButtons.Length > 0)
            SelectButton(0);
    }
    
    private void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Enable();
            
        if (selectAction != null && selectAction.action != null)
        {
            selectAction.action.Enable();
            selectAction.action.performed += OnSelectPressed;
        }
    }
    
    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();
            
        if (selectAction != null && selectAction.action != null)
        {
            selectAction.action.Disable();
            selectAction.action.performed -= OnSelectPressed;
        }
    }
    
    private void Update()
    {
        if (moveAction != null && moveAction.action != null && menuButtons.Length > 0)
        {
            Vector2 stick = moveAction.action.ReadValue<Vector2>();
            
            if (stick.magnitude > stickDeadzone)
            {
                if (!inputProcessed)
                {
                    if (stick.y > 0.5f)
                        MoveSelection(-1);
                    else if (stick.y < -0.5f)
                        MoveSelection(1);
                        
                    inputProcessed = true;
                }
            }
            else
            {
                inputProcessed = false;
            }
        }
    }
    
    private void MoveSelection(int direction)
    {
        int newIndex = currentButtonIndex + direction;
        if (newIndex < 0)
            newIndex = menuButtons.Length - 1;
        else if (newIndex >= menuButtons.Length)
            newIndex = 0;
            
        SelectButton(newIndex);
    }
    
    private void SelectButton(int index)
    {
        if (currentButtonIndex >= 0 && currentButtonIndex < menuButtons.Length)
        {
            ColorBlock colors = menuButtons[currentButtonIndex].colors;
            colors.normalColor = Color.white;
            menuButtons[currentButtonIndex].colors = colors;
        }
            
        currentButtonIndex = index;
        
        ColorBlock newColors = menuButtons[currentButtonIndex].colors;
        newColors.normalColor = new Color(0.8f, 0.8f, 1f);
        menuButtons[currentButtonIndex].colors = newColors;
    }
    
    private void OnSelectPressed(InputAction.CallbackContext context)
    {
        if (currentButtonIndex >= 0 && currentButtonIndex < menuButtons.Length)
        {
            menuButtons[currentButtonIndex].onClick.Invoke();
        }
    }

    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void openMinigameMenu()
    {
        startMenu.SetActive(false);
        minigameMenu.SetActive(true);
    }

    public void OpenScenarioSelect()
    {
        startMenu.SetActive(false);
        freePlayMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        minigameMenu.SetActive(false);
        freePlayMenu.SetActive(false);
        startMenu.SetActive(true);
        
        if (menuButtons.Length > 0)
            SelectButton(0);
    }

    private void OnDestroy()
    {
        tutorialButton.onClick.RemoveListener(LoadTutorialScene);
        minigameButton.onClick.RemoveListener(openMinigameMenu);
        freePlayButton.onClick.RemoveListener(OpenScenarioSelect);
        
        if (selectAction != null && selectAction.action != null)
            selectAction.action.performed -= OnSelectPressed;
    }
}