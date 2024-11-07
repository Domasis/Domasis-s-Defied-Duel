using System.Collections;
using UnityEngine;
//Sheldon Sharp/Cody
public class brickBreaker : MonoBehaviour, IDamage1
{


    [SerializeField] Renderer model;

    [SerializeField] int enemyHP;

    Color colorOriginal;

    Color damageOne;

    //Get & Set originalColor
    public Color GetOriginalColor()
    {
        return colorOriginal;
    }
    public void SetOriginalColor(Color newColor)
    {
        colorOriginal = newColor;
    }

    //Get & Set damageOne
    public Color GetDamageOneColor()
    {
        return damageOne;
    }
    public void SetDamageOneColor(Color newColor)
    {
        damageOne = newColor;
    }

   
    void Start()
    {
        SetOriginalColor(model.material.color);
        SetDamageOneColor(Color.red);

    }

 
    void Update()
    {

    }

    public void ITakesDamage(int amount)
    {
        //take HP from enemy 
        enemyHP -= amount;

        //call flash hit
        StartCoroutine(flashHit());

        if (enemyHP <= 0)
        {
            Destroy(gameObject);
        }
    }

   
    IEnumerator flashHit()
    {
        model.material.color = GetDamageOneColor(); 
        yield return new WaitForSeconds(0.1f); 
        model.material.color = GetOriginalColor(); //reset color to original color
    }
}