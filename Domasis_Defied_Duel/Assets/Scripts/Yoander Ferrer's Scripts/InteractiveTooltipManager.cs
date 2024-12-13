using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class InteractiveTooltipManager : MonoBehaviour
{
    // Static instance of the manager to enforce a singleton pattern. We only need one to manage all tooltips.
    public static InteractiveTooltipManager instance;

    [SerializeField] TMP_Text tipText;

    [SerializeField] Canvas tipCanvas;

    // Error text that will appear if no message was added to the object.
    readonly string origErrorMsg = "No Message Was Added! Add a message in the string field of the editor.";

    public TMP_Text TipText { get => tipText; set => tipText = value; }
    public Canvas TipCanvas { get => tipCanvas; set => tipCanvas = value; }

    public string OrigErrorMsg => origErrorMsg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // We set the instance to this to ensure that only one instance is known globally.
        instance = this;

        if (TipText == null)
        {
            TipText = GetComponent<TMP_Text>();
        }

        if (TipCanvas == null)
        {
            TipCanvas = GetComponent<Canvas>();
        }

        TipCanvas.enabled = false;
        TipText.text = OrigErrorMsg;
    }

    public void ShowTip(string msg)
    {

        TipCanvas.enabled = true;
        GameManager.instance.statePause();
        if (msg != null)
        { TipText.text = msg; }
        else { TipText.text = origErrorMsg; }
        

    }

    public void HideTooltip()
    {
        GameManager.instance.isPaused = false;
        Time.timeScale = GameManager.instance.TimeScaleOriginal;
        Cursor.visible = false; //don't see cursor when paused
        Cursor.lockState = CursorLockMode.Locked; //relock cursor

        TipCanvas.enabled = false;
        TipText.text = OrigErrorMsg;
    }
}
