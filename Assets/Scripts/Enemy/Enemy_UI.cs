using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing.Tweening;
using UnityEngine.Video;

public class Enemy_UI : MonoBehaviour
{
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private bool isBoss;
    private CanvasGroup canvasGroup;


    [SerializeField] private GameObject attackSlotSet;

    [Header("=== Hp Bar UI ===")]
    [SerializeField] private float delayTime;
    [SerializeField] private GameObject hpBarSet;
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    private Coroutine hpCortoutine;


    [Header("=== Name UI ===")]
    [SerializeField] private GameObject nameSet;
    [SerializeField] private Text nameText;


    // 실제 합을 진행할 때 나오는 간략 설명 UI
    [Header("=== Turn Fight Exchange Summary UI ===")]
    [SerializeField] private GameObject exchangeSummarySet;
    [SerializeField] private Text exchangeSummaryNameText;
    [SerializeField] private Text exchangeSummaryDescriptionText;
    [SerializeField] private Text exchangeSummaryDamageText;

    [SerializeField] private GameObject[] exchangeSummaryAttackCountborder;
    [SerializeField] private GameObject[] exchangeSummaryAttackCount;


    [Header("=== Turn Fight Cutscene ===")]
    [SerializeField] private GameObject phase2VideoSet;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip spawnClip;
    [SerializeField] private VideoClip phase2Clip;
    [SerializeField] private VideoClip dieClip;
    public bool isVideoOn;


