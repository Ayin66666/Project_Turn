using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Damage_UI : MonoBehaviour
{
    [Header("---Damage UI Setting---")]
    [SerializeField] private float normalDamagetSize;
    [SerializeField] private float critcalDamagetSize;
    [SerializeField] private float activateTime;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color critcalColor;
    public enum DamageType { Normal, Critical }
    private Coroutine curCoroutine;


    [Header("---Component---")]
    [SerializeField] private Text damageText;
    [SerializeField] private Font normalFont;
    [SerializeField] private Font critcalFont;


    [Header("---Move Pos---")]
    [SerializeField] private Transform[] movePos;


    private void Update()
    {
        LookAt();
    }

    // UI 가 무조건 플레이어를 바라보도록 동작
    public void LookAt()
    {

        Vector3 lookDir = (Camera_Manager.instance.mainCam.transform.position - transform.position).normalized;
        lookDir.y = 0;

        transform.rotation = Quaternion.LookRotation(-lookDir);
    }


    public void DamageUI_Setting(DamageType type, int damage)
    {
        if (curCoroutine != null)
        {
            return;
        }

        damageText.text = damage.ToString();

        switch (type)
        {
            case DamageType.Normal:
                damageText.font = normalFont;
                damageText.color = normalColor;
                break;

            case DamageType.Critical:
                damageText.font = critcalFont;
                damageText.color = critcalColor;
                break;
        }

        curCoroutine = StartCoroutine(DamageUI());
    }


    private IEnumerator DamageUI()
    {
        // Move
        Vector3 startPos = movePos[0].position;
        Vector3 endPos = movePos[1].position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * activateTime;
            damageText.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.15f);

        // Fade
        float a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime * 1.25f;
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, a);
            yield return null;
        }

        Destroy(gameObject);
    }
}
