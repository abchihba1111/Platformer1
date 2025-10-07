using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    public Button closeSettingsButton;

    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";

    void Start()
    {
        // Автоматически находим UI элементы если не назначены
        FindUIElementsAutomatically();

        // Начальное состояние - показываем меню
        ShowMainMenu();
        SetupButtons();
    }

    void FindUIElementsAutomatically()
    {
        // Находим панели
        if (mainMenuPanel == null)
            mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (settingsPanel == null)
            settingsPanel = GameObject.Find("SettingsPanel");

        // Находим кнопки
        if (playButton == null)
        {
            GameObject playObj = GameObject.Find("PlayButton");
            if (playObj != null) playButton = playObj.GetComponent<Button>();
        }
        if (settingsButton == null)
        {
            GameObject settingsObj = GameObject.Find("SettingsButton");
            if (settingsObj != null) settingsButton = settingsObj.GetComponent<Button>();
        }
        if (exitButton == null)
        {
            GameObject exitObj = GameObject.Find("ExitButton");
            if (exitObj != null) exitButton = exitObj.GetComponent<Button>();
        }
        if (closeSettingsButton == null)
        {
            GameObject closeObj = GameObject.Find("CloseButton");
            if (closeObj != null) closeSettingsButton = closeObj.GetComponent<Button>();
        }

        Debug.Log("MainMenu UI elements auto-found");
    }

    void SetupButtons()
    {
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettings);
    }

    void ShowMainMenu()
    {
        Debug.Log("Showing Main Menu");

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        Debug.Log("Play Game button clicked - Loading: " + gameSceneName);

        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("Game scene name is not set!");
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked");

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        Debug.Log("Close Settings button clicked");
        ShowMainMenu();
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}