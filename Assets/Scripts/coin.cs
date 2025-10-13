using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;

    [Header("Visual Effects")]
    public ParticleSystem glowEffect;        // Постоянный блеск
    public ParticleSystem collectEffectPrefab; // ПРЕФАБ эффекта сбора
    public float collectEffectDuration = 1f;

    [Header("Collection Effect")]
    public float collectionSpeed = 5f;
    public float scaleDownSpeed = 2f;

    private GameHUDManager hudManager;
    private bool isCollected = false;
    private Vector3 targetPosition;
    private bool startCollection = false;

    void Start()
    {
        // Автоматически находим HUD Manager
        if (hudManager == null)
        {
            hudManager = FindObjectOfType<GameHUDManager>();
        }

        // Настраиваем и запускаем эффект блеска
        SetupGlowEffect();

        SetTargetPosition();
    }

    void SetupGlowEffect()
    {
        if (glowEffect != null)
        {
            var emission = glowEffect.emission;
            emission.rateOverTime = 10f;
            emission.enabled = true;

            var main = glowEffect.main;
            main.loop = true;

            glowEffect.Play();
        }
    }

    void Update()
    {
        if (startCollection)
        {
            PlayCollectionAnimation();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectCoin();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectCoin();
        }
    }

    void CollectCoin()
    {
        if (isCollected) return;

        isCollected = true;
        Debug.Log("Coin collected!");

        // Сообщаем HUD о собранной монете
        if (hudManager != null)
        {
            hudManager.CollectCoin();
        }

        // Останавливаем эффект блеска
        if (glowEffect != null)
        {
            glowEffect.Stop();
        }

        // Запускаем эффект сбора
        StartCollectionEffect();
    }

    void StartCollectionEffect()
    {
        // Создаем экземпляр эффекта сбора из префаба
        if (collectEffectPrefab != null)
        {
            ParticleSystem effectInstance = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);

            // НЕ настраиваем burst через код - используем настройки префаба
            effectInstance.Play();

            // Уничтожаем после завершения
            Destroy(effectInstance.gameObject, collectEffectDuration);
        }
        else
        {
            Debug.LogWarning("Collect effect prefab is not assigned!");
        }

        // Отключаем коллайдеры
        DisableColliders();

        // Запускаем анимацию сбора
        startCollection = true;
    }

    void DisableColliders()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D != null) collider2D.enabled = false;
    }

    void PlayCollectionAnimation()
    {
        // Двигаем монету к цели
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, collectionSpeed * Time.deltaTime);

        // Уменьшаем монету
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, scaleDownSpeed * Time.deltaTime);

        // Уничтожаем когда достигли цели
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f && transform.localScale.x < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    void SetTargetPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            targetPosition = player.transform.position + Vector3.up * 1.5f;
        }
        else
        {
            targetPosition = transform.position + Vector3.up * 2f;
        }
    }
}