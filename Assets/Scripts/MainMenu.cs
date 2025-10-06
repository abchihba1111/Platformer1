using UnityEngine;
using UnityEngine.UI;

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

    [Header("Cameras")]
    public Camera menuCamera;
    public Camera gameCamera;
    public Camera settingsCamera;

    [Header("Game Objects")]
    public GameObject playerObject;
    public GameHUDManager gameHUD; // Ссылка на HUD менеджер

    void Start()
    {
        // Начальное состояние - показываем меню
        ShowMainMenu();
        SetupButtons();
    }

    void SetupButtons()
    {
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettings);
    }

    public void ShowMainMenu()
    {
        Debug.Log("Showing Main Menu");

        // UI Панели
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Камеры
        SetCameraActive(menuCamera, true);
        SetCameraActive(gameCamera, false);
        SetCameraActive(settingsCamera, false);

        // Игровые объекты
        if (playerObject != null) playerObject.SetActive(false);

        // Скрываем игровой HUD
        if (gameHUD != null) gameHUD.SetHUDVisible(false);
    }

    void ShowGame()
    {
        Debug.Log("Showing Game");

        // UI Панели
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Камеры
        SetCameraActive(menuCamera, false);
        SetCameraActive(gameCamera, true);
        SetCameraActive(settingsCamera, false);

        // Игровые объекты
        if (playerObject != null) playerObject.SetActive(true);

        // Показываем игровой HUD и сбрасываем игру
        if (gameHUD != null)
        {
            gameHUD.SetHUDVisible(true);
            gameHUD.ResetGame();
        }
    }

    void ShowSettings()
    {
        Debug.Log("Showing Settings");

        // UI Панели
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        // Камеры
        SetCameraActive(menuCamera, false);
        SetCameraActive(gameCamera, false);
        SetCameraActive(settingsCamera, true);

        // Игровые объекты
        if (playerObject != null) playerObject.SetActive(false);

        // Скрываем игровой HUD
        if (gameHUD != null) gameHUD.SetHUDVisible(false);
    }

    void SetCameraActive(Camera camera, bool active)
    {
        if (camera != null)
        {
            camera.gameObject.SetActive(active);
            camera.enabled = active;
        }
    }

    public void PlayGame()
    {
        Debug.Log("Play Game button clicked");
        ShowGame();
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked");
        ShowSettings();
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