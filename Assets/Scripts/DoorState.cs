using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorState : MonoBehaviour
{
    public List<GameObject> switchObject;
    public GameObject wispPrefab;

    public bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool flag = false;
        for (int i = 0; i < switchObject.Count; i++) 
        {
            if (!switchObject[i].GetComponent<SwitchState>().objectActive)
            {
                flag = true; break;
            }
        }

        if (!flag)
        {
            isOpen = true;
        }

        if (isOpen)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void HelperWisp()
    {
        for (int i = 0; i < switchObject.Count; i++)
        {
            GameObject wisp = Instantiate(wispPrefab);
            wisp.transform.position = this.transform.position;
            wisp.GetComponent<Wisp>().targetObject = switchObject[i];
        }
    }
}
