using System.Collections;
using UnityEngine;

public class AnimateCamera : MonoBehaviour
{
    [SerializeField] Camera mainCam;

    float shakeIntensity;

    float duration;

    Vector3 origCamPos;

    public bool isShaking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        
    }

    public IEnumerator ShakeCamera()
    {
        isShaking = true;
        
        origCamPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            transform.position = new Vector3(x, y, -10f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = origCamPos;
        isShaking = false;
    }
}
