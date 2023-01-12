using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    public bool shaking = false;

    [SerializeField]
    private float shakeAmt;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (shaking)
        {
            Vector3 newPos = Random.insideUnitSphere * (Time.deltaTime * shakeAmt);
            newPos.x += transform.position.x;
            newPos.y = transform.position.y;
            newPos.z = transform.position.z;

            transform.position = newPos;
        }
    }

    public static void ShakeMe()
    {
        instance.StartCoroutine(ShakeNow());
    }

    public static IEnumerator ShakeNow()
    {
        Vector3 originalPos = instance.transform.position;

        if (instance.shaking == false)
        {
            instance.shaking = true;
        }

        yield return new WaitForSeconds(2.5f);

        instance.shaking = false;
        instance.transform.position = originalPos;
    }
}
