using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractiveTooltipObj : MonoBehaviour
{
    // Editor exposed string that stores the message held by this instance of the TutorialTooltip. MUST BE FILLED OUT FOR THE TOOLTIP TO DISPLAY A MESSAGE.
    [SerializeField] string tooltipMsg;

    // Sphere collider, required for OnTriggerStay implementation.
    SphereCollider interactiveCollider;

    public string TooltipMsg { get => tooltipMsg; set => tooltipMsg = value; }
    public SphereCollider InteractiveCollider { get => interactiveCollider; set => interactiveCollider = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (InteractiveCollider == null)
        {

            InteractiveCollider = GetComponent<SphereCollider>();
            InteractiveCollider.isTrigger = true;

        }


    }

    private void OnTriggerStay(Collider other)
    {
        // We want to make sure that the following logic only occurs when the PLAYER is in the collision.
        if (other.CompareTag("Player"))
        {
            // We show the tooltip with this specific instance's message.
            InteractiveTooltipManager.instance.ShowTip(TooltipMsg);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Like with OnTriggerStay, we want to make sure that only a player is triggering this condition.
        if (other.CompareTag("Player"))
        {
            // We disable the interaction popup as the player no longer needs to be reminded on how to interact with the object.
            GameManager.instance.GetInteractPopup().SetActive(false);

            // We also double check if the player is still looking at a tooltip, and disable it.
            InteractiveTooltipManager.instance.HideTooltip();
        }
    }
}
