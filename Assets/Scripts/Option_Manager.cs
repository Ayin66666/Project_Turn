using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Option_Manager : MonoBehaviour
{
    [Header("=== Sound Setting ===")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider master_slider;
    [SerializeField] private Slider bgm_slider;
    [SerializeField] private Slider sfx_slider;


    [Header("=== FPS Setting ===")]
    [SerializeField] private Dropdown fpsDropDown;

    
    void Awake()
    {
        master_slider.onValueChanged.AddListener(SetMasterVolume);
        bgm_slider.onValueChanged.AddListener(SetBgmVolume);
        sfx_slider.onValueChanged.AddListener(SetSfxVolume);

        fpsDropDown.value = 1;
    }


    #region FPS Setting
    public void SetFPS(int value)
    {
        if (fpsDropDown.value == 0)
        {
            value = 30;
        }
        else if (fpsDropDown.value == 1)
        {
            value = 60;
        }
        else if (fpsDropDown.value == 2)
        {
            value = 90;
        }
        else if (fpsDropDown.value == 3)
        {
            value = 120;
        }
        else if (fpsDropDown.value == 4)
        {
            value = 144;
        }
        else if (fpsDropDown.value == 5)
        {
            value = 165;
        }

        Application.targetFrameRate = value;
    }
    #endregion


    #region Sound Setting
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
    #endregion
}
