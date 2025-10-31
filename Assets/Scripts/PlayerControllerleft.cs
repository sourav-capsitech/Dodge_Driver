using UnityEngine;

public class PlayerControllerleft : MonoBehaviour
{
    // --- Existing Public Variables and Headers ---
    [Header("Lane Positions (Left Half Only)")]
    public float leftLaneX = -4.05f; 
    public float rightLaneX = -1.35f; 
    public float moveSpeed = 10f;

    // Road limits for the left half
    private float minX = -5.4f;
    private float maxX = 0f; 

    private bool isOnLeft = false; // Start in the RIGHT lane of the LEFT half

    void Start()
    {
        // For 3D physics to work with triggers, ensure the player car has a Rigidbody
        // and its collider (BoxCollider) has 'Is Trigger' checked.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.x = rightLaneX; 
        transform.position = pos;
        
        isOnLeft = false; // Ensure initial state matches the position
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 tapPos = Input.mousePosition;

            // Respond only if the tap is on the LEFT half of the screen
            if (tapPos.x >= Screen.width / 2)
            {
                isOnLeft = !isOnLeft; // Toggle the lane
            }
        }

        float targetX = isOnLeft ? leftLaneX : rightLaneX;
        targetX = Mathf.Clamp(targetX, minX, maxX); 

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
