using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackMeleeDamage;
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
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyHP>().Attacked(attackMeleeDamage);
        }
        if (other.gameObject.CompareTag("Door"))
        {
            other.gameObject.GetComponent<DoorState>().HelperWisp();
        }
    }
}
