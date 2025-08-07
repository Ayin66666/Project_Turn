using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Boss : Enemy_Base
{
    private enum Pattern_Phase1 { PatternA1, PatternB1, PatternC1, PatternD1 }
    private enum Pattern_Phase2 { PatternA2, PatternB2, PatternC2, PatternD2 }


    [Header("=== Boss Setting ===")]
    [SerializeField] private Pattern_Phase1 pattern_Phase1;
    [SerializeField] private Pattern_Phase2 pattern_Phase2;


    [Header("=== VFX ===")]
    public GameObject wingVFX;
    public GameObject teleportVFX;
    public GameObject chargeVFX;


    [Header("=== Phase 2 ===")]

    // Effect
    [SerializeField] private GameObject phase2VFX;

    // Boss
    [SerializeField] private SkinnedMeshRenderer swordRenderer;
    [SerializeField] private SkinnedMeshRenderer hair;
    [SerializeField] private SkinnedMeshRenderer wing;
    [SerializeField] private SkinnedMeshRenderer head;
    [SerializeField] private SkinnedMeshRenderer[] dress;
    [SerializeField] private Material hairMat;
    [SerializeField] private Material dressMat;
    [SerializeField] private Material swordMat;
    [SerializeField] private Material wingMat;
    [SerializeField] private Material headMat;

    // Map
    [SerializeField] private MeshRenderer mapRenderer;
    [SerializeField] private Material phase2MapMaterial;
    [SerializeField] private Material skyBox;


    // ��ų 6�� �ʻ�� 1��

    // 3�� ����, �������, ����, ��������- 1 ������
    // 4�� ����, �÷����� - 2 ������
    // ��Ʈ����ũ - �ʻ��


    // ������ 1

    // 3�� - ������� - ���� - �������
    // 3�� - ���� - ���� ���� - �������
    // ������� - ������� - ����- ���� ����
    // 3�� - ���� - ������� - ���� ����


    // ������ 2

    // 4�� - ������� - �÷����� - �������
    // 4�� - ���� - ���� ���� - �������
    // 4�� - ������� - ������� - ���� ����
    // 4�� - 4�� - ������� - ���� ����


    // ���� ����
    // 4�� - ���� - ������� - ��Ʈ����ũ


    // ���� ����
    // 0 : �� �¸� 
    // 1 : �ǰ�
    // 2 : 3������ 1
    // 3 : 3������ 2
    // 4 : 3������ 3 , ���� 2
    // 5 : �������
    // 6 : ���� 1
    // 7 : ��������
    // 8 : 4������ 4
    // 9 : �÷�����
    // 10 : ��Ʈ����ũ
    // 11 : ����
    // 12 : ���

    delegate void PATTERNFUNC();
    PATTERNFUNC[] PatternFuncsA;
    PATTERNFUNC[] PatternFuncsB;


    [Header("=== Phase Attack Value ===")]
    private int[,,] valueList = new int[2,4,4]
    {
        { 
           { 0, 2, 1, 2 },
           { 0, 1, 3, 2 },
           { 2, 2, 1, 3 }, 
           { 0, 1, 2, 3 }
        },
        { 
           { 4, 2, 2, 3 }, 
           { 4, 1, 3, 2 },
           { 4, 2, 5, 3 },
           { 4, 5, 2, 3 }
        },
    };


    [Header("===Charged Attack Setting===")]
    [SerializeField] private List<Attack_Pattern> chargedPattern;
    [SerializeField] private int chargedAttackCount;
    private int curChargedAttackCount;


    [Header("=== Attack Pos Setting ===")]
    [SerializeField] private Transform[] explosionStrikePosA;
    [SerializeField] private Transform[] explosionStrikePosB;
    [SerializeField] private Transform[] explosionStrikePosC;
    [SerializeField] private Transform[] strikeExplosionPos;
    [SerializeField] private Transform[] upwardExplosionPos;


    [Header("=== �ű� ������ ===")]
    [SerializeField] private Transform shotPos;
    public Transform comboPos;
    public Transform[] jumpSlashPos;
    public Transform[] qudraSlashPos;
    public Transform backstepPos;


    [Header("=== VFX ===")]
    [SerializeField] private GameObject explosionStrikeVFX;
    [SerializeField] private GameObject stirkeExplosionVFX;
    [SerializeField] private GameObject upwardExplosionVFX;
    [SerializeField] private GameObject jumpSlashSwordAuraVFX;
    [SerializeField] private GameObject quadrSwordAuraVFX;



    private void Awake()
    {
        // �������ͽ� ����
        Status_Setting();

        // ��������Ʈ ���� ����
        PatternFuncsA = new PATTERNFUNC[4] { PatternA1, PatternB1, PatternC1, PatternD1 };
        PatternFuncsB = new PATTERNFUNC[4] { PatternA2, PatternB2, PatternC2, PatternD2 };
    }


    public override void Turn_AttackSetting(GameObject obj)
    {
        // ���� �ӵ� �Ҵ�
        Slot_SpeedSetting();

        // UI On
        enemyUI.UI_Setting(true);

        // ���� ���� �������� üũ
        if (canAction)
        {
            // Ư�� ���� ���� üũ
            if (curChargedAttackCount >= chargedAttackCount)
            {
                ChargePattern();
            }
            else
            {
                curChargedAttackCount++;

                // ����� ���� ���� ����
                if (isPhase2)
                {
                    Attack_Weight_Phase2();
                }
                else
                {
                    Attack_Weight_Phase1();
                }
            }
        }
        else
        {
            // �ൿ �Ҵ� �� -> ��� ���� ������ ���� ä���
            for (int i = 0; i < attack_Slots.Count; i++)
            {

            }
        }
    }


    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    /// <param name="phaseIndex">1,2������ üũ ��</param>
    /// <param name="patternIndex">n�������� n2 ��° ���� ���� üũ</param>
    void SetAttackSlot(int phaseIndex, int patternIndex)
    {
        for (int i = 0; i < attack_Slots.Count; i++)
        {
            attack_Slots[i].Attack_Setting(attacklist[valueList[phaseIndex, patternIndex, i]]);
            attack_Slots[i].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
            turnManager.Exchange_Setting_Enemy(attack_Slots[i], attack_Slots[i].targetSlot);
        }
    }

    #region Attack Pattern - Phase 1

    private void Attack_Weight_Phase1()
    {
        // ����ġ ���� ���
        int total = 0;
        foreach (var attack in patternList)
        {
            total += attack.Weight;
        }

        int ran = Random.Range(0, total);
        foreach (var attack in patternList)
        {
            if (ran <= attack.Weight)
            {
                pattern_Phase1 = (Pattern_Phase1)attack.enumValue;
                PatternFuncsA[attack.enumValue]();
                Debug.Log("���� ���� : " + pattern_Phase1);
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }
    }

    // 3�� - ������� - ���� - x
    private void PatternA1()
    {
        SetAttackSlot(0, 0);
    }

    // 3�� - ���� - ���� ���� - x
    private void PatternB1()
    {
        SetAttackSlot(0, 1);
    }

    // ������� - ������� - ���� ���� - x
    private void PatternC1()
    {
        SetAttackSlot(0, 2);
    }

    // 3�� - 3�� - ������� - ���� ����
    private void PatternD1()
    {
        SetAttackSlot(0, 3);
    }
    #endregion


    #region Attack Pattern - Phase 2
    private void Attack_Weight_Phase2()
    {
        // ����ġ ���� ���
        int total = 0;
        foreach (var attack in (patternList_Phase2))
        {
            total += attack.Weight;
        }

        int ran = Random.Range(0, total);
        foreach (var attack in patternList)
        {
            if (ran <= attack.Weight)
            {
                pattern_Phase2 = (Pattern_Phase2)attack.enumValue;
                PatternFuncsB[attack.enumValue]();
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }
    }

    // 4�� - ������� - �÷����� - �������
    private void PatternA2()
    {
        SetAttackSlot(1, 0);
    }

    // 4�� - ���� - ���� ���� - �������
    private void PatternB2()
    {
        SetAttackSlot(1, 1);
    }

    // 4�� - ������� - ������� - ���� ����
    private void PatternC2()
    {
        SetAttackSlot(1, 2);
    }

    // 4�� - 4�� - ������� - ���� ����
    private void PatternD2()
    {
        SetAttackSlot(1, 3);
    }
    #endregion


    #region Attack Pattern - Charged

    // 4�� - ������ - ������� - ��Ʈ����ũ
    private void ChargePattern()
    {
        curChargedAttackCount = 0;

        // 1��
        attack_Slots[0].Attack_Setting(attacklist[4]);
        attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);

        // 2��
        attack_Slots[1].Attack_Setting(attacklist[1]);
        attack_Slots[1].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[1], attack_Slots[1].targetSlot);

        // 3��
        attack_Slots[2].Attack_Setting(attacklist[5]);
        attack_Slots[2].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[2], attack_Slots[2].targetSlot);

        // 4��
        attack_Slots[3].Attack_Setting(attacklist[6]);
        attack_Slots[3].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[3], attack_Slots[3].targetSlot);
    }


    // �ʻ�� �߰� ���� ���
    public void ExplosionStrike()
    {
        StartCoroutine(ExplosionStrikeCall());
    }


    private IEnumerator ExplosionStrikeCall()
    {
        // 1�� ����
        //Player_Manager.instnace.Take_Damage(30, Player_Manager.DamageType.Magical, false);
        for (int i2 = 0; i2 < explosionStrikePosA.Length; i2++)
        {
            GameObject obj = Instantiate(explosionStrikeVFX, explosionStrikePosA[i2].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.25f);

        // 2�� ����
        //Player_Manager.instnace.Take_Damage(30, Player_Manager.DamageType.Magical, false);
        for (int i2 = 0; i2 < explosionStrikePosB.Length; i2++)
        {
            GameObject obj = Instantiate(explosionStrikeVFX, explosionStrikePosB[i2].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.25f);

        // 3�� ����
        //Player_Manager.instnace.Take_Damage(30, Player_Manager.DamageType.Magical, false);
        for (int i2 = 0; i2 < explosionStrikePosC.Length; i2++)
        {
            GameObject obj = Instantiate(explosionStrikeVFX, explosionStrikePosC[i2].position, Quaternion.identity);
        }
    }
    #endregion


    public void StrikeExplosion()
    {
        for (int i = 0; i < strikeExplosionPos.Length; i++)
        {
            // ����
            SoundCall(8);

            Instantiate(stirkeExplosionVFX, strikeExplosionPos[i].position, Quaternion.identity);
        }
    }

    public void UpwardExplosion()
    {
        StartCoroutine(UpwardExplosionCall());
    }

    private IEnumerator UpwardExplosionCall()
    {
        for (int i = 0; i < upwardExplosionPos.Length; i++)
        {
            // ����
            SoundCall(9);

            Instantiate(upwardExplosionVFX, upwardExplosionPos[i].position, Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void JumpSlashSwordAura()
    {
        (int damage, bool cir) = DamageCal(myAttack, 0);

        GameObject obj = Instantiate(jumpSlashSwordAuraVFX, shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.ShotType.Normal, Enemy_Bullet.DamageType.Magical, cir, damage, 20, Player_Manager.instnace.player_Turn.gameObject);
    }

    public void QuadrSwordAura(int attackCount)
    {
        (int damage, bool cir) = DamageCal(myAttack, attackCount);
        GameObject obj = Instantiate(quadrSwordAuraVFX, shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.ShotType.Normal, Enemy_Bullet.DamageType.Magical, cir, damage, 20, Player_Manager.instnace.player_Turn.gameObject);
    }

    public override void Phase2()
    {
        StartCoroutine(Phase2Call());
    }

    private IEnumerator Phase2Call()
    {
        isPhase2 = true;
        isCutscene = true;

        // ������ ��ȯ �ƾ� On
        enemyUI.Phase2Cutscene();

        // �� ����
        MapChange();

        // ���� ����
        BossChange();

        // ���� ����
        Stage_Manager.instance.BGM_OnOff(false);

        // ���� ����
        Stage_Manager.instance.BGM_Setting(1);

        // ���� ���� ���
        while (enemyUI.isVideoOn)
        {
            yield return null;
        }
        isCutscene = false;

        // ����
        SoundCall(10);

        // ����Ʈ
        phase2VFX.SetActive(true);

        // ���̾�α�
        Dialog_Manager.instance.Dialog(3);

        // ������ 2 ��ȯ �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isPhase2", true);
        while(anim.GetBool("isPhase2"))
        {
            yield return null;
        }
    }

    public void MapChange()
    {
        if(mapRenderer != null)
        {
            RenderSettings.skybox = skyBox;
            mapRenderer.material = phase2MapMaterial;
        }
    }

    private void BossChange()
    {
        swordRenderer.material = swordMat;
        head.material = headMat;
        hair.material = hairMat;
        dress[0].material = dressMat;
        dress[1].material = dressMat;
        wing.material = wingMat;
    }


    public override void HitAnim()
    {
        SoundCall(1);
        anim.SetTrigger("Hit");
    }


    public override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        isSpawn = true;
        isCutscene = true;

        // ���� ���
        enemyUI.SpawnCutscene();

        yield return new WaitForSeconds(1f);
        while(enemyUI.isVideoOn)
        {
            yield return null;
        }

        // ���� UI ����Ʈ
        enemyUI.Boss_SpawnUI();

        // ����
        Stage_Manager.instance.BGM_Setting(0);

        // ���� ���̾�α� üũ
        if (haveStartDialog)
        {
            Dialog_Manager.instance.Dialog(1);
        }

        // ���̾�α� üũ
        if (haveDialog)
        {
            dialogCheckCoroutine = StartCoroutine(Dialog_Check());
        }

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);

        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        isSpawn = false;
        isCutscene = false;
    }


    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        isDie = true;
        isCutscene = true;

        // ����
        SoundCall(2);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);

        while (anim.GetBool("isDie"))
        {
            yield return null;
        }

        // ���� ���
        enemyUI.DieCutscene();
        while (enemyUI.isVideoOn)
        {
            yield return null;
        }

        isCutscene = false;
    }
}
