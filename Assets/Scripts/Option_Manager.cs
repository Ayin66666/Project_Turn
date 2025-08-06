using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option_Manager : MonoBehaviour
{
    public static Option_Manager instance;

    [Header("=== Sound Setting ===")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider master_slider;
    [SerializeField] private Slider bgm_slider;
    [SerializeField] private Slider sfx_slider;


    [Header("=== FPS Setting ===")]
    [SerializeField] private Dropdown fpsDropDown;


    // (���, �κ�, ��ų, �ɼ�) â
    [Header("=== Option UI ===")]
    public GameObject menuSet;
    public GameObject optionSet;
    public GameObject exitSet;
    [SerializeField] private Toggle fpsToggle;
    private float deltaTime = 0f;
    [SerializeField] private GameObject option_BackGround;



    // ���� �ΰ� �Ұ����� �ɼ�â ������ Ȯ�� & �ɼ�â���� ������ ��ư �������� Ȯ��
    public bool isOptionOn;
    public bool isExitOn;
    public bool isFpsOn;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        master_slider.onValueChanged.AddListener(SetMasterVolume);
        bgm_slider.onValueChanged.AddListener(SetBgmVolume);
        sfx_slider.onValueChanged.AddListener(SetSfxVolume);

        fpsDropDown.value = 1;
    }

    private void Update()
    {
        //FPS ǥ�� �� ������
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    public void On_Menu()
    {
        menuSet.SetActive(true);

        if(SceneManager.GetActiveScene().name == "Start_Scene")
        {
            option_BackGround.SetActive(true);
        }
        else
        {
            option_BackGround.SetActive(false);
            //optionSet.SetActive(false);
            exitSet.SetActive(false);
        }
    }

    public void Off_Menu()
    {
        menuSet.SetActive(false);
    }

    public void Menu_Click_Exit()
    {
        if(SceneManager.GetActiveScene().name == "Start_Scene")
        {
            Off_Menu();
            Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
        }
        else
        {
            isExitOn = true;
            exitSet.SetActive(true);
            Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
        }
    }

    public void Menu_Click_ExitOff()
    {
        isExitOn = false;
        exitSet.SetActive(false);
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void Menu_Click_Option()
    {
        //�ɼ�â Ű��
        OnOff_Option(true);

        if(Player_UI.instance != null)
        {
            //���â ����
            Player_UI.instance.OnOff_Weapon_Item(false);
            //��ųâ ����
            Player_UI.instance.OnOff_Skill(false);
            //Ʃ��â ����
            Player_UI.instance.OnOff_Tutorial(false);
        }

        // Ŭ�� ����
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void OnOff_Option(bool isOn)
    {
        isOptionOn = isOn;
        optionSet.SetActive(isOn);
    }


    public void Click_Start()
    {
        Scene_Loading.LoadScene("Test_Scene");
    }

    public void Exit_Game()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    #region FPS Setting

    public void FpsOn()
    {
        if (fpsToggle.isOn)
        {
            isFpsOn = true;
        }
        else
        {
            isFpsOn = false;
        }
    }

    private void OnGUI()
    {
        if (isFpsOn && SceneManager.GetActiveScene().name != "Loading_Scene")
        {
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(30, 30, Screen.width, Screen.height);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 25;
            style.normal.textColor = Color.red;

            float ms = deltaTime * 1000f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

            GUI.Label(rect, text, style);
        }
        else
        {
            return;
        }
    }

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
