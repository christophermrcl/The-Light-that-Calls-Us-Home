using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BossHP : MonoBehaviour
{
    public float maxHP;
    private float currHP;

    public GameObject HP;
    public Image fillImage;
    // Start is called before the first frame update
    void Start()
    {
        currHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {

        if (currHP <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }

        

        fillImage.fillAmount = currHP / maxHP;
    }

    public void Hurt(float damage)
    {
        currHP -= damage;
    }
}
