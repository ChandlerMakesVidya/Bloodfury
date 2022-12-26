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

    float mouseX, mouseY, clamp, viewTilt;
    [SerializeField] Transform knockObject;

    private void Start()
    {
        mouseX = transform.parent.eulerAngles.y;
        mouseY = -transform.localEulerAngles.x;
    }

    private void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSens;
        mouseY += Input.GetAxisRaw("Mouse Y") * mouseSens;

        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        viewTilt = Mathf.Lerp(viewTilt, Input.GetAxisRaw("Horizontal") * viewTiltIntensity, 10f * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(-mouseY, 0f, -viewTilt);
        //transform.parent.rotation = Quaternion.Euler(0f, mouseX, 0f);
        transform.parent.Rotate(0f, mouseX, 0f);

        if(knockObject.localEulerAngles.magnitude != 0f)
        {
            knockObject.localRotation = Quaternion.Lerp(knockObject.transform.localRotation, Quaternion.identity, 15f * Time.deltaTime);
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
        knockObject.localEulerAngles += dir;
    }
}
