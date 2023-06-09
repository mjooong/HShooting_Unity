using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    public Text m_BestScoreText = null; //�ְ����� ǥ�� UI
    public Text m_CurScoreText = null;  //�������� ǥ�� UI
    public Text m_GoldText = null;      //������� ǥ�� UI
    public Text m_UserInfoText = null;  //���� ���� ǥ�� UI
    public Button GoLobby_Btn = null;   //�κ�� �̵� ��ư

    int m_CurScore = 0; //�̹� ������������ ���� ���� ����
    int m_CurGold = 0;  //�̹� ������������ ���� ��尪

    //--- ĳ���� �Ӹ����� ������ ����� ���� ����
    GameObject m_DmgClone;  //Damage Text ���纻�� ���� ����
    DamageText m_DmgText;   //Damage Text ���纻�� �پ� �ִ� DamageText ������Ʈ�� ���� ����
    Vector3 m_StCacPos;     //���� ��ġ�� ����� �ֱ� ���� ����
    [Header("------ Damage Text ------")]
    public Transform  m_Damage_Canvas = null;
    public GameObject m_DamageRoot = null;
    //--- ĳ���� �Ӹ����� ������ ����� ���� ����

    //--- ���� ������ ���� ����
    GameObject m_CoinItem = null;
    //--- ���� ������ ���� ����

    //--- ��Ʈ ������ ���� ����
    GameObject m_HeartItem = null;
    //--- ��Ʈ ������ ���� ����

    [Header("------ Skill Timer ------")]
    public GameObject m_SkCoolNode = null;
    public Transform  m_SkillCoolRoot = null;

    [Header("------ Game Over ------")]
    public GameObject GameOverPanel = null;
    public Text       Result_Text = null;
    public Button     Replay_Btn = null;
    public Button     RstLobby_Btn = null;

    //--- �κ��丮 ���� ����
    [Header("------ Inventory Show OnOff ------")]
    public Button m_Inven_Btn = null;
    public Transform m_InvenScrollTr = null;
    bool m_Inven_ScOnOff = false;
    float m_ScSpeed = 9000.0f;
    Vector3 m_ScOnPos  = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 m_ScOffPos = new Vector3(-1000.0f, 0.0f, 0.0f);
    Vector3 m_BtnOnPos = new Vector3(410.0f, -247.8f, 0.0f);
    Vector3 m_BtnOffPos = new Vector3(-560.0f, -247.8f, 0.0f);
    public Transform m_ScContent;
    public GameObject m_SkSmallPrefab;
    //--- �κ��丮 ���� ����

    // --- Config Box (ȯ�漳��) ���� ����
    [Header("-----ConfigBox-----")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    // --- Config Box (ȯ�漳��) ���� ����

    HeroCtrl m_RefHero = null;  //���ΰ� ��ü ����

    //--- �̱��� ����
    public static Game_Mgr Inst = null;

    void Awake()
    {
        Inst = this;    
    }
    //--- �̱��� ����

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //���� �ӵ���...

        GlobalValue.LoadGameData();
        InitRefreshUI();
        RefreshSkillList();

        if (GoLobby_Btn != null)
            GoLobby_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        m_CoinItem = Resources.Load("CoinPrefab") as GameObject;
        m_HeartItem = Resources.Load("HeartPrefab") as GameObject;

        //GameObject a_HeroObj = GameObject.Find("HeroRoot");
        //if (a_HeroObj != null)
        //   m_RefHero = a_HeroObj.GetComponent<HeroCtrl>();
        m_RefHero = GameObject.FindObjectOfType<HeroCtrl>();

        //--- GameOver ��ư ó�� �κ�
        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("InGameScene");
            });

        if (RstLobby_Btn != null)
            RstLobby_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
        //--- GameOver ��ư ó�� �κ�

        if (m_Inven_Btn != null)
            m_Inven_Btn.onClick.AddListener(() =>
            {
                m_Inven_ScOnOff = !m_Inven_ScOnOff;
            });

        // --- ȯ�漳�� Dlg ���� �����κ�
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = Instantiate(m_ConfigBoxObj) as GameObject;
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);

                Time.timeScale = 0.0f;  // �Ͻ�����
            });
        // --- ȯ�漳�� Dlg ���� �����κ�

        Sound_Mgr.Instance.PlayBGM("sound_bgm_island_001", 1.0f);

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        //--- ����Ű �̿����� ��ų ����ϱ�
        if(Input.GetKeyDown(KeyCode.Alpha1) || 
           Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseSkill_Key(SkillType.Skill_0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Keypad2))
        {
            UseSkill_Key(SkillType.Skill_1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3) || 
            Input.GetKeyDown(KeyCode.Keypad3))
        {
            UseSkill_Key(SkillType.Skill_2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4) ||
            Input.GetKeyDown(KeyCode.Keypad4))
        {
            UseSkill_Key(SkillType.Skill_3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5) ||
            Input.GetKeyDown(KeyCode.Keypad5))
        {
            UseSkill_Key(SkillType.Skill_4);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6) ||
            Input.GetKeyDown(KeyCode.Keypad6))
        {
            UseSkill_Key(SkillType.Skill_5);
        }

        if (m_RefHero != null && m_RefHero.m_CurHP <= 0.0f)
            GameOverMethod(); //���ΰ��� ���Ϳ� ���� �����鼭 ���� ��
        //������ ������ ���� ���� �ǳڿ� �ݿ����� �ʴ� ������ �ذ��ϱ� ����
        //���ΰ��� HP�� ���ʿ��� üũ�ϰ� ȣ���� �ش�.
        
        ScrollViewOnOff_Update();

    }//void Update()

    public void DamageText(float a_Value, Vector3 a_Pos, Color a_Color)
    {
        if (m_Damage_Canvas == null || m_DamageRoot == null)
            return;

        m_DmgClone = (GameObject)Instantiate(m_DamageRoot);
        m_DmgClone.transform.SetParent(m_Damage_Canvas);
        m_DmgText = m_DmgClone.GetComponent<DamageText>();
        if (m_DmgText != null)
            m_DmgText.InitDamage(a_Value, a_Color);
        m_StCacPos = new Vector3(a_Pos.x, a_Pos.y + 1.14f, 0.0f);
        m_DmgClone.transform.position = m_StCacPos;
    }

    public void SpawnCoin(Vector3 a_Pos, int a_Value = 10)
    {
        if (m_CoinItem == null)
            return;

        GameObject a_CoinObj = (GameObject)Instantiate(m_CoinItem);
        a_CoinObj.transform.position = a_Pos;
        Coin_Ctrl a_CoinCtrl = a_CoinObj.GetComponent<Coin_Ctrl>();
        if (a_CoinCtrl != null)
            a_CoinCtrl.m_RefHero = m_RefHero;
    }

    public void AddScore(int a_Value = 10)
    {
        m_CurScore += a_Value;
        if (m_CurScore < 0)
            m_CurScore = 0;

        int a_MaxValue = int.MaxValue - 10; //int.MaxValue == 2147483647
        if(a_MaxValue < m_CurScore)
            m_CurScore = a_MaxValue;

        m_CurScoreText.text = "��������(" + m_CurScore + ")";
        if(GlobalValue.g_BestScore < m_CurScore)
        {
            GlobalValue.g_BestScore = m_CurScore;
            m_BestScoreText.text = "�ְ�����(" + GlobalValue.g_BestScore + ")";
            PlayerPrefs.SetInt("BestScore", GlobalValue.g_BestScore);
        }
    }//public void AddScore(int a_Value = 10)

    public void AddGold(int a_Value = 10)
    {
        m_CurGold += a_Value;   //�̹� ������������ ���� ��尪
        GlobalValue.g_UserGold += a_Value; //���ÿ� ����Ǿ� �ִ� ������ ���� ��尪

        int a_MaxValue = int.MaxValue - 10;
        if (a_MaxValue < GlobalValue.g_UserGold)
            GlobalValue.g_UserGold = a_MaxValue;

        m_GoldText.text = "�������(" + GlobalValue.g_UserGold + ")";
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
    }

    void InitRefreshUI()
    {
        if (m_BestScoreText != null)
            m_BestScoreText.text = "�ְ�����(" + GlobalValue.g_BestScore + ")";

        if (m_CurScoreText != null)
            m_CurScoreText.text = "��������(" + m_CurScore + ")";

        if (m_GoldText != null)
            m_GoldText.text = "�������(" + GlobalValue.g_UserGold + ")";

        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ")";
    }

    public void SpawnHeart(Vector3 a_Pos)
    {
        GameObject a_HeartObj = (GameObject)Instantiate(m_HeartItem);
        a_HeartObj.transform.position = a_Pos;
        Destroy(a_HeartObj, 10.0f);
    }

    void UseSkill_Key(SkillType a_SkType)
    {
        if (GlobalValue.m_SkDataList[(int)a_SkType].m_CurSkillCount <= 0)
            return; //�����ϰ� �ִ� ��ų �������� ����� �� ����

        if (m_RefHero == null)
            m_RefHero = GameObject.FindObjectOfType<HeroCtrl>();

        if (m_RefHero == null)
            return;

        m_RefHero.UseSkill(a_SkType);

        //--- ��ų ������ ���� UI ���� �ڵ�
        if (m_ScContent == null)
            return;

        SkSmallNode[] a_SkSmallList =
                        m_ScContent.GetComponentsInChildren<SkSmallNode>();
        for(int ii = 0; ii < a_SkSmallList.Length; ii++)
        {
            if(a_SkSmallList[ii].m_SkType == a_SkType)
            {
                a_SkSmallList[ii].Refresh_UI(a_SkType);
                break;
            }
        }
        //--- ��ų ������ ���� UI ���� �ڵ�
    }//void UseSkill_Key(SkillType a_SkType)

    public void SkillCoolMethod(SkillType a_SkType, float a_Time, float a_During)
    {
        GameObject Obj = Instantiate(m_SkCoolNode) as GameObject;
        Obj.transform.SetParent(m_SkillCoolRoot, false);
        SkillCool_Ctrl a_SCtrl = Obj.GetComponent<SkillCool_Ctrl>();
        if (a_SCtrl != null)
            a_SCtrl.InitState(a_SkType, a_Time, a_During);
    }

    public void GameOverMethod()
    {
        if (GameOverPanel != null && GameOverPanel.activeSelf == false)
            GameOverPanel.SetActive(true);

        if(Result_Text != null)
           Result_Text.text = "NickName\n" + GlobalValue.g_NickName + "\n\n" + 
                                "ȹ�� ����\n" + m_CurScore + "\n\n" + 
                                "ȹ�� ���\n" + m_CurGold;
    }

    void ScrollViewOnOff_Update()
    {
        if (m_InvenScrollTr == null)
            return;

        if(Input.GetKeyDown(KeyCode.R))
        {
            m_Inven_ScOnOff = !m_Inven_ScOnOff;
        }

        if(m_Inven_ScOnOff == false)
        {
            if(m_InvenScrollTr.localPosition.x > m_ScOffPos.x)
            {
                m_InvenScrollTr.localPosition =
                    Vector3.MoveTowards(m_InvenScrollTr.localPosition,
                            m_ScOffPos, m_ScSpeed * Time.deltaTime);
            }

            if(m_Inven_Btn.transform.localPosition.x > m_BtnOffPos.x)
            {
                m_Inven_Btn.transform.localPosition =
                    Vector3.MoveTowards(m_Inven_Btn.transform.localPosition,
                            m_BtnOffPos, m_ScSpeed * Time.deltaTime);
            }
        }//if(m_Inven_ScOnOff == false)
        else //if(m_Inven_ScOnOff == true)
        {
            if(m_ScOnPos.x > m_InvenScrollTr.localPosition.x)
            {
                m_InvenScrollTr.localPosition =
                    Vector3.MoveTowards(m_InvenScrollTr.localPosition,
                             m_ScOnPos, m_ScSpeed * Time.deltaTime);
            }

            if(m_BtnOnPos.x > m_Inven_Btn.transform.localPosition.x)
            {
                m_Inven_Btn.transform.localPosition =
                    Vector3.MoveTowards(m_Inven_Btn.transform.localPosition,
                            m_BtnOnPos, m_ScSpeed * Time.deltaTime);
            }
        }//else if(m_Inven_ScOnOff == true)

    }//void ScrollViewOnOff_Update()

    void RefreshSkillList() //���� Skill Item ����� UI�� �����ϴ� �Լ�
    {
        for(int ii = 0; ii < GlobalValue.m_SkDataList.Count; ii++)
        {
            GlobalValue.m_SkDataList[ii].m_CurSkillCount =
                                GlobalValue.m_SkDataList[ii].m_Level;

            if (GlobalValue.m_SkDataList[ii].m_Level <= 0)
                continue;

            GameObject a_SkillClone = Instantiate(m_SkSmallPrefab);
            a_SkillClone.GetComponent<SkSmallNode>().InitState(GlobalValue.m_SkDataList[ii]);
            a_SkillClone.transform.SetParent(m_ScContent, false);
        }
    }//void RefreshSkillList() 
}
