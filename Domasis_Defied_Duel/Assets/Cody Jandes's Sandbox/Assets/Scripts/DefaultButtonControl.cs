using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultButtonControl : MonoBehaviour
{

    [SerializeField] private GameObject defaultButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Make sure events chooses the right button 
        EventSystem.current.SetSelectedGameObject(null); //Clear Previous
        EventSystem.current.SetSelectedGameObject(defaultButton); //Set default
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
