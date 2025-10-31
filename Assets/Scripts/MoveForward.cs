using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f; // Customize speed per prefab in Inspector

    private void Update()
    {
        // Move the object forward continuously along its local Z-axis
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}
