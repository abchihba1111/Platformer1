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

        // Инициализация
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        UpdateCoinCounter();

        Debug.Log("Game HUD initialized in game scene");
        Debug.Log($"Health: {currentHealth}, Coins: {collectedCoins}");
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

        // Сортируем сердечки по позиции X (слева направо)
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
        // Находим панели
        if (healthPanel == null)
            healthPanel = GameObject.Find("HealthPanel");
        if (coinsPanel == null)
            coinsPanel = GameObject.Find("CoinsPanel");

        // Находим счетчик монет
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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
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