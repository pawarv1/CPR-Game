using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenarioSelection : MonoBehaviour
{
    [SerializeField] private Button selectHeartAttack;
    [SerializeField] private Button selectDrowning;
    [SerializeField] private Button selectChoking;
    [SerializeField] private Button selectSmoke;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject freePlayMenu;

    // Start is called before the first frame update
    void Start()
    {
        selectHeartAttack.onClick.AddListener(LoadHeartAttack);
        selectDrowning.onClick.AddListener(LoadDrowning);
        selectChoking.onClick.AddListener(LoadChoking);
        selectSmoke.onClick.AddListener(LoadSmoke);
    }

    private void LoadHeartAttack()
    {
        SceneManager.LoadScene("HeartAttack");
    }

    private void LoadDrowning()
    {
        SceneManager.LoadScene("Drowning");
    }

    private void LoadChoking()
    {
        SceneManager.LoadScene("Choking");
    }

    private void LoadSmoke()
    {
        SceneManager.LoadScene("Smoke");
    }
}
