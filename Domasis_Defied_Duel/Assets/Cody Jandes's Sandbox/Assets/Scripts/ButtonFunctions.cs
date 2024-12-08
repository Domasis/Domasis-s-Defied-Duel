using UnityEngine;
using UnityEngine.SceneManagement;

//CODY JANDES CREATED THIS SCRIPT 

public class ButtonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.stateUnpause(); //call singleton to tell it to do its thing
    }
    //Simple version of restart
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    //Add in respawn for lose menu
    public void respawn()
    {
        GameManager.instance.playerScript.spawnPlayer();
        GameManager.instance.stateUnpause();
    }

    public void QuittoMain()
    {
        //CHANGE THIS TO QUIT TO MAIN MENU
        SceneManager.LoadScene("MainMenuScene");
       
    }

    public void OntoLevelOne()
    {
        SceneManager.LoadScene("BEL_Level_First");
        GameManager.instance.stateUnpause();
    }

    public void OntoLevelTwo()
    {
        SceneManager.LoadScene("BEL_Level_Final");
        GameManager.instance.stateUnpause();
    }
}
