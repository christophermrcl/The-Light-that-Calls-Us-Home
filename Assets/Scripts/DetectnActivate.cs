using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectnActivate : MonoBehaviour
{
    public GameObject toActivate;
    public GameObject bossActive;
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
        if (other.gameObject.CompareTag("Player") && bossActive==null)
        {
            GameObject boss = Instantiate(toActivate);
            boss.transform.position = this.transform.position;
            bossActive = boss;
        }
    }
}
