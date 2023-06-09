using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    public Text m_BestScoreText = null; //최고점수 표시 UI
    public Text m_CurScoreText = null;  //현재점수 표시 UI
    public Text m_GoldText = null;      //보유골드 표시 UI
    public Text m_UserInfoText = null;  //유저 정보 표시 UI
    public Button GoLobby_Btn = null;   //로비로 이동 버튼

    int m_CurScore = 0; //이번 스테이지에서 얻은 게임 점수
    int m_CurGold = 0;  //이번 스테이지에서 얻은 골드값

    //--- 캐릭터 머리위에 데미지 띄우기용 변수 선언
    GameObject m_DmgClone;  //Damage Text 복사본을 받을 변수
    DamageText m_DmgText;   //Damage Text 복사본에 붙어 있는 DamageText 컴포넌트를 받을 변수
    Vector3 m_StCacPos;     //시작 위치를 계산해 주기 위한 변수
    [Header("------ Damage Text ------")]
    public Transform  m_Damage_Canvas = null;
    public GameObject m_DamageRoot = null;
    //--- 캐릭터 머리위에 데미지 띄우기용 변수 선언

    //--- 코인 아이템 관련 변수
    GameObject m_CoinItem = null;
    //--- 코인 아이템 관련 변수

    //--- 하트 아이템 관련 변수
    GameObject m_HeartItem = null;
    //--- 하트 아이템 관련 변수

    [Header("------ Skill Timer ------")]
    public GameObject m_SkCoolNode = null;
    public Transform  m_SkillCoolRoot = null;

    [Header("------ Game Over ------")]
    public GameObject GameOverPanel = null;
    public Text       Result_Text = null;
    public Button     Replay_Btn = null;
    public Button     RstLobby_Btn = null;

    //--- 인벤토리 관련 변수
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
    //--- 인벤토리 관련 변수

    // --- Config Box (환경설정) 관련 변수
    [Header("-----ConfigBox-----")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    // --- Config Box (환경설정) 관련 변수

    HeroCtrl m_RefHero = null;  //주인공 객체 변수

    //--- 싱글턴 패턴
    public static Game_Mgr Inst = null;

    void Awake()
    {
        Inst = this;    
    }
    //--- 싱글턴 패턴

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //원래 속도로...

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

        //--- GameOver 버튼 처리 부분
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
        //--- GameOver 버튼 처리 부분

        if (m_Inven_Btn != null)
            m_Inven_Btn.onClick.AddListener(() =>
            {
                m_Inven_ScOnOff = !m_Inven_ScOnOff;
            });

        // --- 환경설정 Dlg 관련 구현부분
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = Instantiate(m_ConfigBoxObj) as GameObject;
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);

                Time.timeScale = 0.0f;  // 일시정지
            });
        // --- 환경설정 Dlg 관련 구현부분

        Sound_Mgr.Instance.PlayBGM("sound_bgm_island_001", 1.0f);

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        //--- 단축키 이용으로 스킬 사용하기
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
            GameOverMethod(); //주인공이 몬스터에 들이 받으면서 죽을 때
        //마지막 점수가 게임 오버 판넬에 반영되지 않는 문제를 해결하기 위해
        //주인공의 HP를 이쪽에서 체크하고 호출해 준다.
        
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

        m_CurScoreText.text = "현재점수(" + m_CurScore + ")";
        if(GlobalValue.g_BestScore < m_CurScore)
        {
            GlobalValue.g_BestScore = m_CurScore;
            m_BestScoreText.text = "최고점수(" + GlobalValue.g_BestScore + ")";
            PlayerPrefs.SetInt("BestScore", GlobalValue.g_BestScore);
        }
    }//public void AddScore(int a_Value = 10)

    public void AddGold(int a_Value = 10)
    {
        m_CurGold += a_Value;   //이번 스테이지에서 얻은 골드값
        GlobalValue.g_UserGold += a_Value; //로컬에 저장되어 있는 유저의 보유 골드값

        int a_MaxValue = int.MaxValue - 10;
        if (a_MaxValue < GlobalValue.g_UserGold)
            GlobalValue.g_UserGold = a_MaxValue;

        m_GoldText.text = "보유골드(" + GlobalValue.g_UserGold + ")";
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
    }

    void InitRefreshUI()
    {
        if (m_BestScoreText != null)
            m_BestScoreText.text = "최고점수(" + GlobalValue.g_BestScore + ")";

        if (m_CurScoreText != null)
            m_CurScoreText.text = "현재점수(" + m_CurScore + ")";

        if (m_GoldText != null)
            m_GoldText.text = "보유골드(" + GlobalValue.g_UserGold + ")";

        if (m_UserInfoText != null)
            m_UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName + ")";
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
            return; //보유하고 있는 스킬 소진으로 사용할 수 없음

        if (m_RefHero == null)
            m_RefHero = GameObject.FindObjectOfType<HeroCtrl>();

        if (m_RefHero == null)
            return;

        m_RefHero.UseSkill(a_SkType);

        //--- 스킬 아이템 사용시 UI 갱신 코드
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
        //--- 스킬 아이템 사용시 UI 갱신 코드
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
                                "획득 점수\n" + m_CurScore + "\n\n" + 
                                "획득 골드\n" + m_CurGold;
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

    void RefreshSkillList() //보유 Skill Item 목록을 UI에 복원하는 함수
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
