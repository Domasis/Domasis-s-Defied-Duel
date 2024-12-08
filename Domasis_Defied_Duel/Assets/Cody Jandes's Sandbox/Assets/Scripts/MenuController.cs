using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject optionsMenuPanel;

    public void OpenOptionsMenu()
    {
        optionsMenuPanel.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenuPanel?.SetActive(false);
    }
}
