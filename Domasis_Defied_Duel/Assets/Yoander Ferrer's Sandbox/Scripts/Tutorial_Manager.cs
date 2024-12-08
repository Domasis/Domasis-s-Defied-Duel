using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(TMP_Text))]
public class Tutorial_Manager : MonoBehaviour
{
    static Tutorial_Manager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
