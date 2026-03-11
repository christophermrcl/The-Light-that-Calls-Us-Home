using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DoorEnemy : MonoBehaviour
{
    public List<GameObject> enemyObject;
    public GameObject wispPrefab;

    public bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyObject.Count <= 0)
        {
            isOpen = true;
        }

        if (enemyObject.Count > 0 && enemyObject[0] == null)
        {
            enemyObject.Remove(enemyObject[0]);
        }

        if (isOpen)
        {
            this.gameObject.SetActive(false);
        }
    }
}
