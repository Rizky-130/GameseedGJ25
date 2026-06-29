using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Stats")]
    public int score = 0;
    public int customerServed= 0;
    public int nyawa = 5;
    [Header("UI")]
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textServed;
    public TextMeshProUGUI textNyawa;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        UpdateUI();
    }

    public void OnCustomerServed()
    {
        score += 100;
        customerServed++;
        UpdateUI();
    }
    public void OnCustomerLeft()
    {
        nyawa--;
        UpdateUI();

        if(nyawa <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        textScore.text = "Score : " + score;
        textServed.text = "CustomerServed : " + customerServed;
        textNyawa.text = "Nyawa : " + nyawa;


    }

    void GameOver()
    {
        Debug.Log("GameOver");

            if (GameOverManager.Instance != null)
            GameOverManager.Instance.ShowGameOver();
    }
}
