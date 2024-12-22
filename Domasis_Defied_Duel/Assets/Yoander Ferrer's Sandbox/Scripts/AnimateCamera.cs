using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AnimateCamera : MonoBehaviour
{

    [SerializeField] [Range(0.15f, 1f)] float shakeIntensity;

    [SerializeField] [Range(0.15f, 0.45f)] float duration;

    [SerializeField] GameObject CamPos;

    public bool isShaking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator ShakeCamera(Camera cam, Renderer model)
    {
            model.enabled = false;

            isShaking = true;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * shakeIntensity;

                cam.transform.position += new Vector3(x, 0, 0);

                elapsed += Time.deltaTime;

                yield return null;
            }

            cam.transform.position = CamPos.transform.position;
            model.enabled = true;
            isShaking = false;
    }

    public void stopShake(Camera cam, Renderer model)
    {
        isShaking = false;
        cam.transform.position = CamPos.transform.position;
        model.enabled = true;
    }
}
