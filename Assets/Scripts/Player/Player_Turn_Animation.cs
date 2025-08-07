using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Turn_Animation : MonoBehaviour
{
    [Header("=== Exchange ===")]
    [SerializeField] private GameObject[] exchangeVFX;


    [Header("=== Underside Effect ===")]
    [SerializeField] private GameObject undersideVFX;


    [Header("=== AD Effect ===")]
    public GameObject guardEffect;
    [SerializeField] private GameObject[] comboVFX;
    [SerializeField] private GameObject[] updownVFX;
    [SerializeField] private GameObject strikeVFX;
    [SerializeField] private GameObject[] tripleVFX;


    [Header("=== AP Effect ===")]
    [SerializeField] private GameObject explosionSlashVFX;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject lightningSlashVFX;
    [SerializeField] private GameObject lightningVFX;
    [SerializeField] private GameObject laserVFX;


    [Header("=== Component ===")]
    private Animator anim;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] laserShotPos;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    // UI 사운드 인덱스
    // 0 : 스킬&슬롯 마우스 오버
    // 1 : 스킬&슬롯 버튼 클릭
    // 2 : 전투 시작 버튼 클릭
    // 3 : 전투시작
    // 4 : 페이즈시작
    // 5 : 전투승리
    // 6 : 전투패배


    // 플레이어 사운드 인덱스
    // 0 : 돌진
    // 1 : 복귀
    // 2 : 피격
    // 3 : 사망
    // 4 : 콤보 1,2,3 / 트리플 1,2 / 필살기1
    // 5 : 익스플로전
    // 6 : 레이저
    // 7 : 라이트닝 - 휘둘기 / 합 승리
    // 8 : 라이트닝 - 폭발
    // 9 : 스트라이크 - 휘둘기 / 트리플3
    // 10: 필살기2

    public void SoundCall(int index)
    {
        Player_Manager.instnace.Turn_Sound_Call(index);
    }

    #region 기본 애니메이션

    // 스폰 애니메이션 종료
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
    }

    // 승리 애니메이션 종료
    public void WinOver()
    {
        anim.SetBool("isWin", false);
    }

    // 사망 애니메이션 종료
    public void DieAnim()
    {
        anim.SetBool("isDie", false);
    }

    // 합 이동 준비 종료
    public void ExchangeMoveReadyAnim()
    {
        anim.SetBool("isExchangeMoveReady", false);
    }

    // 합 종료 시 애니메이션 종료
    public void ExchangeEnd()
    {
        anim.SetBool("isExchangeWin", false);
        anim.SetBool("isExchangeLose", false);
    }

    // 합 종료 후 복귀 백스텝 종료
    public void BackstepEnd()
    {
        anim.SetBool("isBackStep", false);
    }
    #endregion


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {
            LaserEffect();   
        }
    }

    #region 공격
    public void AttackCall()
    {
        // 미리 데이터가 들어가 있어야만 제대로 동작함!
        Player_Manager.instnace.player_Turn.Attack_Call();
    }
    public void AttackComboCall()
    {
        Player_Manager.instnace.player_Turn.AttackCombo_Call();
    }


    // 공격 전 타겟 위치로 이동
    public void AttackMove()
    {
        Player_Manager.instnace.player_Turn.Turn_AttackMove();
    }


    // 공격이 종료할때

    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    public void UndersideOver() // 아마 미사용
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isUndersideAttack", false);
    }

    public void GuardOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isGuard", false);
    }

    public void ComboAttackOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isComboAttack", false);
    }

    public void UpDownAttackOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isUpDownAttack", false);
    }

    public void StrikeAttackOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isStrikeAttack", false);
    }

    public void TripleWhirlwindAttackOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isStrikeAttack", false);
    }


    public void ExposionOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isExplosion", false);
    }

    public void LaserOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isLaser", false);
    }

    public void LightningOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isLightning", false);
    }
    #endregion


    #region 이펙트
    public void ExchangewinAEffect(int index)
    {
        SoundCall(7);

        if (exchangeVFX[index].activeSelf)
            exchangeVFX[index].SetActive(false);

        exchangeVFX[index].SetActive(true);
    }

    public void UndersideEffect()
    {
        undersideVFX.SetActive(true);
        Player_Manager.instnace.player_Turn.Attack_UndersideLastCall();
    }

    public void ComboEffect(int count)
    {
        comboVFX[count].SetActive(true);
    }

    public void UpDownEffect(int count)
    {
        // 카메라 변경
        Camera_Manager.instance.Camera_Setting(count == 0 ? Camera_Manager.CameraType.UpDownA : Camera_Manager.CameraType.UpDownB, 0.3f);

        updownVFX[count].SetActive(true);
    }

    public void StrikeEffect()
    {
        strikeVFX.SetActive(true);
    }

    public void TripleEffect(int count)
    {
        tripleVFX[count].SetActive(true);
    }


    public void ExposionEffect()
    {
        // 카메라 연출
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.ExplosionB, 0.2f);

        SoundCall(5);

        explosionSlashVFX.SetActive(true);
        Player_Manager.instnace.player_Turn.AttackAOE_Call(explosionVFX);
    }

    public void LaserEffect()
    {
        // 카메라 연출
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.LaserB, 0.5f);

        for (int i = 0; i < laserShotPos.Length; i++)
        {
            GameObject laser = Instantiate(laserVFX, laserShotPos[i].position, Quaternion.identity);
            Vector3 lookDir = (Player_Manager.instnace.player_Turn.attackTarget.transform.position - laser.transform.position).normalized;
            laser.transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    public void LightningEffect()
    {
        // 카메라 연출
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.LightningB, 0.2f);

        SoundCall(8);

        lightningSlashVFX.SetActive(true);
        Player_Manager.instnace.player_Turn.AttackAOE_Call(lightningVFX);
    }
    #endregion
}
