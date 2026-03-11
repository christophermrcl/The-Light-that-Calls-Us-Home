using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class PlayerAttack : MonoBehaviour
{
    public float attackMeleeDamage;
    public GameObject hitPrefab;
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
            GameObject hit = Instantiate(hitPrefab);
            hit.transform.position = other.transform.position;
            other.gameObject.GetComponent<EnemyHP>().Hurt(attackMeleeDamage);
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            GameObject hit = Instantiate(hitPrefab);
            hit.transform.position = other.transform.position;
            other.gameObject.GetComponent<BossHP>().Hurt(attackMeleeDamage);
        }
    }

}
