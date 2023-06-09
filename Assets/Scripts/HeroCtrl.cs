using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    //---- Ű���� �Է°� ���� ����
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;
    //---- Ű���� �Է°� ���� ����

    //---- ���ΰ��� ȭ�� ������ ���� �� ������ ���� ���� ����
    Vector3 HalfSize    = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;
    //---- ���ΰ��� ȭ�� ������ ���� �� ������ ���� ���� ����

    //---- �Ѿ� �߻� ����
    public GameObject m_BulletPrefab = null;
    public GameObject m_ShootPos = null;
    float m_ShootCool = 0.0f;           //�Ѿ� �߻� �ֱ� ���� ����
    //---- �Ѿ� �߻� ����

    //---- ���ΰ� ü�� ����
    float m_MaxHP = 200.0f;
    [HideInInspector] public float m_CurHP = 200.0f;
    public Image m_HpBar = null;
    //---- ���ΰ� ü�� ����

    //--- Wolf ��ų
    public GameObject m_WolfPrefab = null;
    //--- Wolf ��ų

    //--- ���� ��ų
    float m_SdOnTime = 0.0f;
    float m_SdDuration = 12.0f; //12�� ���� �ߵ�
    public GameObject ShieldObj = null;
    //--- ���� ��ų

    //--- ����ź ��ų
    [HideInInspector] public bool IsHoming = false;
    float m_Homing_OnTime = 0.0f;
    float m_HomingDur = 12.0f;
    //--- ����ź ��ų

    //--- ���� ��ų
    [HideInInspector] public float m_Double_OnTime = 0.0f;
    float m_Double_Dur = 12.0f;
    //--- ���� ��ų

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
        //--- ĳ������ ���� �ݻ�����, ���� �ݻ����� ���ϱ�
        //���忡 �׷��� ��������Ʈ ������ ������
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f; //������ Ŀ�� ���� ����
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f;
        HalfSize.z = 1.0f;
        //���忡 �׷��� ��������Ʈ ������ ������
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

            if (0.0f < m_Double_OnTime) //����
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
            else //�Ϲ��Ѿ�
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
            //���� ��� ����
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

        if (0.0f < m_SdOnTime) //���� ��ų �ߵ� �� �� ��... ������ ��ŵ
            return;

        Game_Mgr.Inst.DamageText(-a_Value, transform.position, Color.blue);

        m_CurHP -= a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f)
        {   //���ó��
            Time.timeScale = 0.0f;
        }
    }//void TakeDamage(float a_Value)

    void Update_Skill()
    {
        //--- ���� ���� ������Ʈ
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
        //--- ���� ���� ������Ʈ

        //--- ����ź ���� ������Ʈ
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
        //--- ����ź ���� ������Ʈ

        //--- ���� ��
        if(0.0f < m_Double_OnTime)
        {
            m_Double_OnTime -= Time.deltaTime;

            if (m_Double_OnTime < 0.0f)
                m_Double_OnTime = 0.0f;
        }
        //--- ���� ��

        //--- Sub Hero ������Ʈ
        if(0.0f < m_Sub_OnTime)
        {
            m_Sub_OnTime -= Time.deltaTime;

            if (m_Sub_OnTime < 0.0f)
                m_Sub_OnTime = 0.0f;
        }
        //--- Sub Hero ������Ʈ
    }

    public void UseSkill(SkillType a_SkType)
    {
        if (m_CurHP <= 0) //���ΰ� ����� ��ų �ߵ� ����
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
        else if(a_SkType == SkillType.Skill_1) //���� ��ų
        {
            GameObject a_Clone = Instantiate(m_WolfPrefab) as GameObject;
            a_Clone.transform.position =
                new Vector3(CameraResolution.m_ScreenMin.x - 1.0f, 0.0f, 0.0f);
        }
        else if(a_SkType == SkillType.Skill_2) //��ȣ��
        {
            if (0.0f < m_SdOnTime)
                return;

            m_SdOnTime = m_SdDuration;

            //UI ��Ÿ�� �ߵ�
            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_SdOnTime, m_SdDuration);
        }
        else if(a_SkType == SkillType.Skill_3) //����ź
        {
            if (0.0f < m_Homing_OnTime)
                return;

            m_Homing_OnTime = m_HomingDur;

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_Homing_OnTime, m_HomingDur);
        }
        else if(a_SkType == SkillType.Skill_4) //����
        {
            if (0.0f < m_Double_OnTime)
                return;

            m_Double_OnTime = m_Double_Dur;

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_Double_OnTime, m_Double_Dur);
        }
        else if(a_SkType == SkillType.Skill_5) //��ȯ�� ��ų
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

        }//else if(a_SkType == SkillType.Skill_5) //��ȯ�� ��ų

        GlobalValue.m_SkDataList[(int)a_SkType].m_CurSkillCount--;

    }//public void UseSkill(SkillType a_SkType)
}
