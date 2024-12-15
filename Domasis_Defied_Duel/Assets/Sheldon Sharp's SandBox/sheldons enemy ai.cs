using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Import NavMesh for enemy navigation

// CODY JANDES CREATED THIS SCRIPT 
public class EnemyAI : MonoBehaviour, TakesDamage
{
    [SerializeField] private Renderer model;
    [SerializeField] private int enemyHP;
    [SerializeField] private NavMeshAgent agent; // NavMeshAgent for movement

    private Color colorOriginal;
    private Color damageOne;

    // Get and Set originalColor
    public Color GetOriginalColor()
    {
        return colorOriginal;
    }
    public void SetOriginalColor(Color newColor)
    {
        colorOriginal = newColor;
    }

    // Get and Set damageOne
    public Color GetDamageOneColor()
    {
        return damageOne;
    }
    public void SetDamageOneColor(Color newColor)
    {
        damageOne = newColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetOriginalColor(model.material.color);
        SetDamageOneColor(Color.red);
        GameManager.instance.updateGameGoal(1);

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>(); // Ensure NavMeshAgent is set
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Additional enemy behavior can be added here
    }

    public void TakeSomeDamage(int amount)
    {
        // Reduce HP by the damage amount
        enemyHP -= amount;

        // Start Coroutine to flash color on hit
        StartCoroutine(FlashHit());

        // Check if enemy HP is depleted
        if (enemyHP <= 0)
        {
            GameManager.instance.updateGameGoal(-1); // Update game goal
            Destroy(gameObject); // Destroy enemy
        }
    }

    // Coroutine to flash color when enemy is hit
    private IEnumerator FlashHit()
    {
        model.material.color = GetDamageOneColor(); // Set material to damage color
        yield return new WaitForSeconds(0.1f); // Timer
        model.material.color = GetOriginalColor(); // Reset color to original color
    }

    // Method to react to the damage location by moving toward the target position
    public void ReactToDamage(Vector3 targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition); // Move toward the damaged object’s location
        }
        else
        {
            //Debug.LogWarning("NavMeshAgent not assigned on " + gameObject.name);
        }
    }
}
