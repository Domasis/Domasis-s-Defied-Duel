using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Renderer model;
    [SerializeField] private Transform playerSpawnPos;
    [SerializeField] private GameObject checkpointPopup;

    private Color origColor;

    public Renderer Model
    {
        get => model;
        set => model = value;
    }

    public Color OrigColor
    {
        get => origColor;
        private set => origColor = value;
    }

    public Transform PlayerSpawnPos
    {
        get => playerSpawnPos;
        set => playerSpawnPos = value;
    }

    public GameObject CheckpointPopup
    {
        get => checkpointPopup;
        set => checkpointPopup = value;
    }

    void Start()
    {
        if (Model != null)
        {
            OrigColor = Model.material.color;
        }
        //else
        //{
        //    Debug.LogWarning("Model is not assigned in the Inspector.");
        //}

        //if (PlayerSpawnPos == null)
        //{
        //    Debug.LogWarning("PlayerSpawnPos is not assigned in the Inspector.");
        //}

        //if (CheckpointPopup == null)
        //{
        //    Debug.LogWarning("CheckpointPopup is not assigned in the Inspector.");
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && transform.position != GameManager.instance.GetPlayerSpawnPoint().transform.position)
        {
            GameManager.instance.GetPlayerSpawnPoint().transform.position = transform.position;

            StartCoroutine(FlashColor());
        }
    }

    private IEnumerator FlashColor()
    {
       Model.material.color = Color.red; 
        GameManager.instance.GetCheckpointPopup().SetActive(true);
        yield return new WaitForSeconds(0.75f);
        GameManager.instance.GetCheckpointPopup().SetActive(false);
        Model.material.color = origColor;

    }

}
