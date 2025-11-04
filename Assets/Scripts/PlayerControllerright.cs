using UnityEngine;

public class PlayerControllerright : MonoBehaviour
{
    [Header("Lane Positions (Right Half Only)")]
    // Lane 3: Near Center (Center point of the 0.0 to 2.7 range)
    public float leftLaneX = 1.35f; 
    // Lane 4: Far Right (Center point of the 2.7 to 5.4 range)
    public float rightLaneX = 4.05f; 
    
    public float moveSpeed = 10f;

    // Road limits derived from 10.8 total width centered at 0
    private const float minX = 0f; // The central line
    private const float maxX = 3.6f;

    // Tracks if the car is currently in the near center lane (leftLaneX) or the far right lane (rightLaneX).
    // true = Near Center (1.35f) | false = Far Right (4.05f)
    private bool isOnLeft = true; // Initialize to true (Near Center)

    void Start()
    {
        // 1. Clamp position to the valid half
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        
        // 2. Ensure the car starts in the initial lane (Near Center) and match the boolean state.
        pos.x = leftLaneX; 
        transform.position = pos;
        
        // isOnLeft is already true, matching the start at leftLaneX.
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 tapPos = Input.mousePosition;

            // Respond only if the tap is on the RIGHT half of the screen
            if (tapPos.x < Screen.width / 2) 
            {
                // Toggle the lane state
                isOnLeft = !isOnLeft;
            }
        }

        // Determine the target X position based on the current state
        float targetX = isOnLeft ? leftLaneX : rightLaneX;

        // Safety clamp ensures the target is within the road bounds
        targetX = Mathf.Clamp(targetX, minX, maxX);

        // Smoothly move towards the target position
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    // --- CORRECTION: Using 3D OnTriggerEnter for 3D Box Colliders/Rigidbodies ---
    /// <summary>
    /// Handles collisions with obstacles (Cars/Police) and collectibles (Coins).
    /// Assumes all objects use 3D Colliders (BoxCollider, etc.) and they are set to 'Is Trigger'.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // IMPORTANT: Check if the GameManager instance exists before trying to use it!
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL! Check Script Execution Order.");
            return;
        }

        // Check for collision with the car obstacle (assuming tag is "Car" or "Police")
        if (other.CompareTag("Car")) // Added "Police" tag check for safety
        {
            // Game Over for all players
            GameManager.Instance.GameOver();
        }

        // Check for collision with the coin collectible (assuming tag is "Coin")
        else if (other.CompareTag("Coin"))
        {
            // Collect the coin and destroy it
            GameManager.Instance.CollectCoin(other.gameObject);
        }
    }
    
    
}
