using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameHUDManager : MonoBehaviour
{
    [Header("Health Settings")]
    public TextMeshProUGUI[] hearts;
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Coin Settings")]
    public TextMeshProUGUI coinCounter;
    public int totalCoins = 10;
    public int collectedCoins = 0;

    [Header("UI Panels")]
    public GameObject healthPanel;
    public GameObject coinsPanel;

    [Header("Scene Settings")]
    public string mainMenuScene = "MainMenu";

    [Header("Character Switching")]
    public GameObject[] players; // Массив персонажей
    public Camera[] playerCameras; // Массив камер для каждого персонажа
    public int currentPlayerIndex = 0;
    public KeyCode switchKey = KeyCode.P;

    void Start()
    {
        CheckEventSystem();

        // Автоматически находим все UI элементы
        FindUIElementsAutomatically();

        // Автоматически находим сердечки если массив пустой
        if (hearts == null || hearts.Length == 0)
        {
            FindHeartsAutomatically();
        }

        SortHeartsByPosition();

        // Автоматически находим персонажей если не назначены
        if (players == null || players.Length == 0)
        {
            FindAllPlayers();
        }

        // Автоматически находим камеры если не назначены
        if (playerCameras == null || playerCameras.Length == 0)
        {
            FindAllCameras();
        }

        // Активируем первого персонажа
        SwitchToPlayer(currentPlayerIndex);

        // Инициализация
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        UpdateCoinCounter();

        Debug.Log("Game HUD initialized in game scene");
        Debug.Log($"Health: {currentHealth}, Coins: {collectedCoins}, Players: {players.Length}");
    }

    void FindAllPlayers()
    {
        // Ищем всех персонажей с тегом Player
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");
        if (foundPlayers.Length > 0)
        {
            players = foundPlayers;
            Debug.Log("Found players: " + players.Length);
        }
        else
        {
            Debug.LogError("No players found with tag 'Player'!");
        }
    }

    void FindAllCameras()
    {
        // Ищем все камеры в сцене
        Camera[] allCameras = FindObjectsOfType<Camera>();
        List<Camera> playerCameraList = new List<Camera>();

        foreach (Camera cam in allCameras)
        {
            if (cam.CompareTag("MainCamera") || cam.name.Contains("PlayerCamera") || cam.name.Contains("Camera"))
            {
                playerCameraList.Add(cam);
            }
        }

        if (playerCameraList.Count > 0)
        {
            playerCameras = playerCameraList.ToArray();
            Debug.Log("Found cameras: " + playerCameras.Length);
        }
        else
        {
            Debug.LogWarning("No specific cameras found, using all cameras");
            playerCameras = allCameras;
        }
    }

    void SwitchToPlayer(int playerIndex)
    {
        if (players == null || players.Length == 0) return;

        // Деактивируем всех персонажей
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                SetPlayerActive(players[i], false);
            }
        }

        // Деактивируем все камеры
        for (int i = 0; i < playerCameras.Length; i++)
        {
            if (playerCameras[i] != null)
            {
                playerCameras[i].gameObject.SetActive(false);
            }
        }

        // Активируем выбранного персонажа и его камеру
        currentPlayerIndex = playerIndex;

        if (players[currentPlayerIndex] != null)
        {
            SetPlayerActive(players[currentPlayerIndex], true);
            Debug.Log("Switched to player: " + players[currentPlayerIndex].name);
        }

        // Активируем соответствующую камеру
        if (playerCameras.Length > currentPlayerIndex && playerCameras[currentPlayerIndex] != null)
        {
            playerCameras[currentPlayerIndex].gameObject.SetActive(true);
            Debug.Log("Switched to camera: " + playerCameras[currentPlayerIndex].name);
        }
        else if (playerCameras.Length > 0 && playerCameras[0] != null)
        {
            // Если нет отдельной камеры для этого персонажа, используем первую
            playerCameras[0].gameObject.SetActive(true);
        }
    }

    void SetPlayerActive(GameObject player, bool active)
    {
        // Включаем/выключаем компоненты управления
        MonoBehaviour[] components = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component != null && component != this)
            {
                // Пропускаем трансформы и другие не-скриптовые компоненты
                if (component.GetType() != typeof(Transform))
                {
                    component.enabled = active;
                }
            }
        }

        // Включаем/выключаем Rigidbody если есть
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = !active;
        }

        Rigidbody2D rb2D = player.GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.simulated = active;
        }
    }

    void CheckEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.Log("Creating EventSystem...");
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }
    }

    void SortHeartsByPosition()
    {
        if (hearts == null || hearts.Length == 0) return;

        List<TextMeshProUGUI> sortedHearts = new List<TextMeshProUGUI>(hearts);
        sortedHearts.Sort((a, b) => a.rectTransform.anchoredPosition.x.CompareTo(b.rectTransform.anchoredPosition.x));
        hearts = sortedHearts.ToArray();

        Debug.Log("Hearts sorted by position:");
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                Debug.Log($"Heart {i}: {hearts[i].name} (X: {hearts[i].rectTransform.anchoredPosition.x})");
            }
        }
    }

    void FindUIElementsAutomatically()
    {
        if (healthPanel == null)
            healthPanel = GameObject.Find("HealthPanel");
        if (coinsPanel == null)
            coinsPanel = GameObject.Find("CoinsPanel");

        if (coinCounter == null)
        {
            GameObject coinCounterObj = GameObject.Find("CoinCounter");
            if (coinCounterObj != null) coinCounter = coinCounterObj.GetComponent<TextMeshProUGUI>();
        }

        Debug.Log("Game HUD UI elements auto-found");
    }

    void FindHeartsAutomatically()
    {
        Debug.Log("Auto-finding hearts...");

        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        List<TextMeshProUGUI> heartList = new List<TextMeshProUGUI>();

        foreach (TextMeshProUGUI text in allTexts)
        {
            if (text.text.Contains("♥") || text.text.Contains("♡") || text.name.Contains("Heart"))
            {
                heartList.Add(text);
                Debug.Log("Found heart: " + text.name);
            }
        }

        hearts = heartList.ToArray();

        if (hearts.Length == 0)
        {
            hearts = new TextMeshProUGUI[maxHealth];
            Debug.LogWarning("No hearts found! Created empty array");
        }
    }

    void Update()
    {
        // Обработка клавиши Escape - возврат в главное меню
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC KEY PRESSED!");
            ReturnToMainMenu();
        }

        // Переключение персонажей по клавише P
        if (Input.GetKeyDown(switchKey) && players != null && players.Length > 1)
        {
            int nextIndex = (currentPlayerIndex + 1) % players.Length;
            SwitchToPlayer(nextIndex);
        }

        // Тестирование: нажмите H для проверки здоровья
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("=== MANUAL HEALTH TEST ===");
            LoseHealth();
        }

        // Тестирование: нажмите R для восстановления здоровья
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
            Debug.Log("Game reset");
        }

        // Принудительный возврат в меню (тестирование)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("=== FORCED MENU RETURN ===");
            ReturnToMainMenu();
        }
    }

    public void PlayerFell()
    {
        Debug.Log("PlayerFell() called from DeathZone");
        LoseHealth();
    }

    public void LoseHealth()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
            Debug.Log($"Player lost health! Current health: {currentHealth}");

            UpdateHealthDisplay();

            if (currentHealth <= 0)
            {
                Debug.Log("HEALTH REACHED ZERO - GAME OVER!");
                GameOver();
            }
        }
        else
        {
            Debug.Log("Health already at zero!");
        }
    }

    void UpdateHealthDisplay()
    {
        if (hearts.Length == 0)
        {
            Debug.LogError("Hearts array is empty!");
            return;
        }

        Debug.Log($"Updating health display: {currentHealth} hearts");

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                bool isActive = i < currentHealth;
                hearts[i].gameObject.SetActive(isActive);
                Debug.Log($"Heart {i}: {(isActive ? "ACTIVE" : "HIDDEN")}");
            }
        }

        Canvas.ForceUpdateCanvases();
    }

    public void CollectCoin()
    {
        if (collectedCoins < totalCoins)
        {
            collectedCoins++;
            UpdateCoinCounter();
            Debug.Log($"Coin collected! Total: {collectedCoins}/{totalCoins}");

            if (collectedCoins >= totalCoins)
            {
                LevelComplete();
            }
        }
    }

    void UpdateCoinCounter()
    {
        if (coinCounter != null)
        {
            coinCounter.text = $"Монеты: {collectedCoins} / {totalCoins}";
        }
        else
        {
            Debug.LogError("CoinCounter is null!");
        }
    }

    void GameOver()
    {
        Debug.Log("=== GAME OVER - RETURNING TO MAIN MENU ===");
        ReturnToMainMenu();
    }

    void LevelComplete()
    {
        Debug.Log("Level Complete! All coins collected!");
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("RETURNING TO MAIN MENU...");

        if (!string.IsNullOrEmpty(mainMenuScene))
        {
            Debug.Log($"Loading scene: {mainMenuScene}");
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            Debug.LogError("Main menu scene name is not set!");
        }
    }

    public void ResetGame()
    {
        currentHealth = maxHealth;
        collectedCoins = 0;
        UpdateHealthDisplay();
        UpdateCoinCounter();

        // Сбрасываем позицию активного игрока
        if (players != null && players.Length > currentPlayerIndex && players[currentPlayerIndex] != null)
        {
            GameObject player = players[currentPlayerIndex];
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log("Player respawned at spawn point");
            }
            else
            {
                player.transform.position = Vector3.zero;
                Debug.Log("Player respawned at zero position");
            }

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Debug.Log("Player physics reset");
            }
        }
        else
        {
            Debug.LogWarning("Player not found for reset!");
        }
    }
}