using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDuration : MonoBehaviour
{
    public float timeToDestroy = 5f;

    void Start()
    {
        // Destroy this GameObject after the specified time
        Destroy(gameObject, timeToDestroy);
    }
}
