using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//CODY JANDES CREATED THIS SCRIPT 
public class enemyAI : MonoBehaviour, TakesDamage
{

    //CODY JANDES CREATED THIS SCRIPT 

    [SerializeField] Renderer model;

    [SerializeField] int enemyHP;

    Color colorOriginal;

    Color damageOne;

    //Enemy HealthBar Practice TRIAL
    private float healthGoAwayTimes = 3f;
    private float timer;

    [SerializeField] Slider healthBarSlider;
    [SerializeField] Canvas healthBarCanvas;

    //Get and Set originalColor
    public Color GetOriginalColor()
    {
        return colorOriginal;
    }
    public void SetOriginalColor(Color newColor)
    {
        colorOriginal = newColor;
    }

    //Get and Set damageOne
    public Color GetDamageOneColor()
    {
        return damageOne;
    }
    public void SetDamageOneColor(Color newColor)
    {
        damageOne = newColor;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetOriginalColor(model.material.color);
        SetDamageOneColor(Color.red);
        GameManager.instance.updateGameGoal(1);
        
        //TRIAL
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = enemyHP; //make sure to adjust in unity when necesarry
            healthBarSlider.value = enemyHP;
            healthBarCanvas.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TRIAL
        if (healthBarCanvas.enabled)
        {
            timer -=Time.deltaTime;
            if(timer <= 0)
            {
                healthBarCanvas.enabled=false;
            }
        }
    }

    public void TakeSomeDamage(int amount)
    {
        //take away HP from enemy HP
        enemyHP -= amount;

        //TRIAL
        if (healthBarSlider != null && healthBarCanvas != null)
        {
            healthBarCanvas.enabled = true;
            healthBarSlider.value = enemyHP;
            timer = healthGoAwayTimes;
        }

        //Start Coroutine to call flash hit
        StartCoroutine(flashHit());

        if(enemyHP <= 0)
        {
            //Update instanc when enemy dies
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    //create timer for flashing when enemy is hit
    IEnumerator flashHit()
    {
        model.material.color = GetDamageOneColor(); //get material to damage color
        yield return new WaitForSeconds(0.1f); //timer 
        model.material.color = GetOriginalColor(); //reset color to original color
    }
}
