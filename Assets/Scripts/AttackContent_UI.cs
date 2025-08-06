using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AttackContent_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("=== Data ===")]
    [SerializeField] private Attack_Base myAttack;


    [Header("=== UI ===")]
    [SerializeField] private Image icon;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image iconBorderImage;
    [SerializeField] private Text attackName_Text;
    [SerializeField] private Text attackCost_Text;
    [SerializeField] private Text attacDamage_Text;
    [SerializeField] private Text attackDescription_Text;

    public System.Action click_Action;

    private void Start()
    {
        if (myAttack != null)
        {
            UI_Setting();
        }
    }

    private void UI_Setting()
    {
        icon.sprite = myAttack.icon;
        attackName_Text.text = myAttack.attackName;
        attackDescription_Text.text = myAttack.attackDescription_Text;
        attackCost_Text.text = myAttack.attackCost.ToString();


        // �ּ� ~ �ִ� �������� ����
        (int playerDamage, int pMinDamage, int pMaxDamage) = Player_Manager.instnace.Turn_WinningCheck(myAttack);
        attacDamage_Text.text = $"���� ������ \n{pMinDamage} ~ {pMaxDamage}";

        /*
        // ������ ����
        // ���ڿ��� ������ StringBuilder ���
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // damageValue �迭�� ������ �ݺ��Ͽ� �ؽ�Ʈ �������� ��ȯ
        for (int i = 0; i < myAttack.damageValue.Length; i++)
        {
            sb.Append($"{myAttack.damageValue[i].x} ~ {myAttack.damageValue[i].y}");

            // ������ ��Ұ� �ƴ϶�� �ٹٲ� �߰�
            if (i < myAttack.damageValue.Length - 1)
            {
                sb.Append("\n");
            }
        }

        // ��ȯ�� ���ڿ��� Text ������Ʈ�� �Ҵ�
        attacDamage_Text.text = sb.ToString();
        */
    }


    #region ���콺 �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ����
        Player_Manager.instnace.UI_Sound_Call(0);

        borderImage.color = Color.gray;
        iconBorderImage.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImage.color = Color.white;
        iconBorderImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ����
        Player_Manager.instnace.UI_Sound_Call(1);

        // �̹� ��ų ���������� Ȯ�� -> �ڿ� üũ�� Slot_AttackSetting()���� �ϴ���!
        if (Player_Manager.instnace.player_Turn.isExchangeTargetSelect)
        {
            return;
        }
        else
        {
            Player_Manager.instnace.player_Turn.Slot_AttackSetting(myAttack);
            click_Action?.Invoke();
        }
    }
    #endregion
}

