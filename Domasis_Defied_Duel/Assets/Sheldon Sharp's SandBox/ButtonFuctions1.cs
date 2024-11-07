using UnityEngine;
using UnityEngine.UI;
//Sheldon Sharp
public class ButtonFunctions1 : MonoBehaviour
{
    // Private fields for the buttons
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    // Public properties with getters and setters for button references
    public Button ResumeButton
    {
        get => resumeButton;
        set
        {
            resumeButton = value;
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);
        }
    }

    public Button RestartButton
    {
        get => restartButton;
        set
        {
            restartButton = value;
            if (restartButton != null)
                restartButton.onClick.AddListener(Restart);
        }
    }

    public Button QuitButton
    {
        get => quitButton;
        set
        {
            quitButton = value;
            if (quitButton != null)
                quitButton.onClick.AddListener(Quit);
        }
    }

    void Start()
    {
        // Ensure the buttons are linked to their respective methods at start
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);

        if (quitButton != null)
            quitButton.onClick.AddListener(Quit);
    }

    public void Resume()
    {
        GameManager1.instance.StateUnpause();
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        GameManager1.instance.StateUnpause();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
