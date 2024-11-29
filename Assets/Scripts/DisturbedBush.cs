using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisturbedBush : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationAmount = 45f; // Amount to rotate (in degrees)
    [SerializeField] private float rotationSpeed = 2f; // Speed at which rotation reverts to 0
    [SerializeField] private float cooldownTime = 1f; // Cooldown time between triggers

    private bool canTrigger = true; // Flag to check if the cooldown is active
    private float currentRotation = 0f; // Current rotation state
    private float targetRotation = 0f; // Target rotation to rotate to
    private float lastTriggerTime = 0f; // Time when the trigger was last activated

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is tagged as Player or Enemy
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // Check if the cooldown has expired
            if (canTrigger)
            {
                // Get the velocity direction of the object (assuming it has a SpriteRenderer)
                SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Determine the direction the object is moving
                    if (sr.flipX) // Moving left
                    {
                        targetRotation = -rotationAmount;
                    }
                    else // Moving right
                    {
                        targetRotation = rotationAmount;
                    }
                }

                // Start cooldown by setting canTrigger to false
                canTrigger = false;
                lastTriggerTime = Time.time;
            }
        }
    }

    private void Update()
    {
        // Handle the cooldown based on the elapsed time since the last trigger
        if (!canTrigger && Time.time - lastTriggerTime >= cooldownTime)
        {
            canTrigger = true; // Allow the next trigger after cooldown
        }

        // Smoothly rotate to the target rotation
        if (currentRotation != targetRotation)
        {
            currentRotation = Mathf.MoveTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
        else if (currentRotation == targetRotation && targetRotation != 0f)
        {
            // Once it reaches the target, set target to 0
            targetRotation = 0f;
        }
        else if (currentRotation != 0f)
        {
            // Slowly revert to 0 rotation
            currentRotation = Mathf.MoveTowards(currentRotation, 0f, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }
}