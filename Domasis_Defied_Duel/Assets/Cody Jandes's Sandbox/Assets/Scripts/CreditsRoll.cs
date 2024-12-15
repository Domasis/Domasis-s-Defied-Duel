using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RollingCredits : MonoBehaviour
{
    [SerializeField] private RectTransform creditsText; 
    [SerializeField] private float scrollSpeed = 50f; 
    [SerializeField] private float endDelay = 5f; 
    [SerializeField] private string mainMenuSceneName = "MainMenuScene"; 

    private bool creditsEnded = false;

    private void Update()
    {
        if (!creditsEnded)
        {
            // Scroll the credits
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // Check if the credits are off screen
            if (creditsText.anchoredPosition.y >= creditsText.rect.height)
            {
                creditsEnded = true;
                Invoke(nameof(ReturnToMainMenu), endDelay);
            }
        }

        // Allow skipping credits with a key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToMainMenu();
        }
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
