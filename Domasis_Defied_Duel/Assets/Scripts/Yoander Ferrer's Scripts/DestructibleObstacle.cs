using System.Collections;
using UnityEngine;

public class DestructibleObstacle : MonoBehaviour, TakesDamage
{

    [SerializeField] int hp;

    [SerializeField] Renderer model;

    Color origColor;

    [SerializeField] Color dmgColor;

    public int HP { get => hp; set => hp = value; }
    public Color OrigColor { get => origColor; set => origColor = value; }
    public Color DmgColor { get => dmgColor; set => dmgColor = value; }
    public Renderer Model { get => model; set => model = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        origColor = Model.material.color; 

    }

    public void TakeSomeDamage(int amt)
    {

        HP -= amt;

        StartCoroutine(FlashDmg());

        if (HP <= 0)
        {
            Destroy(gameObject);
        }

    }

    IEnumerator FlashDmg()
    {

        Model.material.color = DmgColor;

        yield return new WaitForSeconds(0.2f);

        Model.material.color = origColor;

    }
}
