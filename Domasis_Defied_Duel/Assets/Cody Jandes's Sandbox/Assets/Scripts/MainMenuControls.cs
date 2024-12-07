using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{
   public void StartGame()
    {
        SceneManager.LoadScene("DDD_Official_Release"); //load desired starting scene - testing own sandbox for now 
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("CreditsScene"); //load credits
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
