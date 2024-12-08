using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{
   public void StartGame()
    {
        SceneManager.LoadScene("BEL_Level_Tutorial"); //load desired starting scene - testing own sandbox for now 
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("CreditsScene"); //load credits
    }

    public void OpenShowcase()
    {
        SceneManager.LoadScene("BEL_Level_Showcase"); //load showcase level
    }

    public void QuitGame()
    {
        //Use previous code for quittting the application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
