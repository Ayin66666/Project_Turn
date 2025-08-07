using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class Player_Turn : MonoBehaviour
{
    [Header("=== State ===")]
    public bool isSpawnAnim;
    public bool isSelect;
    public bool isExchangeMove;
    public bool isRecoilMove;
    public bool isExchangeTargetSelect;
    public bool isEndAnim;
    public bool isLook;
    [SerializeField]private float movement; // �ִϸ��̼�

    public enum ExchangeResuit { Win, Draw, Lose }
    public enum EndType { Win, Lose }

    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attackSlot;
    [SerializeField] private Attack_Base undersideData;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] exchange_RecoilPos;
    public Transform exchangePos;


    [Header("=== LookAt Setting ===")]
    public GameObject forwardTarget; // ���͸� �ֽ����� ���� �� �ٶ� ���
    [SerializeField] private GameObject looktarget; // �ֽ��� ���


    [Header("=== Component ===")]
    public Animator anim = null;
    public Player_Turn_Animation turnAnim;
    private Coroutine curCoroutine;
    private Coroutine selectAnimCoroutine;
    private Coroutine lookCoroutine;


    [Header("=== Target List ===")]
    [SerializeField] private List<Attack_Slot> enemyAttackList;
    [SerializeField] private int enemyIndex;


    [Header("=== Attack ===")]
    [Range(0,1)]
    [SerializeField] private float posF;
    [SerializeField] private GameObject target_Lockon;
    [SerializeField] public List<GameObject> attackTargets; // �ټ� ���ݽ� ���
    [SerializeField] public GameObject attackTarget; // �Ѹ� ���� �� ���
    [SerializeField] private Attack_Base myAttack;
    public int curAttackCount;
    [SerializeField] private bool isExchange;

    [Header("---Animation---")]
    [SerializeField] private string[] animTrigger;
    [SerializeField] private string[] animBool;

    // ������ �ڵ� - �̽� ��ġ �𸣰���;;
    public void ResetAll()
    {
        for (int i = 0; i < animTrigger.Length; i++)
        {
            anim.ResetTrigger(animTrigger[i]);
        }

        for (int i = 0; i < animBool.Length; i++)
        {
            anim.SetBool(animBool[i], false);
        }
    }


    // �� ���� ���� �ִϸ��̼� ȣ��
    public void Turn_StartAnim()
    {
        curCoroutine = StartCoroutine(Turn_StartAnimCall());
    }

    // �� ���� ���� �ִϸ��̼� ���� -> ī�޶� ���� �ʿ���!
    private IEnumerator Turn_StartAnimCall()
    {
        isSpawnAnim = true;

        if(anim != null)
        {
            // Animation
            anim.SetTrigger("Action");
            anim.SetBool("isSpawn", true);

            // Camera Movement


            // Animation Wait
            while (anim.GetBool("isSpawn"))
            {
                yield return null;
            }
        }

        isSpawnAnim = false;
    }


    // ���� ���� �ӵ� ����
    public void Slot_SpeedSetting()
    {
        for (int i = 0; i < attackSlot.Count; i++)
        {
            int ran = Random.Range(Player_Manager.instnace.SlotSpeed.x, Player_Manager.instnace.SlotSpeed.y);
            attackSlot[i].Speed_Setting(ran);
        }
    }

    // ���� ���� �� ����
    public void Turn_AttackSelect()
    {
        if(isExchangeTargetSelect)
        {
            return;
        }
        isSelect = true;

        // ���� ���� -> �����ִ� ���� ���� ��� ���
        for (int i = 0; i < attackSlot.Count; i++)
        {
            attackSlot[i].ResetSlot();
        }
    }


    // �ٶ󺸱� ��� ����
    public void LookAt(GameObject target)
    {

        looktarget = target;
        if(lookCoroutine != null)
        {
            StopCoroutine(lookCoroutine);
        }
        lookCoroutine = StartCoroutine(LookAtCall());
    }
    private IEnumerator LookAtCall()
    {
        Vector3 lookDir = (looktarget.transform.position - transform.position).normalized;
        lookDir.y = 0;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2f;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, EasingFunctions.OutExpo(timer));
            yield return null;
        }

    }


    // ���� �� Ÿ�� ��ġ�� �̵�
    public void Turn_AttackMove()
    {
        StartCoroutine(Turn_AttackMoveCall());
    }

    private IEnumerator Turn_AttackMoveCall()
    {
        isExchangeMove = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttackMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // �̵� �غ� �ִϸ��̼� ���
        while(anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // ����
        Player_Manager.instnace.Turn_Sound_Call(0);

        // �̵�
        Vector3 startPos = transform.position;
        Vector3 endPos = attackTarget.GetComponent<Enemy_Base>().exchangePos.position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isAttackMove", false);

        yield return new WaitForSeconds(0.05f);
        isExchangeMove = false;
    }


    // ���Կ� ���� ����
    public void Slot_AttackSetting(Attack_Base attack)
    {
        // ���� ����Ʈ üũ
        if(Player_Manager.instnace.curAttackPoint < attack.attackCost)
        {
            Debug.Log("����Ʈ ����");
            return;
        }
        else
        {
            // �� ���� üũ
            for (int i = 0; i < attackSlot.Count; i++)
            {
                if (!attackSlot[i].haveAttack)
                {
                    // ���� ����Ʈ UI �Ҹ�
                    Player_Manager.instnace.AttackPointAdd(-attack.attackCost);

                    attackSlot[i].Attack_Setting(attack);
                    Slot_TargetSetting(attackSlot[i]);
                    return;
                }
            }

            Debug.Log("���� ����!");
        }
    }

    // �ش� �������� �� ������ ���� ȣ��
    public void Slot_TargetSetting(Attack_Slot slot)
    {
        StartCoroutine(Slot_TargetSettingCall(slot));
    }

    // �ش� �������� �� ������ ���� ����
    private IEnumerator Slot_TargetSettingCall(Attack_Slot slot)
    {
        isExchangeTargetSelect = true;

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.Select, 0.35f);

        // ����Ʈ ���� -> ���� ���� ��� null �� ���� ������
        enemyAttackList?.Clear(); // -> �̰� null ���� ����̶�µ� �Ƹ� ����ó�� ���̴� �뵵�ε�?

        // ���� ���� ���� ��������
        enemyAttackList = Player_Manager.instnace.turnManger.GetEnemyAttackSlot();

        // Ÿ�� ���� UI On
        enemyIndex = 0;
        Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>()); // -> �̰� �� ���� �߻�?
        Player_UI.instance.Turn_TargetSelect(true);
        slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);

        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);

        // �·� �� UI On
        Player_UI.instance.Turn_WinningUI(true, slot, enemyAttackList[enemyIndex]);

        // ���� �߱� ȿ��
        enemyAttackList[enemyIndex].Highlights_Effect(true);

        // ����
        Turn_LockOn(true, enemyAttackList[enemyIndex].slotOwner.transform);

        // �ٶ󺸱� On
        LookAt(enemyAttackList[0].slotOwner);

        // �ִϸ��̼� On
        if (selectAnimCoroutine != null)
        {
            StopCoroutine(selectAnimCoroutine);
        }
        selectAnimCoroutine = StartCoroutine(Slot_TargetSettingAnim(true));

        // �� & �Ϲ� �ؽ�Ʈ
        if (slot.slotSpeed >= enemyAttackList[enemyIndex].slotSpeed)
        {
            Player_UI.instance.Turn_Exchange(true);
        }
        else
        {
            Player_UI.instance.Turn_Exchange(false);
        }

        // Ÿ�� ���ñ��� ���
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (enemyAttackList.Count > 1)
            {
                // �·� �̵�
                if (Input.GetKeyDown(KeyCode.A))
                {
                    enemyAttackList[enemyIndex].Highlights_Effect(false);

                    enemyIndex = (enemyIndex - 1 + enemyAttackList.Count) % enemyAttackList.Count;
                    Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);
                    Player_UI.instance.Turn_WinningUI(true, slot, enemyAttackList[enemyIndex]);

                    // �� & �Ϲ� �ؽ�Ʈ
                    if (slot.slotSpeed >= enemyAttackList[enemyIndex].slotSpeed)
                    {
                        Player_UI.instance.Turn_Exchange(true);
                    }
                    else
                    {
                        Player_UI.instance.Turn_Exchange(false);
                    }

                    // ���� ����Ʈ
                    slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);
                    enemyAttackList[enemyIndex].Highlights_Effect(true);
                    Turn_LockOn(true, enemyAttackList[enemyIndex].slotOwner.transform);

                    // �÷��̾ �ٶ󺸴� Ÿ�� ����
                    LookAt(enemyAttackList[enemyIndex].slotOwner);
                }

                // ��� �̵�
                if (Input.GetKeyDown(KeyCode.D))
                {
                    enemyAttackList[enemyIndex].Highlights_Effect(false);

                    enemyIndex = (enemyIndex + 1) % enemyAttackList.Count;
                    Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);
                    Player_UI.instance.Turn_WinningUI(true, slot, enemyAttackList[enemyIndex]);

                    // �� & �Ϲ� �ؽ�Ʈ
                    if (slot.slotSpeed >= enemyAttackList[enemyIndex].slotSpeed)
                    {
                        Player_UI.instance.Turn_Exchange(true);
                    }
                    else
                    {
                        Player_UI.instance.Turn_Exchange(false);
                    }

                    // ���� ����Ʈ
                    slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);
                    enemyAttackList[enemyIndex].Highlights_Effect(true);
                    Turn_LockOn(true, enemyAttackList[enemyIndex].slotOwner.transform);

                    // �÷��̾ �ٶ󺸴� Ÿ�� ����
                    LookAt(enemyAttackList[enemyIndex].slotOwner);
                }
            }

            yield return null;
        }


        // �ٶ󺸱� Off
        LookAt(forwardTarget);


        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.4f);


        // �ִϸ��̼� Off
        if (selectAnimCoroutine != null)
        {
            StopCoroutine(selectAnimCoroutine);
        }
        selectAnimCoroutine = StartCoroutine(Slot_TargetSettingAnim(false));


        // Ÿ�� ���� -> ����ü ����
        Player_Manager.instnace.turnManger.Exchange_Setting_Add(slot, enemyAttackList[enemyIndex]);
        //Player_Manager.instnace.turnManger.Exchange_Setting_Add2(slot, enemyAttackList[enemyIndex]);
        slot.targetSlot = enemyAttackList[enemyIndex];


        // Ÿ�� ���� UI Off
        Player_UI.instance.Turn_TargetSelect(false);
        Player_UI.instance.Turn_WinningUI(false, null, null);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, slot, false);
        slot.Attack_LineSetting(false, null);
        enemyAttackList[enemyIndex].Highlights_Effect(false);
        isExchangeTargetSelect = false;
        Turn_LockOn(false, null);


        // ���� ���� üũ
        Turn_FightButtonCheck();
    }

    // Ÿ�� ���� �� �ִϸ��̼� On/Off
    private IEnumerator Slot_TargetSettingAnim(bool isOn)
    {
        if(isOn)
        {
            // �ִϸ��̼�
            anim.SetFloat("Movement", movement);
            float start = movement;
            float end = 1;
            float timer = 0;
            while (movement < 1)
            {
                timer += Time.deltaTime * 1.2f;
                movement = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
                anim.SetFloat("Movement", movement);
                yield return null;
            }
            movement = 1;
            anim.SetFloat("Movement", movement);
        }
        else
        {
            // �ִϸ��̼�
            anim.SetFloat("Movement", movement);
            float start = movement;
            float end = 0;
            float timer = 0;
            while (movement > 0)
            {
                timer += Time.deltaTime * 1.5f;
                movement = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
                anim.SetFloat("Movement", movement);
                yield return null;
            }
            movement = 0;
            anim.SetFloat("Movement", movement);
        }
    }


    // ���� �̹��� on/off ��ġ ����
    private void Turn_LockOn(bool isOn, Transform lockPos)
    {
        Debug.Log("Call LockOn" + isOn);
        target_Lockon.SetActive(isOn);
        if (isOn)
        {
            Vector3 pos = Vector3.Lerp(transform.position, lockPos.position, posF);
            pos.y = 1;
            target_Lockon.transform.position = pos;
        }
    }


    // ���� ���� ��ư Ȱ��ȭ üũ
    private void Turn_FightButtonCheck()
    {
        // ��� ���� �� ���� �������� ���� ������ �ִ��� üũ
        for (int i = 0; i < attackSlot.Count; i++)
        {
            // ���� ������ ���� �������� �ʾ��� ���
            if (!attackSlot[i].haveAttack)
            {
                return;
            }
        }

        // ��� ���Կ� ������ ������ ���
        Player_UI.instance.Turn_AttackButton(true);
    }

    // �Ϲ� ���� �̵� ȣ��
    public void Turn_OneSideMove(Transform movePos, float moveSpeed)
    {
        curCoroutine = StartCoroutine(Turn_OnesideMoveCall(movePos, moveSpeed));
    }

    // �Ϲ� ���� �̵� ����
    private IEnumerator Turn_OnesideMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isExchangeMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // �̵� �غ� ���� ���
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // ����
        Player_Manager.instnace.Turn_Sound_Call(0);

        // �̵� �ڵ�
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isExchangeMove", false);

        isExchangeMove = false;
    }
    

    // �� �̵� ȣ��
    public void Turn_ExchangeMove(Transform movePos, float moveSpeed)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos, moveSpeed));
    }

    // �� �̵� ����
    private IEnumerator Turn_EngageMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isExchangeMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // �̵� �غ� ���� ���
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // ����
        Player_Manager.instnace.Turn_Sound_Call(0);

        // �̵� �ڵ�
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        transform.position = endPos;

        isExchangeMove = false;
    }


    // �� ���� �� ���� �̵� ȣ��
    public void Turn_ExchangeEndMove(Transform movePos, float moveSpeed)
    {
        StartCoroutine(Turn_ExchangeEndMoveCall(movePos, moveSpeed));
    }

    // �� ���� �� ���� �̵� ����
    private IEnumerator Turn_ExchangeEndMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // Ÿ�� �ٶ󺸱�
        //Player_Manager.instnace.player_Turn.LookAt(Player_Manager.instnace.turnManger.exchangeHolderPos.gameObject);

        // �ִϸ��̼�
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isBackStep", true);
        }

        // ����
        Player_Manager.instnace.Turn_Sound_Call(1);

        // �̵�
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        while(anim.GetBool("isBackStep"))
        {
            yield return null;
        }
        isExchangeMove = false;
    }


    // �� ��� �ִϸ��̼� ȣ��
    public void Turn_ExchangeResuitAnim(ExchangeResuit type)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeResuitAnimCall(type));
    }


    // �� ��� �ִϸ��̼� ����
    private IEnumerator Turn_ExchangeResuitAnimCall(ExchangeResuit type)
    {
        isRecoilMove = true;

        // �� �� �и� �̵� ����
        StartCoroutine(Turn_ExchanageResuitMove(type));

        // �� �ִϸ��̼�
        if(anim != null)
        {
            // �̵� �ִϸ��̼� ����
            anim.SetBool("isExchangeMove", false);

            // �� ��� �ִϸ��̼� ȣ�� 
            anim.SetTrigger("Action");
            switch (type)
            {
                case ExchangeResuit.Win:
                case ExchangeResuit.Draw:
                    int ran = Random.Range(0, 3);
                    anim.SetInteger("WinAnim", ran);
                    anim.SetBool("isExchangeWin", true);
                    while(anim.GetBool("isExchangeWin"))
                    {
                        yield return null;
                    }
                    break;

                case ExchangeResuit.Lose:
                    anim.SetBool("isExchangeLose", true);
                    while (anim.GetBool("isExchangeLose"))
                    {
                        yield return null;
                    }
                    break;
            }
        }

        // Delay
        yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));

        isRecoilMove = false;
    }

    // �� �и� �̵� ����
    private IEnumerator Turn_ExchanageResuitMove(ExchangeResuit type)
    {
        // Recoil Move
        Vector3 startPos = transform.position;
        Vector3 endPos = exchange_RecoilPos[type == ExchangeResuit.Win ? 0 : (type == ExchangeResuit.Draw ? 1 : 2)].position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
    }


    // �ʻ�� ȣ�� 
    public void Turn_UndersideAttack()
    {
        if(Player_Manager.instnace.turnManger != null)
        {
            // ��ư UI ����
            Player_UI.instance.Turn_UndersideSkillOn(false);
            curAttackCount = 0;

            // ����
            Turn_AttackData_Setting(Player_Manager.instnace.turnManger.enemyObjects, undersideData, false, undersideData.attackCount);
            Player_Manager.instnace.turnManger.Turn_UnderSideAttack();
        }
        else
        {
            Debug.Log("�� �޴��� ����!");
        }
    }



    // ���� ��� ȣ�� & ���� ������ �Է� - ���� ����
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target">�����ϴ� ���</param>
    /// <param name="attack">���� ������</param>
    /// <param name="isExchange">�� ����</param>
    /// <param name="attackCount">���� ���� Ƚ��</param>
    public void Turn_AttackData_Setting(GameObject target, Attack_Base attack, bool isExchange, int attackCount)
    {
        Player_Manager.instnace.isAttack = true;
        Debug.Log("������ ���� ȣ�� / " + target);

        // ������ ����
        attackTarget = target;
        myAttack = attack;
        curAttackCount = 0;
        this.isExchange = isExchange;

        Debug.Log("������ ���� ȣ�� ���� ������ / " + attackTarget);


        // ���� UI ȣ��
        Player_UI.instance.TurnFight_ExchanageSummary(true, myAttack);

        // ���� ȣ��
        myAttack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, target, isExchange, attackCount);
    }

    // ���� ��� ȣ�� & ���� ������ �Է� - �ټ� ����
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target">�����ϴ� ���</param>
    /// <param name="attack">���� ������</param>
    /// <param name="isExchange">�� ����</param>
    /// <param name="attackCount">���� ���� Ƚ��</param>
    public void Turn_AttackData_Setting(List<GameObject> targets, Attack_Base attack, bool isExchange, int attackCount)
    {
        Player_Manager.instnace.isAttack = true;

        // ������ ����
        for (int i = 0; i < targets.Count; i++)
        {
            attackTargets.Add(targets[i]);
        }
        myAttack = attack;
        curAttackCount = 0;
        this.isExchange = isExchange;

        // ���� UI ȣ��
        Player_UI.instance.TurnFight_ExchanageSummary(true, myAttack);

        // ���� ȣ��
        myAttack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, targets, isExchange, attackCount);
    }


    // ���� ������ �ʱ�ȭ
    public void Turn_AttackData_Reset()
    {
        attackTarget = null;
        attackTargets.Clear();
        myAttack = null;
        curAttackCount = 0;
        isExchange = false;
    }


    // �ִϸ��̼� �̺�Ʈ���� ȣ���ϴ� �κ� - ���� �����͸� ����ؼ� ������ ������
    public void Attack_Call()
    {
        // ������ ���
        (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, curAttackCount);

        // ������ UI
        Player_UI.instance.TurnFight_ExchangeSummary_ExchangeValue(pdamage, 0.1f);

        // ������ ����
        if (myAttack.isAOE)
        {
            for (int i = 0; i < Player_Manager.instnace.turnManger.enemyObjects.Count; i++)
            {
                for (int j = curAttackCount; j < myAttack.damageValue.Length; j++)
                {
                    if (attackTargets[i] != null)
                    {
                        attackTargets[i].GetComponent<Enemy_Base>().Take_Damage(pdamage, myAttack.damageType[curAttackCount] == Attack_Base.DamageType.physical ? Enemy_Base.DamageType.Physical : Enemy_Base.DamageType.Magical, isExchange, isCritical);
                    }
                }
            }
        }
        else
        {
            attackTarget.GetComponent<Enemy_Base>().Take_Damage(pdamage, myAttack.damageType[curAttackCount] == Attack_Base.DamageType.physical ? Enemy_Base.DamageType.Physical : Enemy_Base.DamageType.Magical, isExchange, isCritical);
        }

        // ���� Ƚ�� ����
        curAttackCount++;
    }

    // �ִϸ��̼� 1�� & ������ ��Ÿ�� ���
    public void AttackCombo_Call()
    {
        StartCoroutine(AttackCombo());
    }

    private IEnumerator AttackCombo()
    {
        for (int i = 0; i < curAttackCount; i++)
        {
            (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, i);
            attackTarget.GetComponent<Enemy_Base>().Take_Damage(pdamage, Enemy_Base.DamageType.Physical, false, isCritical);
           
            // ������
            yield return new WaitForSeconds(0.15f);
        }
    }


    // �ʻ�� 10��Ÿ ����
    public void Attack_UndersideCall()
    {
        // ����
        Player_Manager.instnace.Turn_Sound_Call(4);

        // ������ ���
        (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, curAttackCount);
        for (int i = 0; i < attackTargets.Count; i++)
        {
            attackTargets[i].GetComponent<Enemy_Base>().Take_Damage(pdamage, Enemy_Base.DamageType.Physical, false, isCritical);
        }
        curAttackCount++;
    }

    // �ʻ�� ������ ����
    public void Attack_UndersideLastCall()
    {
        // ����
        Player_Manager.instnace.Turn_Sound_Call(10); //-> �ε��� �̽�

        // ������ ���
        Debug.Log($"�ʻ�� ��Ÿ ȣ�� Ƚ��2 / {curAttackCount} ���� ���� Ƚ��");
        (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, curAttackCount);
        for (int i = 0;  i < attackTargets.Count; i++) 
        {
            attackTargets[i].GetComponent<Enemy_Base>().Take_Damage(pdamage, Enemy_Base.DamageType.Magical, false, isCritical);
        }
    }


    public void AttackAOE_Call(GameObject effectObj)
    {
        // ����Ʈ ���
        for (int i = 0; i < Player_Manager.instnace.turnManger.enemyObjects.Count; i++)
        {
            Vector3 pos = new Vector3(attackTargets[i].transform.position.x, attackTargets[i].transform.position.y + effectObj.transform.position.y, attackTargets[i].transform.position.z);
            Instantiate(effectObj, pos, Quaternion.identity);
        }
    }

    // ���� ���� �ִϸ��̼� ȣ�� -> �¸�/��� ��� ������
    public void Turn_End(EndType type)
    {
        StartCoroutine(Turn_EndCall(type));
    }


    // ���� ���� �ִϸ��̼� -> ���� �ִϸ��̼� ���� ��� �ȸ���!
    private IEnumerator Turn_EndCall(EndType type)
    {
        isEndAnim = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool(type == EndType.Win ? "isWin" : "isDie", true);
        while (anim.GetBool(type == EndType.Win ? "isWin" : "isDie"))
        {
            yield return null;
        }

        isEndAnim = false;
    }
}
