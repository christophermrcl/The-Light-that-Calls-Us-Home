using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int maxHP;
    private int currHP;
    // Start is called before the first frame update
    void Start()
    {
        currHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if(currHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Attacked(int damage)
    {
        currHP = currHP - damage;
    }
}
