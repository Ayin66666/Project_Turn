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


    // UI ���� �ε���
    // 0 : ��ų&���� ���콺 ����
    // 1 : ��ų&���� ��ư Ŭ��
    // 2 : ���� ���� ��ư Ŭ��
    // 3 : ��������
    // 4 : ���������
    // 5 : �����¸�
    // 6 : �����й�


    // �÷��̾� ���� �ε���
    // 0 : ����
    // 1 : ����
    // 2 : �ǰ�
    // 3 : ���
    // 4 : �޺� 1,2,3 / Ʈ���� 1,2 / �ʻ��1
    // 5 : �ͽ��÷���
    // 6 : ������
    // 7 : ����Ʈ�� - �ֵѱ� / �� �¸�
    // 8 : ����Ʈ�� - ����
    // 9 : ��Ʈ����ũ - �ֵѱ� / Ʈ����3
    // 10: �ʻ��2

    public void SoundCall(int index)
    {
        Player_Manager.instnace.Turn_Sound_Call(index);
    }

    #region �⺻ �ִϸ��̼�

    // ���� �ִϸ��̼� ����
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
    }

    // �¸� �ִϸ��̼� ����
    public void WinOver()
    {
        anim.SetBool("isWin", false);
    }

    // ��� �ִϸ��̼� ����
    public void DieAnim()
    {
        anim.SetBool("isDie", false);
    }

    // �� �̵� �غ� ����
    public void ExchangeMoveReadyAnim()
    {
        anim.SetBool("isExchangeMoveReady", false);
    }

    // �� ���� �� �ִϸ��̼� ����
    public void ExchangeEnd()
    {
        anim.SetBool("isExchangeWin", false);
        anim.SetBool("isExchangeLose", false);
    }

    // �� ���� �� ���� �齺�� ����
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

    #region ����
    public void AttackCall()
    {
        // �̸� �����Ͱ� �� �־�߸� ����� ������!
        Player_Manager.instnace.player_Turn.Attack_Call();
    }
    public void AttackComboCall()
    {
        Player_Manager.instnace.player_Turn.AttackCombo_Call();
    }


    // ���� �� Ÿ�� ��ġ�� �̵�
    public void AttackMove()
    {
        Player_Manager.instnace.player_Turn.Turn_AttackMove();
    }


    // ������ �����Ҷ�

    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    public void UndersideOver() // �Ƹ� �̻��
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


    #region ����Ʈ
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
        // ī�޶� ����
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
        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.ExplosionB, 0.2f);

        SoundCall(5);

        explosionSlashVFX.SetActive(true);
        Player_Manager.instnace.player_Turn.AttackAOE_Call(explosionVFX);
    }

    public void LaserEffect()
    {
        // ī�޶� ����
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
        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.LightningB, 0.2f);

        SoundCall(8);

        lightningSlashVFX.SetActive(true);
        Player_Manager.instnace.player_Turn.AttackAOE_Call(lightningVFX);
    }
    #endregion
}
