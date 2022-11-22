using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseLook : MonoBehaviour
{
    public float mixX = -60f;

    public float maxX = 60f;

    public float sensitivity;

    public Camera Camera;

    private float _rotY = 0f;

    private float _rotX = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        _rotY += Input.GetAxis("Mouse X") * sensitivity;
        _rotX += Input.GetAxis("Mouse Y") * sensitivity;

        _rotX = Mathf.Clamp(_rotX, mixX, maxX);

        transform.localEulerAngles = new Vector3(0, _rotY, 0);
        Camera.transform.localEulerAngles = new Vector3(-_rotX, 0, 0);
    }
}
