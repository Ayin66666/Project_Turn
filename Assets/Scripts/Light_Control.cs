using System.Collections;
using UnityEngine;

public class Light_Control : MonoBehaviour
{
    private float timer;
    private Light _light;
    private Vector2 random_Cycle;
    [SerializeField] private int intensity_Amount;
    [SerializeField] private float intensity_Cycle;
    [SerializeField] float delayTime;

    Coroutine myCoroutine;

    void Start()
    {
        timer = 0f;
        intensity_Amount = 5;
        intensity_Cycle = 0.01f;
        random_Cycle = new Vector2(2, 5);
        delayTime = Random.Range(random_Cycle.x, random_Cycle.y);
        _light = GetComponent<Light>();
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if(timer > delayTime)
        {
            if(myCoroutine == null)
            {
                myCoroutine = StartCoroutine(Blink());
            }
        }
    }

    IEnumerator Blink()
    {
        while(_light.intensity > 0)
        {
            _light.intensity -= intensity_Amount;
            yield return new WaitForSeconds(intensity_Cycle);
        }

        yield return new WaitForSeconds(0.3f);

        while(_light.intensity < 50)
        {
            _light.intensity += intensity_Amount * 2;
            yield return new WaitForSeconds(intensity_Cycle);
        }
        while (_light.intensity > 0)
        {
            _light.intensity -= intensity_Amount * 2;
            yield return new WaitForSeconds(intensity_Cycle);
        }
        while (_light.intensity < 50)
        {
            _light.intensity += intensity_Amount * 2;
            yield return new WaitForSeconds(intensity_Cycle);
        }

        timer = 0f;
        delayTime = Random.Range(random_Cycle.x, random_Cycle.y);
        myCoroutine = null;
    }
}
