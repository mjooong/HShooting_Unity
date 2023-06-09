using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    //---- 키보드 입력값 변수 선언
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;
    //---- 키보드 입력값 변수 선언

    //---- 주인공이 화면 밖으로 나갈 수 없도록 막기 위한 변수
    Vector3 HalfSize    = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;
    //---- 주인공이 화면 밖으로 나갈 수 없도록 막기 위한 변수

    //---- 총알 발사 변수
    public GameObject m_BulletPrefab = null;
    public GameObject m_ShootPos = null;
    float m_ShootCool = 0.0f;           //총알 발사 주기 계산용 변수
    //---- 총알 발사 변수

    //---- 주인공 체력 변수
    float m_MaxHP = 200.0f;
    [HideInInspector] public float m_CurHP = 200.0f;
    public Image m_HpBar = null;
    //---- 주인공 체력 변수

    //--- Wolf 스킬
    public GameObject m_WolfPrefab = null;
    //--- Wolf 스킬

    //--- 쉴드 스킬
    float m_SdOnTime = 0.0f;
    float m_SdDuration = 12.0f; //12초 동안 발동
    public GameObject ShieldObj = null;
    //--- 쉴드 스킬

    //--- 유도탄 스킬
    [HideInInspector] public bool IsHoming = false;
    float m_Homing_OnTime = 0.0f;
    float m_HomingDur = 12.0f;
    //--- 유도탄 스킬

    //--- 더블샷 스킬
    [HideInInspector] public float m_Double_OnTime = 0.0f;
    float m_Double_Dur = 12.0f;
    //--- 더블샷 스킬

    //--- Sub Hero
    int Sub_Count = 3;
    float m_Sub_OnTime = 0.0f;
    float m_Sub_Dur = 12.0f;
    public GameObject Sub_Hero_Prefab = null;
    public GameObject Sub_Parent = null;
    //--- Sub Hero

    // Start is called before the first frame update
    void Start()
    {
        //--- 캐릭터의 가로 반사이즈, 새로 반사이즈 구하기
        //월드에 그려진 스프라이트 사이즈 얻어오기
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f; //여백이 커서 조금 줄임
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f;
        HalfSize.z = 1.0f;
        //월드에 그려진 스프라이트 사이즈 얻어오기
    }

    // Update is called once per frame
    void Update()
    { 
        h = Input.GetAxis("Horizontal");    // -1.0f ~ 1.0f
        v = Input.GetAxis("Vertical");      // -1.0f ~ 1.0f

        if(h != 0.0f || v != 0.0f)
        {
            moveDir = new Vector3(h, v, 0);
            if (1.0f < moveDir.magnitude)
                moveDir.Normalize();

            transform.position +=
                        moveDir * moveSpeed * Time.deltaTime;
        }//if(h != 0.0f || v != 0.0f)

        LimmitMove();

        FireUpdate();

        Update_Skill();

    }//void Update()

    void LimmitMove()
    {
        m_CacCurPos = transform.position;

        if (m_CacCurPos.x < CameraResolution.m_ScreenMin.x + HalfSize.x)
            m_CacCurPos.x = CameraResolution.m_ScreenMin.x + HalfSize.x;

        if (CameraResolution.m_ScreenMax.x - HalfSize.x < m_CacCurPos.x)
            m_CacCurPos.x = CameraResolution.m_ScreenMax.x - HalfSize.x;

        if (m_CacCurPos.y < CameraResolution.m_ScreenMin.y + HalfSize.y)
            m_CacCurPos.y = CameraResolution.m_ScreenMin.y + HalfSize.y;

        if (CameraResolution.m_ScreenMax.y - HalfSize.y < m_CacCurPos.y)
            m_CacCurPos.y = CameraResolution.m_ScreenMax.y - HalfSize.y;

        transform.position = m_CacCurPos;
    }

    void FireUpdate()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;

        //if(Input.GetKeyDown(KeyCode.Space) == true)
        if(m_ShootCool <= 0.0f)
        {
            Bullet_Ctrl a_BulletSc = null;

            if (0.0f < m_Double_OnTime) //더블샷
            {
                Vector3 a_Pos;
                GameObject a_CloneObj;
                for(int ii = 0; ii < 2; ii++)
                {
                    a_CloneObj = (GameObject)Instantiate(m_BulletPrefab);
                    a_Pos = m_ShootPos.transform.position;
                    a_Pos.y += 0.2f - (ii * 0.4f);
                    a_CloneObj.transform.position = a_Pos;
                    a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                    if (a_BulletSc != null)
                        a_BulletSc.IsHoming = IsHoming;
                }
            }
            else //일반총알
            {
                GameObject a_CloneObj = Instantiate(m_BulletPrefab) as GameObject;
                a_CloneObj.transform.position = m_ShootPos.transform.position;
                a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                if (a_BulletSc != null)
                    a_BulletSc.IsHoming = IsHoming;
            }

            Sound_Mgr.Instance.PlayEffSound("gun", 0.5f);

            m_ShootCool = 0.15f;
        }
    }//void FireUpdate()

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "Monster")
        {
            TakeDamage(50.0f);
            Monster_Ctrl a_RefMon = coll.gameObject.GetComponent<Monster_Ctrl>();
            if (a_RefMon != null)
                a_RefMon.TakeDamage(1000);
        }
        else if(coll.tag == "EnemyBullet")
        {
            TakeDamage(20.0f);
            Destroy(coll.gameObject);
        }
        else if(coll.gameObject.name.Contains("CoinPrefab") == true)
        {
            //유저 골드 증가
            Game_Mgr.Inst.AddGold();
            Destroy(coll.gameObject);
        }
        else if(coll.gameObject.name.Contains("Heart") == true)
        {
            m_CurHP += m_MaxHP * 0.5f;
            Game_Mgr.Inst.DamageText(m_MaxHP * 0.5f,
                                transform.position, new Color(0.18f, 0.5f, 0.34f));

            if (m_MaxHP < m_CurHP)
                m_CurHP = m_MaxHP;

            if (m_HpBar != null)
                m_HpBar.fillAmount = m_CurHP / m_MaxHP;

            Destroy(coll.gameObject);
        }
    }

    void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f)
            return;

        if (0.0f < m_SdOnTime) //쉴드 스킬 발동 중 일 때... 데미지 스킵
            return;

        Game_Mgr.Inst.DamageText(-a_Value, transform.position, Color.blue);

        m_CurHP -= a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f)
        {   //사망처리
            Time.timeScale = 0.0f;
        }
    }//void TakeDamage(float a_Value)

    void Update_Skill()
    {
        //--- 쉴드 상태 업데이트
        if(0.0f < m_SdOnTime)
        {
            m_SdOnTime -= Time.deltaTime;
            if (ShieldObj != null && ShieldObj.activeSelf == false)
                ShieldObj.SetActive(true);
        }
        else
        {
            if (ShieldObj != null && ShieldObj.activeSelf == true)
                ShieldObj.SetActive(false);
        }
        //--- 쉴드 상태 업데이트

        //--- 유도탄 상태 업데이트
        if(0.0f < m_Homing_OnTime)
        {
            m_Homing_OnTime -= Time.deltaTime;

            if (m_Homing_OnTime < 0.0f)
                m_Homing_OnTime = 0.0f;

            IsHoming = true;
        }
        else
        {
            IsHoming = false;
        }
        //--- 유도탄 상태 업데이트

        //--- 더블 샷
        if(0.0f < m_Double_OnTime)
        {
            m_Double_OnTime -= Time.deltaTime;

            if (m_Double_OnTime < 0.0f)
                m_Double_OnTime = 0.0f;
        }
        //--- 더블 샷

        //--- Sub Hero 업데이트
        if(0.0f < m_Sub_OnTime)
        {
            m_Sub_OnTime -= Time.deltaTime;

            if (m_Sub_OnTime < 0.0f)
                m_Sub_OnTime = 0.0f;
        }
        //--- Sub Hero 업데이트
    }

    public void UseSkill(SkillType a_SkType)
    {
        if (m_CurHP <= 0) //주인공 사망시 스킬 발동 제외
            return;

        if(a_SkType == SkillType.Skill_0)
        {
            m_CurHP += m_MaxHP * 0.5f;
            Game_Mgr.Inst.DamageText(m_MaxHP * 0.5f, transform.position,
                                    new Color(0.18f, 0.5f, 0.34f));
            if (m_MaxHP < m_CurHP)
                m_CurHP = m_MaxHP;

            if (m_HpBar != null)
                m_HpBar.fillAmount = m_CurHP / m_MaxHP;
        }//if(a_SkType == SkillType.Skill_0)
        else if(a_SkType == SkillType.Skill_1) //울프 스킬
        {
            GameObject a_Clone = Instantiate(m_WolfPrefab) as GameObject;
            a_Clone.transform.position =
                new Vector3(CameraResolution.m_ScreenMin.x - 1.0f, 0.0f, 0.0f);
        }
        else if(a_SkType == SkillType.Skill_2) //보호막
        {
            if (0.0f < m_SdOnTime)
                return;

            m_SdOnTime = m_SdDuration;

            //UI 쿨타임 발동
            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_SdOnTime, m_SdDuration);
        }
        else if(a_SkType == SkillType.Skill_3) //유도탄
        {
            if (0.0f < m_Homing_OnTime)
                return;

            m_Homing_OnTime = m_HomingDur;

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_Homing_OnTime, m_HomingDur);
        }
        else if(a_SkType == SkillType.Skill_4) //더블샷
        {
            if (0.0f < m_Double_OnTime)
                return;

            m_Double_OnTime = m_Double_Dur;

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_Double_OnTime, m_Double_Dur);
        }
        else if(a_SkType == SkillType.Skill_5) //소환수 스킬
        {
            if (0.0f < m_Sub_OnTime)
                return;

            Sub_Count = 3;
            m_Sub_OnTime = m_Sub_Dur;

            for(int ii = 0; ii < Sub_Count; ii++)
            {
                GameObject obj = Instantiate(Sub_Hero_Prefab) as GameObject;
                obj.transform.SetParent(Sub_Parent.transform);
                SubHero_Ctrl sub = obj.GetComponent<SubHero_Ctrl>();
                if (sub != null)
                    sub.SubHeroSpawn(this, (360 / Sub_Count) * ii, m_Sub_OnTime);
            }

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_Sub_OnTime, m_Sub_Dur);

        }//else if(a_SkType == SkillType.Skill_5) //소환수 스킬

        GlobalValue.m_SkDataList[(int)a_SkType].m_CurSkillCount--;

    }//public void UseSkill(SkillType a_SkType)
}
