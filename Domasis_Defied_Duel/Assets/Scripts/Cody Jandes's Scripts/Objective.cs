using UnityEngine;

public class Objective : MonoBehaviour
{

    //CODY JANDES CREATED THIS SCRIPT 

    [SerializeField] Renderer model;

    Color colorOriginal;

    //Get and Set originalColor
    public Color GetOriginalColor()
    {
        return colorOriginal;
    }
    public void SetOriginalColor(Color newColor)
    {
        colorOriginal = newColor;
    }
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetOriginalColor(model.material.color);
        GameManager.instance.updateSecondaryGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.updateSecondaryGameGoal(-1);
            Destroy(gameObject);
        }

    }


}
