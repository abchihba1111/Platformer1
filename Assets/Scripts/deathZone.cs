using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Настройки телепортации")]
    public Transform respawnPoint; 
    public Vector3 customPosition = new Vector3(0, 2, 0); 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (respawnPoint != null)
            {
                other.transform.position = respawnPoint.position;
                other.transform.rotation = respawnPoint.rotation; 
            }
            else
            {
                other.transform.position = customPosition;
            }

            ResetRigidbody(other);
        }
    }

    private void ResetRigidbody(Collider player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}