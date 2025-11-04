using UnityEngine;

/// <summary>
/// Handles the scrolling and repetition of a single road segment to create
/// the illusion of an infinitely long, endless runner track.
/// 
/// This version makes the road scroll FORWARD (Positive Z).
/// </summary>
public class RoadRepeater : MonoBehaviour
{
    [Header("Scrolling Configuration")]
    [Tooltip("The speed at which the road moves away from the player (Positive Z).")]
    public float scrollSpeed = 15.0f;

    [Tooltip("The length of a single road segment along the Z-axis.")]
    public float roadLength = 19.16f; 

    // The distance to teleport the road backward when it needs to reset (3x the length).
    private float resetDistance;

    // We store the Z position where the road segment started.
    private float startZ;

    private void Start()
    {
        startZ = transform.position.z;
        
        // Use '3' for the smoothest, off-screen transition buffer.
        resetDistance = 3.5f * roadLength;
    }

    private void Update()
    {
        // 1. Move the road segment in the POSITIVE Z direction (forward).
        // Using Vector3.forward is cleaner than -Vector3.back.
        transform.Translate(Vector3.forward * scrollSpeed * Time.deltaTime, Space.World);

        // 2. Check for the reset condition.
        // We reset the segment when it has moved past the total length of the visible road chain.
        // The visible chain is approximately 2 * roadLength away from the starting point 
        // of the first segment. Checking against resetDistance (3 * roadLength) ensures 
        // the segment is fully off-screen before it resets.
        if (transform.position.z > startZ + resetDistance)
        {
            RepositionRoad();
        }
    }

    /// <summary>
    /// Teleports the road segment from the front of the queue back to the end.
    /// </summary>
    private void RepositionRoad()
    {
        // KEY CHANGE: We SUBTRACT the resetDistance to instantly move the segment
        // backward to the beginning of the repeating queue.
        Vector3 newPosition = transform.position;
        newPosition.z -= resetDistance;
        transform.position = newPosition;
    }
}
