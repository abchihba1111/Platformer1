using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float RotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }
}