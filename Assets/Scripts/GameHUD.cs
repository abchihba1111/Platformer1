using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
    public GameObject healthPanel;    // Панель здоровья
    public GameObject coinsPanel;     // Панель монет

    void Start()
    {
        // Автоматически находим сердечки если массив пустой
        if (hearts == null || hearts.Length == 0)
        {
            FindHeartsAutomatically();
        }

        // Инициализация здоровья
        currentHealth = maxHealth;
        UpdateHealthDisplay();

        // Инициализация счетчика монет
        UpdateCoinCounter();

        // Скрываем HUD в начале (будет показан когда начнется игра)
        SetHUDVisible(false);

        Debug.Log("Game HUD initialized");
    }

    void Update()
    {
        // Обработка клавиши Escape - возврат в главное меню
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
    }

    // Включает/выключает отображение HUD
    public void SetHUDVisible(bool visible)
    {
        if (healthPanel != null) healthPanel.SetActive(visible);
        if (coinsPanel != null) coinsPanel.SetActive(visible);
        Debug.Log($"HUD visible: {visible}");
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

    public void PlayerFell()
    {
        Debug.Log("PlayerFell() called");
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
                GameOver();
            }
        }
    }

    void UpdateHealthDisplay()
    {
        if (hearts.Length == 0)
        {
            Debug.LogError("Hearts array is empty!");
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].gameObject.SetActive(i < currentHealth);
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
    }

    void GameOver()
    {
        Debug.Log("Game Over! Returning to main menu...");
        ReturnToMainMenu();
    }

    void LevelComplete()
    {
        Debug.Log("Level Complete! All coins collected!");
    }

    // Метод для возврата в главное меню по Escape
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu (Escape pressed)");

        // Находим MainMenuController и переключаем на меню
        MainMenuController menuController = FindObjectOfType<MainMenuController>();
        if (menuController != null)
        {
            menuController.ShowMainMenu();
        }
        else
        {
            Debug.LogError("MainMenuController not found!");
        }
    }

    public void ResetGame()
    {
        currentHealth = maxHealth;
        collectedCoins = 0;
        UpdateHealthDisplay();
        UpdateCoinCounter();

        // Сброс позиции игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
            else
            {
                player.transform.position = Vector3.zero;
            }

            // Сброс физики
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}