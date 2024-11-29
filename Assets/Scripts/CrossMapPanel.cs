using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossMapPanel : MonoBehaviour
{
    public GameObject targetPath;

    const float fadeDuration = 1f;
    private float fadeBuffer = 0f;
    public bool isFade = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFade)
        {
            GetComponent<Image>().color = new Color(0f, 0f, 0f, fadeBuffer / fadeDuration);
            fadeBuffer += Time.deltaTime;

            if (fadeBuffer >= fadeDuration)
            {
                isFade = false;

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<CharacterController>().enabled = false;
                player.transform.position = targetPath.transform.position;
                player.gameObject.GetComponent<CharacterController>().enabled = true;
            }
        }
        else
        {
            if (fadeBuffer > 0f)
            {
                GetComponent<Image>().color = new Color(0f, 0f, 0f, fadeBuffer / fadeDuration);
                fadeBuffer -= Time.deltaTime;
            }
            else
            {
                GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            }
        }
    }
}
