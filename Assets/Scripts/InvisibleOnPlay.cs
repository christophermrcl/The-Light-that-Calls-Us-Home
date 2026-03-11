using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleOnPlay : MonoBehaviour
{
    private MeshRenderer m_Renderer;
    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
