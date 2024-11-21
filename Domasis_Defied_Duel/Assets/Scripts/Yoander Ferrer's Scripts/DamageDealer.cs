using UnityEngine;

// Created by Yoander Ferrer
public class DamageDealer : MonoBehaviour
{
   // Enumerator that tracks the types of damage that can be dealt.
    enum DamageType { bullet, stationary };

    // Editor exposed DamageType instance that tracks the damage source.
    [SerializeField] DamageType type;

    // Editor exposed Rigidbody that is used for collision purposes.
    [SerializeField] Rigidbody rb;

    // Editor exposed variable that stores the damage dealt by our projectile/stationary object.
    [SerializeField] int damage;

    // Editor exposed variable that tracks the projectile speed of our bullet.
    [SerializeField] float speed;

    // Editor exposed variable that tracks the life of the projectile before it is destroyed.
    [SerializeField] float timeTilDestruction;

    // Properties for each of the above member variables.
    private DamageType Type { get => type; set => type = value; }
    public Rigidbody Rb { get => rb; set => rb = value; }
    public int Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public float TimeTilDestruction { get => timeTilDestruction; set => timeTilDestruction = value; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
        // If the inheriting object is a bullet:
        if (type == DamageType.bullet)
        {
            // We give it a forward linear velocity
            Rb.linearVelocity = transform.forward * speed;

            // After a specified amount of time has passed, we destroy the object.
            Destroy(gameObject, TimeTilDestruction);

        }

    }

    // Private method called when the player enters the sphere collision radius.
    private void OnTriggerEnter(Collider other)
    {
        // If the object that collided with it is also a trigger, or is a player, we exit early to avoid triggers setting off other triggers.
        if (other.isTrigger || other.CompareTag("Enemy"))
        {
            return;
        }

        // We create an IDamage instance based on the collider's IDamage component.
        TakesDamage dmg = other.GetComponent<TakesDamage>();

        Debug.Log(other.gameObject);

        // If this was successful, we call that component's TakeDamage method.
        dmg?.TakeSomeDamage(damage);


        // Finally, if the object that triggered this was a bullet, we destroy the game object.
        if (type == DamageType.bullet)
        {
            Destroy(gameObject);
        }

    }
}
