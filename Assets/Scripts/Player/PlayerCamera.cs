using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Name: Player Camera
/// Description: Handles the camera attached to the player character.
/// Date Created: 11/09/22
/// Date Modified: 11/25/22
/// </summary>

public class PlayerCamera : MonoBehaviour
{
    public float mouseSens = 2.5f;
    public float viewTiltIntensity = 2f;
    public float viewBobIntensity, viewBobSpeed;

    float mouseX, mouseY, clamp, viewTilt, viewBob, _vb;
    Camera cam;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        mouseX = transform.parent.eulerAngles.y;
        mouseY = -transform.localEulerAngles.x;
    }

    private void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSens;
        mouseY += Input.GetAxisRaw("Mouse Y") * mouseSens;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            _vb += Time.deltaTime;
        else
            _vb = 0f;

        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        viewTilt = Mathf.Lerp(viewTilt, Input.GetAxisRaw("Horizontal") * viewTiltIntensity, 10f * Time.deltaTime);
        viewBob = viewBobIntensity * Mathf.Sin(_vb * viewBobSpeed);

        transform.localRotation = Quaternion.Euler(-mouseY, 0f, -viewTilt);
        //cam.transform.localPosition = new Vector3(0f, viewBob, 0f);
        //transform.parent.rotation = Quaternion.Euler(0f, mouseX, 0f);
        transform.parent.Rotate(0f, mouseX, 0f);

        if(cam.transform.localEulerAngles.magnitude != 0f)
        {
            cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.identity, 15f * Time.deltaTime);
        }

        //!!!DEBUG FUNCTION, REMOVE LATER!!!
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Knock(20f);
        }
    }

    public void Knock(float power)
    {
        Vector3 dir = Random.insideUnitCircle * power;
        dir.z = Random.Range(-1f, 1f) * power;
        cam.transform.localEulerAngles += dir;
    }
}
