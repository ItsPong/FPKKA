using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public InputField nameInputField;  
    public Button startButton;         
    public Button exitButton;          
    public Text errorMessage;          
    public Text TotalKills;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        errorMessage.gameObject.SetActive(false); 

        if (PlayerPrefs.HasKey("KillCount")) {
            TotalKills.gameObject.SetActive(true);
            TotalKills.text = "Total Kills: " + PlayerPrefs.GetInt("KillCount");
        } else {
            TotalKills.gameObject.SetActive(false);
        }
    }

    void OnStartButtonClicked()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            errorMessage.gameObject.SetActive(true); 
            errorMessage.text = "Nama hero harus diisi!";
        }
        else
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            SceneManager.LoadScene("Game");
        }
    }

    void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
