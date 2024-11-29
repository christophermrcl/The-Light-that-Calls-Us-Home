using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchState : MonoBehaviour
{
    public bool objectActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity))
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.CompareTag("Movable"))
            {
                
                objectActive = true;
            }
        }

        Debug.DrawRay(transform.position, Vector3.up, Color.green);
    }

}
