using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    //void Update()
    //{
    //    transform.rotation = Camera.main.transform.rotation;
    //}

    [SerializeField] private BillboardType billboardType;
    [SerializeField] private Camera cameraToLook;

    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;

    public enum BillboardType { LookAtCamera, CameraForward };

    private void Awake()
    {
        if (cameraToLook == null)
        {
            cameraToLook = Camera.main;
        }
        originalRotation = transform.rotation.eulerAngles;
    }

    // Use Late update so everything should have finished moving.
    void LateUpdate()
    {
        // There are two ways people billboard things.
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                //transform.LookAt(cameraToLook.transform.position, Vector3.up);
                transform.forward = cameraToLook.transform.forward;
                break;
            case BillboardType.CameraForward:
                transform.forward = cameraToLook.transform.forward;
                break;
            default:
                break;
        }
        // Modify the rotation in Euler space to lock certain dimensions.
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) { rotation.x = originalRotation.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
        transform.rotation = Quaternion.Euler(rotation);
    }
}
