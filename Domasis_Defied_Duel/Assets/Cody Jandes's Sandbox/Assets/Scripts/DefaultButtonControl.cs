using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultButtonControl : MonoBehaviour
{
    //Set default button
    [SerializeField] private GameObject defaultButton;

    private GameObject lastSelected;
    private bool isUsingMouse = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Set default on start 
        SetDefaultButton();
    }

    // Update is called once per frame
    void Update()
    {
        //Handle mouse if clicked
        if (Input.GetMouseButtonDown(0))
        {
            isUsingMouse = true;
        }

        //Handle Arrow keys 
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            //turn mouse off
            isUsingMouse= false;

            //If no button selected
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected ?? defaultButton);
            }
        }

        //If button is selected
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }

    private void SetDefaultButton()
    {
        //Make sure events chooses the right button 
        EventSystem.current.SetSelectedGameObject(null); //Clear Previous
        EventSystem.current.SetSelectedGameObject(defaultButton); //Set default

        //Save default as last 
        lastSelected = defaultButton;
    }
}
