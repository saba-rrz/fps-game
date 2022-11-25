using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region Settings
    
    [Header("Cameras")]
    [SerializeField] private Camera mainCamera; //Main camera
    [SerializeField] private Camera gunCamera; //Gun camera
    
    [Header("Settings")]
    [SerializeField] private float sensitivity; //Sensitivity value
    
    #endregion

    #region Variables

    private const float MixX = -60f; //Min X rotation
    private const float MaxX = 60f; //Max X rotation
    private float _rotY; //Y rotation
    private float _rotX; //X rotation
    private PlayerMovementScript _moveScript; //PlayerMovementScript component
    
    #endregion
    
    void Start() //Start Function
    {
        Cursor.lockState = CursorLockMode.Locked; //Locks the cursor in place
        Cursor.visible = false; //Turns off the cursor

        _moveScript = GetComponent<PlayerMovementScript>(); //Gets the PlayerMovementScript component
    }
    
    void Update() //Update Function
    {
        _rotY += Input.GetAxis("Mouse X") * sensitivity; //Handles the input from the mouse
        _rotX += Input.GetAxis("Mouse Y") * sensitivity; //Handles the input from the mouse

        _rotX = Mathf.Clamp(_rotX, MixX, MaxX); //Clamps the camera  with the Min and Max values

        transform.localEulerAngles = new Vector3(0, _rotY, 0); //Moves the cameras
        mainCamera.transform.localEulerAngles = new Vector3(-_rotX, 0, _moveScript.tilt); //Moves the main camera
        gunCamera.transform.localEulerAngles = new Vector3(-_rotX, 0, _moveScript.tilt); //Moves the gun camera
    }
}
