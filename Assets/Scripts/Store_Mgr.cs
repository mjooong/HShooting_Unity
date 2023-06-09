using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button m_BackBtn = null;
    public Text m_UserInfoText = null;

    public GameObject m_Item_ScContent; //ScrollContent ���ϵ�� ������ Parent ��ü
    public GameObject m_Item_NodeObj;   //Node Prefab

    SkProductNode[] m_SkNodeList;       //<-- ��ũ�ѿ� �پ� �ִ� Item ��ϵ�...

    //-- ���� �� �����Ϸ��� �õ��� ����?
    SkillType m_BuySkType;
    int m_SvMyGold = 0;     //���� ���μ����� ���� �� ���� ����� : ������ �� ��尡 ������?
    int m_SvMyLevel = 0;
    //-- ���� �� �����Ϸ��� �õ��� ����?

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        if (m_UserInfoText != null)
            m_UserInfoText.text = "����(" + GlobalValue.g_NickName + ") : �������(" +
                                    GlobalValue.g_UserGold + ")";

        //--- ������ ��� �߰�
        GameObject a_ItemObj = null;
        SkProductNode a_SkItemNode = null;
        for(int ii = 0; ii < GlobalValue.m_SkDataList.Count; ii++)
        {
            a_ItemObj = (GameObject)Instantiate(m_Item_NodeObj);
            a_SkItemNode = a_ItemObj.GetComponent<SkProductNode>();
            a_SkItemNode.InitData(GlobalValue.m_SkDataList[ii].m_SkType);
            a_ItemObj.transform.SetParent(m_Item_ScContent.transform, false);
        }
        //--- ������ ��� �߰�

        RefreshSkItemList();
    }//void Start()

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshSkItemList()
    {
        if(m_Item_ScContent != null)
        {
            if (m_SkNodeList == null || m_SkNodeList.Length <= 0)
                m_SkNodeList = m_Item_ScContent.GetComponentsInChildren<SkProductNode>();
        }

        for(int ii = 0; ii < GlobalValue.m_SkDataList.Count; ii++)
        {
            if (m_SkNodeList[ii].m_SkType != GlobalValue.m_SkDataList[ii].m_SkType)
                continue;

            if(GlobalValue.m_SkDataList[ii].m_Level <= 0) //���� �� ����
            {
                m_SkNodeList[ii].SetState(GlobalValue.m_SkDataList[ii].m_Price,
                                            GlobalValue.m_SkDataList[ii].m_Level);
            }
            else //�̹� ���Ե� ����(���׷��̵� �ؾ� �ϴ� ����)
            {
                m_SkNodeList[ii].SetState(GlobalValue.m_SkDataList[ii].m_UpPrice,
                                            GlobalValue.m_SkDataList[ii].m_Level);
            }
        }//for(int ii = 0; ii < GlobalValue.m_SkDataList.Count; ii++)
    }

    public void BuySkillItem(SkillType a_SkType)
    { //����Ʈ�信 �ִ� ĳ���� ���ݹ�ư�� ���� ���Խõ��� �� ���

        if (a_SkType < SkillType.Skill_0 || SkillType.SkCount <= a_SkType)
            return;

        string a_Mess = "";
        bool a_NeedDelegate = false;
        Skill_Info a_SkInfo = GlobalValue.m_SkDataList[(int)a_SkType];
        int a_Cost = 0;
        if(a_SkInfo.m_Level <= 0) //���� ������ ���
        {
            a_Cost = a_SkInfo.m_Price;
            if(GlobalValue.g_UserGold < a_SkInfo.m_Price)
            {
                a_Mess = "����(����) ��尡 �����մϴ�.";
            }
            else
            {
                a_Mess = "���� �����Ͻðڽ��ϱ�?";
                a_NeedDelegate = true;  //<-- �� ������ �� ����
            }
        }
        else //(���׷��̵� ���) ����
        {
            a_Cost = a_SkInfo.m_UpPrice +
                        (a_SkInfo.m_UpPrice * (a_SkInfo.m_Level - 1));
            if(5 <= a_SkInfo.m_Level)
            {
                a_Mess = "�ְ� �����Դϴ�.";

            }
            else if(GlobalValue.g_UserGold < a_Cost)
            {
                a_Mess = "�������� �ʿ��� ����(����) ��尡 �����մϴ�.";
            }
            else
            {
                a_Mess = "���� ���׷��̵� �Ͻðڽ��ϱ�?";
                a_NeedDelegate = true;  //<-- �� ������ �� ���׷��̵�
            }
        }//else //(���׷��̵� ���) ����

        m_BuySkType = a_SkType;
        m_SvMyGold = GlobalValue.g_UserGold;
        m_SvMyGold -= a_Cost;  //��尪 ���� ����� ����...
        m_SvMyLevel = GlobalValue.m_SkDataList[(int)a_SkType].m_Level;
        m_SvMyLevel++;  //���� ���� ����� ����...

        GameObject a_DlgRsc = Resources.Load("DlgBox") as GameObject;
        GameObject a_DlgBoxObj = (GameObject)Instantiate(a_DlgRsc);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);
        //false Prefab�� ���� �������� �����ϸ鼭 �߰��� �ְڴٴ� ��
        DlgBox_Ctrl a_DlgBox = a_DlgBoxObj.GetComponent<DlgBox_Ctrl>();
        if(a_DlgBox != null)
        {
            if (a_NeedDelegate == true)
                a_DlgBox.SetMessage(a_Mess, TryBuySkItem);
            else
                a_DlgBox.SetMessage(a_Mess);
        }

    }//public void BuySkillItem(SkillType a_SkType)

    public void TryBuySkItem() //���� Ȯ�� �Լ�
    {
        if (m_BuySkType < SkillType.Skill_0 || SkillType.SkCount <= m_BuySkType)
            return;

        GlobalValue.g_UserGold = m_SvMyGold; //��尪 ����
        GlobalValue.m_SkDataList[(int)m_BuySkType].m_Level = m_SvMyLevel; //���� ����

        RefreshSkItemList();
        m_UserInfoText.text = "����(" + GlobalValue.g_NickName +
                            ") : �������(" + GlobalValue.g_UserGold + ")";

        //--- ���ÿ� ����
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        string a_KeyBuff = string.Format("Skill_Item_{0}", (int)m_BuySkType);
        PlayerPrefs.SetInt(a_KeyBuff,
                                GlobalValue.m_SkDataList[(int)m_BuySkType].m_Level);
        //--- ���ÿ� ����
    }
}
