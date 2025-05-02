// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class ScenarioSelection : MonoBehaviour
// {
//     [SerializeField] private Button selectHeartAttack;
//     [SerializeField] private Button selectDrowning;
//     [SerializeField] private Button selectChoking;
//     [SerializeField] private Button selectSmoke;
//     [SerializeField] private GameObject startMenu;
//     [SerializeField] private GameObject freePlayMenu;

//     // Start is called before the first frame update
//     void Start()
//     {
//         selectHeartAttack.onClick.AddListener(LoadHeartAttack);
//         selectDrowning.onClick.AddListener(LoadDrowning);
//         selectChoking.onClick.AddListener(LoadChoking);
//         selectSmoke.onClick.AddListener(LoadSmoke);
//     }

//     private void LoadHeartAttack()
//     {
//         SceneManager.LoadScene("HeartAttack");
//     }

//     private void LoadDrowning()
//     {
//         SceneManager.LoadScene("Drowning");
//     }

//     private void LoadChoking()
//     {
//         SceneManager.LoadScene("Choking");
//     }

//     private void LoadSmoke()
//     {
//         SceneManager.LoadScene("Smoke");
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ScenarioSelection : MonoBehaviour
{
    [SerializeField] private Button selectHeartAttack;
    [SerializeField] private Button selectDrowning;
    [SerializeField] private Button selectChoking;
    [SerializeField] private Button selectSmoke;
    [SerializeField] private Button randomButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject freePlayMenu;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference selectAction;
    [SerializeField] private float stickDeadzone = 0.5f;
    
    private Button[] menuButtons;
    private int currentButtonIndex = 0;
    private bool inputProcessed = false;
    
    void Start()
    {
        selectHeartAttack.onClick.AddListener(LoadHeartAttack);
        selectDrowning.onClick.AddListener(LoadDrowning);
        selectChoking.onClick.AddListener(LoadChoking);
        selectSmoke.onClick.AddListener(LoadSmoke);
        randomButton.onClick.AddListener(LoadRandomScenario);
        backButton.onClick.AddListener(GoBackToStartMenu);
        
        menuButtons = new Button[] { selectHeartAttack, selectDrowning, selectChoking, selectSmoke, randomButton };
        
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
    
    public void LoadHeartAttack()
    {
        SceneManager.LoadScene("HeartAttack");
    }

    public void LoadDrowning()
    {
        SceneManager.LoadScene("Drowning");
    }

    public void LoadChoking()
    {
        SceneManager.LoadScene("Choking");
    }

    public void LoadSmoke()
    {
        SceneManager.LoadScene("Smoke");
    }
    public void LoadRandomScenario()
    {
        string[] scenarios = new string[] { "HeartAttack", "Drowning", "Choking", "Smoke" };
        int randomIndex = Random.Range(0, scenarios.Length);
        SceneManager.LoadScene(scenarios[randomIndex]);
    }
    public void GoBackToStartMenu()
{
    // Deactivate the current menu (FreePlayMenu)
    if (freePlayMenu != null)
        freePlayMenu.SetActive(false);
    
    // Activate the starting menu
    if (startMenu != null)
        startMenu.SetActive(true);
}
}