    [Header("=== Boss UI ===")]
    [SerializeField] private GameObject boss_SpawnOutlineSet;
    [SerializeField] private Image spawnOutlineImage;
    [SerializeField] private Text spawnBossName;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        hpBarF.maxValue = enemy.hp;
        hpBarB.maxValue = enemy.hp;
        hpBarF.value = enemy.hp;
        hpBarF.value = enemy.hp;
    }

    // Hp UI 호출 부분
    public void HpBar()
    {
        if(hpCortoutine != null)
        {
            StopCoroutine(hpCortoutine);
        }

        hpCortoutine = StartCoroutine(HpBarCall());
    }

    // Hp UI 동작 부분
    private IEnumerator HpBarCall()
    {
        // 앞쪽 Hp
        hpBarF.value = enemy.hp;

        // 최신화 딜레이
        float timer = delayTime;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // 뒷쪽 Hp
        float start = hpBarB.value;
        float end = enemy.hp;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 4f;
            hpBarB.value = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        hpBarF.value = end;
        hpBarB.value = end;
    }


    // 전투 시작 시 UI 활성화 & 전투 종료 시 UI 비활성화
    public void UI_Setting(bool isOn)
    {
        if (isOn)
        {
            nameText.text = enemy.status.EnemyName;
            hpBarF.value = enemy.hp;
            hpBarB.value = enemy.hp;
        }

        nameSet.SetActive(isOn);
        hpBarSet.SetActive(isOn);
    }



    // 턴 UI On / Off
    public void Turn_FightSelect(bool isOn)
    {
        // 공격 슬롯
        attackSlotSet.SetActive(isOn);
    }


    // 합 UI 호출
    public void TurnFight_ExchanageSummary(bool isOn, Attack_Base attackData)
    {
        if (isOn)
        {
            // UI 활성화
            exchangeSummarySet.SetActive(true);
            exchangeSummaryNameText.text = attackData.attackName;
            exchangeSummaryDescriptionText.text = attackData.summaryDescriptionText;
            exchangeSummaryDamageText.text = null;

            // 공격 카운트 활성화
            for (int i = 0; i < exchangeSummaryAttackCount.Length; i++)
            {
                exchangeSummaryAttackCountborder[i].SetActive(false);
                exchangeSummaryAttackCount[i].SetActive(false);
            }
            for (int i = 0; i < attackData.attackCount; i++)
            {
                exchangeSummaryAttackCountborder[i].SetActive(true);
                exchangeSummaryAttackCount[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < exchangeSummaryAttackCount.Length; i++)
            {
                exchangeSummaryAttackCount[i].GetComponent<Image>().color = Color.white;
            }
            exchangeSummaryNameText.text = null;
            exchangeSummaryDamageText.text = null;
            exchangeSummaryDescriptionText.text = null;
            exchangeSummarySet.SetActive(false);
        }
    }

    /// <summary>
    /// 합 도중 현재 공격하는 슬롯 색 변경 -> 기본 : 흰색 / 합 공격 : 빨강 / 파괴 : 검정
    /// </summary>
    /// <param name="attackCount">붉은색으로 표시해야 하는 현재 공격 코인</param>
    public void TurnFight_ExchangeSummary_AttackCount(int attackCount)
    {
        for (int i = 0; i < exchangeSummaryAttackCount.Length; i++)
        {
            if (i < attackCount)
            {
                exchangeSummaryAttackCount[i].GetComponent<Image>().color = Color.black;
            }
            else
            {
                break;
            }
        }

        exchangeSummaryAttackCount[attackCount].GetComponent<Image>().color = Color.red;
    }

    // 공격에서 데미지 표기
    public void TurnFight_ExchangeSummary_Damage(bool isOn, int damage, int curCount, int maxCount, Attack_Base attack)
    {
        if (isOn)
        {
            exchangeSummarySet.SetActive(true);
            exchangeSummaryNameText.text = attack.attackName;
            exchangeSummaryDescriptionText.text = attack.summaryDescriptionText;
            exchangeSummaryDamageText.text = damage.ToString();

            for (int i = 0; i < maxCount; i++)
            {
                exchangeSummaryAttackCount[i].GetComponent<Image>().color = Color.white;
                exchangeSummaryAttackCountborder[i].SetActive(true);
                exchangeSummaryAttackCount[i].SetActive(true);
            }
            exchangeSummaryAttackCount[curCount].GetComponent<Image>().color = Color.white;
            exchangeSummaryAttackCountborder[curCount].SetActive(true);
            exchangeSummaryAttackCount[curCount].SetActive(true);
        }
        else
        {
            exchangeSummarySet.SetActive(false);
        }
    }

    // 합 격돌에서 데미지 표기
    public void TurnFight_ExchangeSummary_AttackDamage(int damage, float time)
    {
        StartCoroutine(TurnFightExchangeSummaryAttackDamageCall(damage, time));
    }

    private IEnumerator TurnFightExchangeSummaryAttackDamageCall(int damage, float time)
    {
        // 지정한 시간동안 어택 카운트 무작위 숫자 표기
        float timer = 0;
        while (timer < time)
        {
            exchangeSummaryDamageText.text = Random.Range(0, 9).ToString();
            timer += Time.deltaTime;
            yield return null;
        }

        // 데미지 표기
        exchangeSummaryDamageText.text = damage.ToString();
    }



    #region 보스 전용

    // 보스 첫 등장 이펙트
    public void Boss_SpawnUI()
    {
        StartCoroutine(BossSpawnUICall());
    }

    private IEnumerator BossSpawnUICall()
    {
        boss_SpawnOutlineSet.SetActive(true);

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            spawnOutlineImage.color= new Color(spawnOutlineImage.color.r, spawnOutlineImage.color.g, spawnOutlineImage.color.b, timer); ;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            spawnBossName.color = new Color(spawnBossName.color.r, spawnBossName.color.g, spawnBossName.color.b, timer);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime * 0.75f;
            spawnOutlineImage.color = new Color(spawnOutlineImage.color.r, spawnOutlineImage.color.g, spawnOutlineImage.color.b, timer);
            spawnBossName.color = new Color(spawnBossName.color.r, spawnBossName.color.g, spawnBossName.color.b, timer);
            yield return null;
        }

        boss_SpawnOutlineSet.SetActive(false);
    }


    // 페이즈 2 컷씬
    public void Phase2Cutscene()
    {
        StartCoroutine(Phase2CutsceneCall());
    }

    private IEnumerator Phase2CutsceneCall()
    {
        isVideoOn = true;

        // 비디오 재생
        videoPlayer.clip = phase2Clip;
        phase2VideoSet.SetActive(true);
        videoPlayer.Play();

        // 비디오 종료 대기
        yield return new WaitForSeconds(1f);
        while(videoPlayer.isPlaying)
        {
            yield return null;
        }
        phase2VideoSet.SetActive(false);

        isVideoOn = false;
    }


    // 스폰 컷씬
    public void SpawnCutscene()
    {
        StartCoroutine(SpawnCutsceneCall());
    }

    private IEnumerator SpawnCutsceneCall()
    {
        isVideoOn = true;

        // 비디오 재생
        videoPlayer.clip = spawnClip;
        phase2VideoSet.SetActive(true);
        videoPlayer.Play();

        // 비디오 종료 대기
        yield return new WaitForSeconds(1f);
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        phase2VideoSet.SetActive(false);

        isVideoOn = false;
    }


    // 사망 컷씬
    public void DieCutscene()
    {
        StartCoroutine(DieCutsceneCall());
    }

    private IEnumerator DieCutsceneCall()
    {
        isVideoOn = true;

        // 비디오 재생
        videoPlayer.clip = dieClip;
        phase2VideoSet.SetActive(true);
        videoPlayer.Play();

        // 비디오 종료 대기
        yield return new WaitForSeconds(1f);
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        phase2VideoSet.SetActive(false);

        isVideoOn = false;
    }
    #endregion
}
