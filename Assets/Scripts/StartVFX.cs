using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfx1;
    [SerializeField] private GameObject vfx2;
    [SerializeField] private GameObject offVfx;
    [SerializeField] private GameObject[] lights;

    [SerializeField] private float onTimer;
    [SerializeField] private float offTimer;

    public void LightOn()
    {
    }

    private IEnumerator LightsOn()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(true);
            yield return new WaitForSeconds(onTimer);
        }
    }

    public void VFX1On()
    {
        vfx1.SetActive(true);
        vfx2.SetActive(true);
        StartCoroutine(LightsOn());
    }

    public void VFXOff()
    {
        vfx1.SetActive(false);
        vfx2.SetActive(false);
        offVfx.SetActive(true);
    }

    public void LightOff()
    {
        StartCoroutine(LightsOff());
    }

    private IEnumerator LightsOff()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
            yield return new WaitForSeconds(offTimer);
        }
    }
}
