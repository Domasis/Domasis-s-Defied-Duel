using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnOff : MonoBehaviour
{

    [SerializeField] Light lightSource;

    public Light LightSource { get => lightSource; set => lightSource = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lightSource.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lightSource.enabled = false;
        }
    }
}