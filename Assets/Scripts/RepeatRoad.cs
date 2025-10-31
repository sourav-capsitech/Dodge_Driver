using UnityEngine;

public class RoadRepeater : MonoBehaviour
{
    [Header("Movement Speed")]
    // This speed must match the speed of your police cars and coins 
    // to keep the game visually coherent.
    public float scrollSpeed = 10f; 

    [Header("Road Segment Configuration")]
    [Tooltip("The total length of this road segment in Unity units (Y-axis).")]
    public float roadLength = 30f; // IMPORTANT: Set this to the exact length of your road asset
    
    // CORRECTION 1: Road segments now scroll UPWARDS (Positive Y direction)
    private Vector3 scrollDirection = Vector3.up;

    private Vector3 startPosition; // Stores the initial position of the road

    void Start()
    {
        // Store the initial position
        startPosition = transform.position;

        // Note: The road segments should be perfectly placed end-to-end at the start.
    }

    void Update()
    {
        // Safety Check: Only scroll if the game is not paused (Time.timeScale > 0)
        if (Time.timeScale == 0)
        {
            return;
        }

        // 1. Calculate movement delta
        // Move the road segment up the screen based on the scroll speed and frame time
        float moveDelta = scrollSpeed * Time.deltaTime;

        // 2. Apply movement
        // We use scrollDirection (Vector3.up) to move along the Y-axis
        transform.Translate(scrollDirection * moveDelta);

        // CORRECTION 2: Check for reset condition
        // The road needs to be reset when its current Y position is GREATER than 
        // the starting Y position PLUS the full road length (since we are moving up).
        if (transform.position.y > startPosition.y + roadLength)
        {
            RepositionRoad();
        }
    }

    /// <summary>
    /// Instantly moves the road segment to the position directly BELOW 
    /// its starting point to create the seamless loop.
    /// </summary>
    private void RepositionRoad()
    {
        // CORRECTION 3: The offset must now be SUBTRACTED.
        // This repositions the segment exactly two lengths DOWN, placing it 
        // back at the beginning of the loop.
        Vector3 offset = new Vector3(0, roadLength * 2, 0); 
        transform.position = transform.position - offset;
    }
}
