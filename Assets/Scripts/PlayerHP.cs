using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public Vector3 checkpoint;

    public float healthAmount;
    public float maxHealth = 100;

    private bool isInvisible;

    public float invisibleTime;
    private float invisibleBuffer = 0f;
    public float blinkInterval = 0.1f;
    private bool isBlinking;
    private SpriteRenderer sr;

    public Image healthFill;

    public Image playerIconImg;
    public Sprite hurtIcon;
    public Sprite playerIcon;
    // Start is called before the first frame update
    void Start()
    {
        healthAmount = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (invisibleBuffer > 0f)
        {
            invisibleBuffer -= Time.deltaTime;
            isInvisible = true;
        }
        else
        {
            isInvisible = false;
        }

        if (isInvisible)
        {
            playerIconImg.sprite = hurtIcon;
        }
        else
        {
            playerIconImg.sprite = playerIcon;
        }

        healthFill.fillAmount = healthAmount / maxHealth;

        if(healthAmount <= 0f)
        {
            Dead();
        }
    }

    public void Dead()
    {
        this.transform.position = checkpoint;

        Destroy(GameObject.FindGameObjectWithTag("Boss"));  

        healthAmount = maxHealth;
    }

    public void Hurt(float damage)
    {
        if (isInvisible)
        {
            return;
        }

        invisibleBuffer = invisibleTime;
        healthAmount -= damage;
        StartBlinking();
    }

    public void StartBlinking()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkCoroutine());
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        isBlinking = true;
        float timer = 0f;

        while (timer < invisibleTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        sr.enabled = true;
        isBlinking = false;
    }
}
