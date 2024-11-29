using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour
{
    public GameObject targetObject; // The target object to move towards
    private float speed = 2f; // Movement speed
    private float countdownTime = 4f; // Countdown time before destruction

    private bool reachedTarget = false; // Flag to check if the target is reached
    private float countdownTimer;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object is not assigned!");
        }
        countdownTimer = countdownTime;
    }

    void Update()
    {
        if (targetObject == null) return;

        if (!reachedTarget)
        {
            MoveTowardsTarget();
        }
        else
        {
            HandleCountdown();
        }
    }

    void MoveTowardsTarget()
    {
        // Move towards the target object
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + 0.5f, targetObject.transform.position.z), step);

        // Check if reached the target
        if (Vector3.Distance(transform.position, new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + 0.5f, targetObject.transform.position.z)) < 0.1f)
        {
            reachedTarget = true;
        }
    }

    void HandleCountdown()
    {
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0f)
        {
            Destroy(gameObject); // Destroy the current game object
        }
    }
}
