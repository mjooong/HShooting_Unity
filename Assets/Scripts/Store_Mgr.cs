using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button m_BackBtn = null;
    public Text m_UserInfoText = null;

    public GameObject m_Item_ScContent; //ScrollContent 차일드로 생성될 Parent 객체
    public GameObject m_Item_NodeObj;   //Node Prefab

    SkProductNode[] m_SkNodeList;       //<-- 스크롤에 붙어 있는 Item 목록들...

    //-- 지금 뭘 구입하려고 시도한 건지?
    SkillType m_BuySkType;
    int m_SvMyGold = 0;     //구입 프로세스에 진입 후 상태 저장용 : 차감된 내 골드가 얼마인지?
    int m_SvMyLevel = 0;
    //-- 지금 뭘 구입하려고 시도한 건지?

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
            m_UserInfoText.text = "별명(" + GlobalValue.g_NickName + ") : 보유골드(" +
                                    GlobalValue.g_UserGold + ")";

        //--- 아이템 목록 추가
        GameObject a_ItemObj = null;
        SkProductNode a_SkItemNode = null;
        for(int ii = 0; ii < GlobalValue.m_SkDataList.Count; ii++)
        {
            a_ItemObj = (GameObject)Instantiate(m_Item_NodeObj);
            a_SkItemNode = a_ItemObj.GetComponent<SkProductNode>();
            a_SkItemNode.InitData(GlobalValue.m_SkDataList[ii].m_SkType);
            a_ItemObj.transform.SetParent(m_Item_ScContent.transform, false);
        }
        //--- 아이템 목록 추가

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

            if(GlobalValue.m_SkDataList[ii].m_Level <= 0) //구매 전 상태
            {
                m_SkNodeList[ii].SetState(GlobalValue.m_SkDataList[ii].m_Price,
                                            GlobalValue.m_SkDataList[ii].m_Level);
            }
            else //이미 구입된 상태(업그레이드 해야 하는 상태)
            {
                m_SkNodeList[ii].SetState(GlobalValue.m_SkDataList[ii].m_UpPrice,
                                            GlobalValue.m_SkDataList[ii].m_Level);
            }
        }//for(int ii = 0; ii < GlobalValue.m_SkDataList.Count; ii++)
    }

    public void BuySkillItem(SkillType a_SkType)
    { //리스트뷰에 있는 캐릭터 가격버튼을 눌러 구입시도를 한 경우

        if (a_SkType < SkillType.Skill_0 || SkillType.SkCount <= a_SkType)
            return;

        string a_Mess = "";
        bool a_NeedDelegate = false;
        Skill_Info a_SkInfo = GlobalValue.m_SkDataList[(int)a_SkType];
        int a_Cost = 0;
        if(a_SkInfo.m_Level <= 0) //최초 구입인 경우
        {
            a_Cost = a_SkInfo.m_Price;
            if(GlobalValue.g_UserGold < a_SkInfo.m_Price)
            {
                a_Mess = "보유(누적) 골드가 부족합니다.";
            }
            else
            {
                a_Mess = "정말 구입하시겠습니까?";
                a_NeedDelegate = true;  //<-- 이 조건일 때 구매
            }
        }
        else //(업그레이드 기능) 상태
        {
            a_Cost = a_SkInfo.m_UpPrice +
                        (a_SkInfo.m_UpPrice * (a_SkInfo.m_Level - 1));
            if(5 <= a_SkInfo.m_Level)
            {
                a_Mess = "최고 레벨입니다.";

            }
            else if(GlobalValue.g_UserGold < a_Cost)
            {
                a_Mess = "레벨업에 필요한 보유(누적) 골드가 부족합니다.";
            }
            else
            {
                a_Mess = "정말 업그레이드 하시겠습니까?";
                a_NeedDelegate = true;  //<-- 이 조건일 때 업그레이드
            }
        }//else //(업그레이드 기능) 상태

        m_BuySkType = a_SkType;
        m_SvMyGold = GlobalValue.g_UserGold;
        m_SvMyGold -= a_Cost;  //골드값 차감 백업해 놓기...
        m_SvMyLevel = GlobalValue.m_SkDataList[(int)a_SkType].m_Level;
        m_SvMyLevel++;  //레벨 증가 백업해 놓기...

        GameObject a_DlgRsc = Resources.Load("DlgBox") as GameObject;
        GameObject a_DlgBoxObj = (GameObject)Instantiate(a_DlgRsc);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);
        //false Prefab의 로컬 포지션을 유지하면서 추가해 주겠다는 뜻
        DlgBox_Ctrl a_DlgBox = a_DlgBoxObj.GetComponent<DlgBox_Ctrl>();
        if(a_DlgBox != null)
        {
            if (a_NeedDelegate == true)
                a_DlgBox.SetMessage(a_Mess, TryBuySkItem);
            else
                a_DlgBox.SetMessage(a_Mess);
        }

    }//public void BuySkillItem(SkillType a_SkType)

    public void TryBuySkItem() //구매 확정 함수
    {
        if (m_BuySkType < SkillType.Skill_0 || SkillType.SkCount <= m_BuySkType)
            return;

        GlobalValue.g_UserGold = m_SvMyGold; //골드값 차감
        GlobalValue.m_SkDataList[(int)m_BuySkType].m_Level = m_SvMyLevel; //레벨 증가

        RefreshSkItemList();
        m_UserInfoText.text = "별명(" + GlobalValue.g_NickName +
                            ") : 보유골드(" + GlobalValue.g_UserGold + ")";

        //--- 로컬에 저장
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        string a_KeyBuff = string.Format("Skill_Item_{0}", (int)m_BuySkType);
        PlayerPrefs.SetInt(a_KeyBuff,
                                GlobalValue.m_SkDataList[(int)m_BuySkType].m_Level);
        //--- 로컬에 저장
    }
}
