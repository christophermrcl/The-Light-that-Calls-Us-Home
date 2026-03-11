using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class MainMenu : MonoBehaviour
{
    public GameObject kamera;
    public GameObject kanvas;
    public Image panel;

    public Vector3 targetPosition; // The position to move to
    public float moveSpeed = 2f;   // Speed of movement
    public float tolerance = 0.1f; // Distance at which the object is considered "at the position"

    private bool isMoving = false; // Track if movement is in progress

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayDemo()
    {
        Debug.Log("aaa");
        kanvas.SetActive(false);
        StartCoroutine(MoveObject(targetPosition, OnReachTarget));
        StartCoroutine(FadePanelAlpha(panel, 0, 1, 0.7f));
    }

    IEnumerator MoveObject(Vector3 destination, System.Action onComplete)
    {
        isMoving = true;

        // Keep moving until close enough to the target position
        while (Vector3.Distance(kamera.transform.position, destination) > tolerance)
        {
            kamera.transform.position = Vector3.MoveTowards(kamera.transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null; // Wait until the next frame
        }

        // Snap to the exact position
        kamera.transform.position = destination;

        // Trigger the callback action
        onComplete?.Invoke();

        isMoving = false;
    }

    // Define what happens when the object reaches the target
    void OnReachTarget()
    {
        SceneManager.LoadScene("Demo");
    }

    public IEnumerator FadePanelAlpha(Image panel, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        // Initialize the panel alpha
        panel.color = new Color (255f, 255f, 255f, startAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panel.color = new Color(255f, 255f, 255f, Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration));
            yield return null;
        }

        // Ensure the final alpha is set
        panel.color = new Color(255f, 255f, 255f, endAlpha);

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
