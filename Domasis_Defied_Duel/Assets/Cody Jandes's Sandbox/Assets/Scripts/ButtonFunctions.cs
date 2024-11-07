using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
   public void resume()
    {
        GameManager.instance.stateUnpause(); //call singleton to tell it to do its thing
    }
}
