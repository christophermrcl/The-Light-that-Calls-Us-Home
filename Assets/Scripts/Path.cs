using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Paths : MonoBehaviour
{
    public GameObject targetPath;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("BlackPanel").GetComponent<CrossMapPanel>().targetPath = targetPath;
            GameObject.FindGameObjectWithTag("BlackPanel").GetComponent<CrossMapPanel>().isFade = true;

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().checkpoint = this.transform.position;
        }
    }
}
