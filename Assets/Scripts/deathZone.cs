using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Настройки телепортации")]
    public Transform respawnPoint;
    public Vector3 customPosition = new Vector3(0, 2, 0);

    private void OnTriggerEnter(Collider other)
    {
        HandlePlayerDeath(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandlePlayerDeath(other.gameObject);
    }

    private void HandlePlayerDeath(GameObject playerObject)
    {
        if (playerObject.CompareTag("Player"))
        {
            Debug.Log("Player entered death zone!");

            // Телепортируем игрока
            if (respawnPoint != null)
            {
                playerObject.transform.position = respawnPoint.position;
                playerObject.transform.rotation = respawnPoint.rotation;
                Debug.Log("Player teleported to respawn point");
            }
            else
            {
                playerObject.transform.position = customPosition;
                Debug.Log("Player teleported to custom position");
            }

            ResetRigidbody(playerObject);

            // Ищем GameHUDManager в сцене
            GameHUDManager hudManager = FindObjectOfType<GameHUDManager>();
            if (hudManager != null)
            {
                hudManager.PlayerFell();
                Debug.Log("GameHUDManager found and PlayerFell() called");
            }
            else
            {
                Debug.LogError("GameHUDManager not found in scene!");
            }
        }
    }

    private void ResetRigidbody(GameObject playerObject)
    {
        Rigidbody rb3D = playerObject.GetComponent<Rigidbody>();
        if (rb3D != null)
        {
            rb3D.linearVelocity = Vector3.zero;
            rb3D.angularVelocity = Vector3.zero;
        }

        Rigidbody2D rb2D = playerObject.GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
        }
    }
}