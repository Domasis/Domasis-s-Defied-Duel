using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class PickupSystem : MonoBehaviour
{
    enum PickupType { Gun, Health, Keycard, Armor } 

    [SerializeField] PickupType type; 

    [SerializeField] GunStats gun;

    [SerializeField] [Range(0.3f, 0.6f)] float hoverHeight;

    [SerializeField] [Range(0, 8.0f)] float animationSpeed;

    Vector3 hoverPos;

    // YF - Health gain added.
    [SerializeField][Range(0, 3)] int healthGain;

    // YF - Armor gain added
    [SerializeField][Range(0, 100)] int armorGain; 

    [SerializeField] AudioClip drinkSound;

    void Start()
    {
        if (type == PickupType.Gun)
        {
            gun.ammoCurrent = gun.ammoMax;
        }

        hoverPos = transform.position;
    }

    // Update is called every frame that the Monobehavior is enabled.
    void Update()
    {
        Hover();
        Rotate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case PickupType.Gun:
                    GameManager.instance.playerScript.getGunStats(gun);
                    Destroy(gameObject);
                    break;

                case PickupType.Health:
                    GameManager.instance.playerScript.Health += healthGain;
                    GameManager.instance.playerScript.updatePlayerUI();
                    GameManager.instance.playerScript.Drink(drinkSound);
                    Destroy(gameObject);
                    break;

                case PickupType.Armor:
                    GameManager.instance.playerScript.AddArmor(armorGain); 
                    Destroy(gameObject);
                    break;

                  
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(GetComponent<TrailRenderer>());
    }

    // Hover functionality by Yoander.

    // Hover causes the pickup to hover around its position in world space, based on a preset animation speed. Place in update to have it hover every frame.
    void Hover()
    {
        /* The first thing we need to do is get the new Y position of our object, we get by taking the Sin of our IN-GAME TIME (Time.time is in-game time), multiplied by our speed.
         * We then multiply that by our hoverHeight (the upper maximum and minimum that our object will hover at), and add that to our static yPos. 
         * Want to invert the direction? Multiply by -1.*/
        float newHoverPos = Mathf.Sin(Time.time * animationSpeed) * hoverHeight + hoverPos.y;

        // Finally, we set our new transform position to use this newHoverPos. We do not update HoverPos specifically because our object position needs to always be relative to its position in world space.
        transform.position = new Vector3(hoverPos.x, newHoverPos, hoverPos.z);

    }

    // Rotation by Yoander.

    // Rotate causes the object to rotate a fixed amount using transform.Rotate. Meant to placed in update so that the object rotates every frame.
    void Rotate()
    {
        /* We call transform.Rotate, multiplying our animation speed by a factor of 8 (to make it noticeable), then multiply it by deltaTime to clamp it to real time.
         * We place it on the y-Axis so that it rotates horizontally and not vertically. */
        transform.Rotate(0, animationSpeed * 8 * Time.deltaTime, 0);
    }
}
