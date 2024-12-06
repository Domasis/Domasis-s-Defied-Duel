using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    public void BackToMain()
    {
        //Used to load back to main menu
        SceneManager.LoadScene("MainMenuScene");
    }
}
