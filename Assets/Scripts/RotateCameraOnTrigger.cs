using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraOnTrigger : MonoBehaviour
{
    [SerializeField]
    private Camera cameraToControl; // Camera to control

    [SerializeField]
    private Quaternion targetRotation; // Target rotation to rotate to when the player enters the trigger

    [SerializeField]
    private Vector3 positionOffset; // Offset from the camera's current position when the player enters the trigger

    public Quaternion initialRotation; // Initial rotation of the camera
    public Vector3 initialPosition; // Initial position of the camera

    private void Start()
    {
        // Store the initial rotation and position of the camera
        if (cameraToControl != null)
        {
            initialRotation = cameraToControl.transform.rotation;
            initialPosition = cameraToControl.transform.localPosition;
        }
        else
        {
            Debug.LogWarning("Camera is not assigned.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the object with the "Player" tag is in the trigger
        if (other.CompareTag("Player"))
        {
            // If the player is in the trigger, rotate the camera to the target rotation
            if (cameraToControl != null)
            {
                cameraToControl.transform.rotation = targetRotation;

                // Apply position offset only if the player is in the trigger
                cameraToControl.transform.localPosition = initialPosition + positionOffset;
            }
        }
        else
        {
            // If the player is not in the trigger, reset the camera's rotation and position to the initial ones
            if (cameraToControl != null)
            {
                cameraToControl.transform.rotation = initialRotation;
                cameraToControl.transform.localPosition = initialPosition; // No offset applied
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If the player is in the trigger, rotate the camera to the target rotation
            if (cameraToControl != null)
            {
                cameraToControl.transform.rotation = initialRotation;

                // Apply position offset only if the player is in the trigger
                cameraToControl.transform.localPosition = initialPosition;
            }
        }
    }
}
