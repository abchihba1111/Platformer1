using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    void CollectCoin()
    {
        Debug.Log("Coin collected!");

        // »щем любой компонент с методом CollectCoin
        MonoBehaviour[] allComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour component in allComponents)
        {
            System.Type type = component.GetType();
            System.Reflection.MethodInfo method = type.GetMethod("CollectCoin");

            if (method != null)
            {
                method.Invoke(component, null);
                Debug.Log("Coin collected via: " + type.Name);
                break;
            }
        }

        // ”ничтожаем монету
        Destroy(gameObject);
    }
}