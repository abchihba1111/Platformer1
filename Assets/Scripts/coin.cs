using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;

    [Header("Visual Effects")]
    public ParticleSystem glowEffect;        // ���������� �����
    public ParticleSystem collectEffectPrefab; // ������ ������� �����
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
        // ������������� ������� HUD Manager
        if (hudManager == null)
        {
            hudManager = FindObjectOfType<GameHUDManager>();
        }

        // ����������� � ��������� ������ ������
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

        // �������� HUD � ��������� ������
        if (hudManager != null)
        {
            hudManager.CollectCoin();
        }

        // ������������� ������ ������
        if (glowEffect != null)
        {
            glowEffect.Stop();
        }

        // ��������� ������ �����
        StartCollectionEffect();
    }

    void StartCollectionEffect()
    {
        // ������� ��������� ������� ����� �� �������
        if (collectEffectPrefab != null)
        {
            ParticleSystem effectInstance = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);

            // �� ����������� burst ����� ��� - ���������� ��������� �������
            effectInstance.Play();

            // ���������� ����� ����������
            Destroy(effectInstance.gameObject, collectEffectDuration);
        }
        else
        {
            Debug.LogWarning("Collect effect prefab is not assigned!");
        }

        // ��������� ����������
        DisableColliders();

        // ��������� �������� �����
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
        // ������� ������ � ����
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, collectionSpeed * Time.deltaTime);

        // ��������� ������
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, scaleDownSpeed * Time.deltaTime);

        // ���������� ����� �������� ����
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