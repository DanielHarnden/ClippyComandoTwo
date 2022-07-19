using UnityEngine;

// Some code stolen from an old project.
public class CameraFollow : MonoBehaviour
{
    // The offset from the player and the locked bounds of the camera. Currently used for single rooms.
    public Vector3 offset;
    public Vector2 boundsHigh;
    public Vector2 boundsLow;
    // The smoothing of the camera.
    public float smoothing = 0.0f;
    // The player object, a 0 vector, and a temporary vector respectively.
    private GameObject player;
    private Vector2 velocity;
    private Vector3 temp;

    private Camera thisCam;

    // Sets the player rigidbody (which the camera follows).
    void Start() { player = GameObject.FindGameObjectWithTag("Player"); thisCam = this.gameObject.GetComponent<Camera>(); }

    Vector3 mousePos;
    Vector3 targetPos;

    
    void FixedUpdate() 
    {
        // Locks the cursor to the game
        Cursor.lockState = CursorLockMode.Confined;

        // Takes mouse position and makes it (-1, -1) to (1, 1) instead of (0, 0) to (1, 1)
        mousePos = thisCam.ScreenToViewportPoint(Input.mousePosition);
        mousePos *= 2;
        mousePos -= Vector3.one;

        // Creates a deadzone where the camera only follows the player
        if (mousePos.x <= 0.25 && mousePos.x >= -0.25)
        {
            mousePos.x = 0;
        }

        if (mousePos.y <= 0.25 && mousePos.y >= -0.25)
        {
            mousePos.y = 0;
        }

        // Centers camera between player and mouse (leaning towards mouse)
        targetPos = player.transform.position + (mousePos * 5f);    

        // Smoothly moves the camera to follow the player, applys an offset.
        temp.x = Mathf.Clamp(Mathf.SmoothDamp(transform.position.x, targetPos.x, ref velocity.x, smoothing), boundsLow.x, boundsHigh.x);
        temp.y = Mathf.Clamp(Mathf.SmoothDamp(transform.position.y, targetPos.y, ref velocity.y, smoothing), boundsLow.y, boundsHigh.y);
        temp.z = targetPos.z;
        temp += offset;
        transform.position = temp;
    }
}