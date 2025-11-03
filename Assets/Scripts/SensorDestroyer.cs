using UnityEngine;

public class SensorDestroyer : MonoBehaviour
{
    [Header("Sensor Configuration")]
    [Tooltip("Set this to true if you only want to destroy specific tagged objects.")]
    public bool useTagFilter = false;

    [Tooltip("Only objects with this tag will be destroyed (if tag filter is enabled).")]
    public string targetTag = "Untagged";

    private void OnTriggerEnter(Collider other)
    {
        // If tag filtering is on, only destroy objects with the specified tag
        if (useTagFilter)
        {
            if (other.CompareTag(targetTag))
            {
                Destroy(other.gameObject);
                Debug.Log($"Sensor destroyed object with tag: {targetTag}");
            }
        }
        else
        {
            // Destroy all objects that collide with this sensor
            Destroy(other.gameObject);
            Debug.Log($"Sensor destroyed object: {other.name}");
        }
    }
}
